// Author: David Maris, Sjors Thomassen, StickyLock
//#define DEBUG_AUDIO_PATHFINDING // Uncomment to draw debug boxes
//#define USE_GREEDY_PATHFINDING // Uncomment to use Greedy Pathfinding Algorithm (Faster, but less precision)

using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/// <summary>
/// Updates the coordinates that are in the audiogroup (the coordinate of the audio group itself and the coordinate of the closest listener) and the distance to the closest listener.
/// Schedules a pathfinding job if various conditions are met.
/// </summary>
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(HisteraClientPresentationSystemGroup))]
public partial class AudioGroupPathfindingSystemClient : SystemBase
{
    private const int m_MAXNODES = 5000; //The maximum number of nodes that pathfinding system is allowed to look at before returning.

    private EntityQuery m_requirementQuery;
    private EntityQuery m_pathfindingGridQuery;
    private EntityQuery m_audioListenerSingletonQuery;

    protected override void OnCreate()
    {
        m_audioListenerSingletonQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<AudioListenerTag, LocalToWorld>()
            .Build(this);

        m_pathfindingGridQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PathFindingGridSettings, GridNode>()
            .Build(this);

        RequireForUpdate(m_audioListenerSingletonQuery);
        RequireForUpdate(m_pathfindingGridQuery);
        RequireForUpdate(m_requirementQuery);
        RequireForUpdate<LocalOwnedPlayerTag>();
        RequireForUpdate<LevelSingleton>();
    }

    protected override void OnUpdate()
    {
        var pathfindEnabled = GlobalConfigVars.pathfindEnabled.BoolValue;
        var pathfindCorner = GlobalConfigVars.pathfindCornerAudio.BoolValue;
        var audioListenerEntity = m_audioListenerSingletonQuery.GetSingletonEntity();

        var levelSingletonEntity = SystemAPI.GetSingletonEntity<LevelSingleton>();
        var levelSingletonComponentLookup = GetComponentLookup<LevelSingleton>(isReadOnly: true);
       
        var gridSingletonEntity = m_pathfindingGridQuery.GetSingletonEntity();
        var gridSettingsComponentLookup = GetComponentLookup<PathFindingGridSettings>(isReadOnly: true);
        var gridBufferLookup = GetBufferLookup<GridNode>(isReadOnly: true);

        var maxNodes = m_MAXNODES;
        var maxDistance = AudioMaxDistance.AUDIOGROUP_PATHFINDING;

        Entities
            .WithStoreEntityQueryInField(ref m_requirementQuery)
            .WithReadOnly(gridBufferLookup)
            .WithReadOnly(gridSettingsComponentLookup)
            .WithReadOnly(levelSingletonComponentLookup)
            .WithAll<AudioGroupPathFindTag>()
            .ForEach((Entity entity, ref AudioGroupData audioGroupData) =>
            {
                // TODO StickyLock (Marco): If priority for the glitch-related audiogroups gets changed, this code could be removed
                if (!levelSingletonComponentLookup[levelSingletonEntity].IsGlitchOngoing()
                    && (SystemAPI.HasComponent<GlitchingSectorAudioSingleton>(entity) || SystemAPI.HasComponent<ProjectionPointAudioSingleton>(entity)))
                {
                    return; // Don't pathfind to glitch-related audio when there's no glitch ongoing
                }

                var listenerPosition = SystemAPI.GetComponent<LocalToWorld>(audioListenerEntity).Position;

                //These old values are used to check if the listener and audiogroup are still in the same position.
                var oldListenerCoordinate = audioGroupData.listenerCoordinate;
                var oldAudioGroupCoordinate = audioGroupData.gridCoordinate;

                //Setting the coordinates for all the audio groups.
                var gridSettings = gridSettingsComponentLookup[gridSingletonEntity];
                audioGroupData.listenerCoordinate = PathfindingHelper.CoordinateFromWorldPoint(listenerPosition, gridSettings);
                audioGroupData.gridCoordinate = PathfindingHelper.CoordinateFromWorldPoint(audioGroupData.position, gridSettings);

                //Setting the distance to listener for all the audio groups
                audioGroupData.distanceToListener = PathfindingHelper.GetDistanceCost(audioGroupData.gridCoordinate, audioGroupData.listenerCoordinate) * 0.1f;
                if (audioGroupData.priority != 2 || !pathfindEnabled)
                {
                    return;// No pathfinding to audiogroups with low priority.
                }
                if (audioGroupData.distanceToListener > maxDistance)
                {
                    // We don't want to do pathfinding halfway across the level.
                    // Assuming that the pathfinding is not going to result in a straight line, we can have the distance where we start to pathfind (this condition)
                    // a bit lower than the max distance (35) of the path. This way we can return more often, resulting in better performance.
                    return;
                }

                if (audioGroupData.gridCoordinate.IsEqual(oldAudioGroupCoordinate) && audioGroupData.listenerCoordinate.IsEqual(oldListenerCoordinate))
                {
                    // TODO StickyLock (David): an optimization would be to ignore this check for the glitch and return early regardless of wether a glitch is ongoing.
                    // Because realisticly, even if the player is standing still, he would not neccessairly be noticing that the occlussion of high priority audiogroups that are also standing still has not changed with loading in of a new level during the glitch.
                    if (!levelSingletonComponentLookup[levelSingletonEntity].IsGlitchOngoing())
                    {
                        // The situation is the same as in the previous frame, no new pathfinding is required.
                        return;
                    }
                }

                #region Find Path
                int maxRange = 440;
                int maxCost = 580;
                int3 startCoord = audioGroupData.gridCoordinate;
                int3 targetCoord = audioGroupData.listenerCoordinate;
                var gridBuffer = gridBufferLookup[gridSingletonEntity];

                var targetIndex = PathfindingHelper.IndexFromCoordinate(targetCoord, gridSettings);
                var startIndex = PathfindingHelper.IndexFromCoordinate(startCoord, gridSettings);
                var startCost = PathfindingHelper.GetDistanceCost(startCoord, targetCoord);
                if (startCost > maxRange || startIndex == targetIndex)
                {
                    return; // Out of range or start node is the target node.
                }

                NativeArray<int> neighbours = new NativeArray<int>(6, Allocator.Temp);
                var gridNodeArray = gridBuffer.AsNativeArray();
                if (!gridNodeArray[targetIndex].open)
                {
                    #region GetClosestOpenNeighbour
                    int neighbourCount = PathfindingHelper.GetNeighbourIndices(targetCoord, gridSettings, ref neighbours);
                    int distance = int.MaxValue;
                    int closestNeighbour = -1;
                    for (int i = 0; i < neighbourCount; i++)
                    {
                        if (neighbours[i] == -1)
                        {
                            break;
                        }
                        GridNode gridNeighbour = gridNodeArray[neighbours[i]];

                        if (!gridNeighbour.open)
                        {
                            continue;
                        }

                        var tempDistance = PathfindingHelper.GetDistanceCost(gridNeighbour.coordinate, startCoord);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            closestNeighbour = neighbours[i];
                        }
                    }
                    #endregion

                    if (closestNeighbour == -1)
                    {
                        return; // No open neighbouring nodes.
                    }
                    targetIndex = closestNeighbour;
                    targetCoord = gridNodeArray[targetIndex].coordinate;
                }

                NativeList<PathNode> pathResult = new NativeList<PathNode>(Allocator.Temp);
                NativeList<PathNode> openSet = new NativeList<PathNode>(Allocator.Temp);
                NativeParallelHashMap<int, PathNode> closedSet = new NativeParallelHashMap<int, PathNode>(maxNodes / 4, Allocator.Temp);

                var startNode = new PathNode
                {
                    index = startIndex,
                    coordinate = startCoord,
                    parentIndex = -1,
                    hCost = startCost
                };
                openSet.Add(startNode);
                if (maxCost == 0 || (openSet[0].hCost + openSet[0].hCost) < maxCost)
                {
                    maxCost = openSet[0].hCost + openSet[0].hCost;
                }

                while (openSet.Length > 0)
                {
                    int currentNodeOpenSetIndex = GetLowestFNodeCostOpenSetIndex(openSet);
                    PathNode currentNode = openSet[currentNodeOpenSetIndex];

                    if (currentNode.index.Equals(targetIndex))
                    {
                        RetracePath(in startCoord, in currentNode, in closedSet, in pathResult, in gridSettings);
                        break;
                    }
                    if (currentNode.hCost > maxCost)
                    {
                        break;
                    }

                    openSet.RemoveAtSwapBack(currentNodeOpenSetIndex);
                    int neighbourCount = PathfindingHelper.GetNeighbourIndices(currentNode.coordinate, gridSettings, ref neighbours);
                    for (int i = 0; i < neighbourCount; i++)
                    {
                        var neighbourIndex = neighbours[i];
                        if (neighbourIndex == -1)
                        {
                            break;
                        }

                        if (closedSet.ContainsKey(neighbourIndex))
                        {
                            continue;
                        }

                        GridNode gridNeighbour = gridNodeArray[neighbourIndex];

                        if (!gridNeighbour.open)
                        {
                            continue;
                        }

                        PathNode neighbour;
                        int neighbourOpenSetIndex = FindSetIndex(openSet, neighbourIndex);
                        int newMovementCostToNeighbour = currentNode.gCost + PathfindingHelper.GetDistanceCost(in currentNode.coordinate, in gridNeighbour.coordinate);

                        if (neighbourOpenSetIndex == -1)
                        {
                            neighbour = new PathNode
                            {
                                index = neighbourIndex,
                                coordinate = gridNeighbour.coordinate,
                                parentIndex = -1
                            };
                        }
                        else
                        {
                            neighbour = openSet[neighbourOpenSetIndex];
                            if (newMovementCostToNeighbour >= openSet[neighbourOpenSetIndex].gCost)
                            {
                                continue;
                            }
                        }

                        if (currentNode.parentIndex != -1)
                        {
                            PathNode grandparent = closedSet[currentNode.parentIndex];
                            if (DiagonalNeighbour(in neighbour.coordinate, in grandparent.coordinate))
                            {
                                newMovementCostToNeighbour = grandparent.gCost + PathfindingHelper.GetDistanceCost(in grandparent.coordinate, in neighbour.coordinate);
                                neighbour.parentIndex = currentNode.parentIndex;
                            }
                            else
                            {
                                neighbour.parentIndex = currentNode.index;
                            }
                        }
                        else
                        {
                            neighbour.parentIndex = currentNode.index;
                        }

                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = PathfindingHelper.GetDistanceCost(in neighbour.coordinate, in targetCoord);

                        if (neighbourOpenSetIndex == -1)
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet[neighbourOpenSetIndex] = neighbour;
                        }
                    }
                    closedSet[currentNode.index] = currentNode;
                }
                #endregion

                #region Complete Path
                audioGroupData.cornerposition = audioGroupData.position;
                audioGroupData.pathDistanceToListener = 0;
                audioGroupData.occlusion = 0;

                if (pathResult.Length <= 0)
                {
                    return;
                }

                var firstNode = pathResult[0];
                var lastNode = pathResult[pathResult.Length - 1];
                float crowDistance = lastNode.hCost * 0.1f * gridSettings.nodeDiameter;
                float pathDistance = firstNode.gCost * 0.1f * gridSettings.nodeDiameter;
                float occlusion = (pathDistance - crowDistance) * 0.1f; // A path distance difference of 10 cubes means 100% occlusion.
                audioGroupData.occlusion = occlusion;

                if (pathResult.Length > 2 && pathfindCorner)
                {
                    var cornerIndex = PathfindingHelper.GetCornerIndex(pathResult);
                    if (cornerIndex > 0)
                    {
                        var cornerNode = pathResult[cornerIndex];
                        float3 audioPos = PathfindingHelper.WorldPointFromCoordinate(gridBuffer[cornerNode.index].coordinate, gridSettings);
                        audioGroupData.cornerposition = audioPos;

                        var distanceSourceToCorner = math.distance(audioPos, audioGroupData.position);
                        audioGroupData.pathDistanceToListener = distanceSourceToCorner;

#if DEBUG_AUDIO_PATHFINDING
                        DebugDraw.Sphere(audioPos, 1, UnityEngine.Color.magenta, 1);
#endif
                    }
                }
                #endregion
            })
#if !DEBUG_AUDIO_PATHFINDING
            .ScheduleParallel();
#else
            .WithoutBurst().Run();
#endif
    }

    private static int FindSetIndex(NativeList<PathNode> set, int index)
    {
        for (int i = 0; i < set.Length; i++)
        {
            if (set[i].index == index)
            {
                return i;
            }
        }
        return -1;
    }

    private static int GetLowestFNodeCostOpenSetIndex(NativeList<PathNode> openSet)
    {
        PathNode lowestCostPathNode = openSet[0];
        int index = 0;
        for (int i = 1; i < openSet.Length; i++)
        {
            PathNode pathNode = openSet[i];
#if USE_GREEDY_PATHFINDING
            if (pathNode.greedyfCost < lowestCostPathNode.greedyfCost)
#else
            if (pathNode.fCost < lowestCostPathNode.fCost || (pathNode.fCost == lowestCostPathNode.fCost && pathNode.hCost < lowestCostPathNode.hCost))
#endif
            {
                index = i;
                lowestCostPathNode = pathNode;
            }
        }
        return index;
    }

    private static void RetracePath(in int3 startCoordinate, in PathNode endNode, in NativeParallelHashMap<int, PathNode> closedSet, in NativeList<PathNode> path, in PathFindingGridSettings gridSettings)
    {
        PathNode currentNode = endNode;
        while (!currentNode.coordinate.Equals(startCoordinate))
        {
#if DEBUG_AUDIO_PATHFINDING
            var pos = PathfindingHelper.WorldPointFromCoordinate(currentNode.coordinate, gridSettings);
            DebugDraw.DrawBox(pos, new float3(1, 1, 1) * gridSettings.nodeDiameter * .5f, quaternion.identity, UnityEngine.Color.blue, .1f);
#endif
            path.Add(currentNode);
            currentNode = closedSet[currentNode.parentIndex];
        }
    }

    private static bool DiagonalNeighbour(in int3 coordinateA, in int3 coordinateB)
    {
        int3 difference = coordinateA - coordinateB;
        return (difference.x < 2 && difference.x > -2 && difference.y < 2 && difference.y > -2 && difference.z < 2 && difference.z > -2);
    }
}
