// Author: Sjors Thomassen, StickyLock
//#define DEBUG_AUDIO_OCCLUSION // Uncomment to debug audio occlusion
//#define USE_GREEDY_PATHFINDING // Uncomment to use Greedy Pathfinding Algorithm (Faster, but less precision)

using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(HisteraClientPresentationSystemGroup))]
public partial class AudioOcclusionSystemClient : SystemBase
{
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
        var audioListenerEntity = m_audioListenerSingletonQuery.GetSingletonEntity();

        var levelSingletonEntity = SystemAPI.GetSingletonEntity<LevelSingleton>();
        var levelSingletonComponentLookup = GetComponentLookup<LevelSingleton>(isReadOnly: true);

        var gridSingletonEntity = m_pathfindingGridQuery.GetSingletonEntity();
        var gridSettingsComponentLookup = GetComponentLookup<PathFindingGridSettings>(isReadOnly: true);
        var gridBufferLookup = GetBufferLookup<GridNode>(isReadOnly: true);

        var pathfindAudioEnabled = GlobalConfigVars.pathfindEnabled.BoolValue;
        var damping = GlobalConfigVars.occlusionDamping.FloatValue;
        var maxDistance = AudioMaxDistance.AUDIOGROUP_PATHFINDING;
        int maxRange = 440;
        int maxCost = 580;

        Entities
            .WithStoreEntityQueryInField(ref m_requirementQuery)
            .WithReadOnly(gridBufferLookup)
            .WithReadOnly(gridSettingsComponentLookup)
            .ForEach((ref AudioGroupData audioGroupData) =>
        {
            audioGroupData.occlusion = 0;
            if (audioGroupData.priority != 1 || (audioGroupData.priority == 2 && pathfindAudioEnabled))
            {
                return;
            }

            var gridSettings = gridSettingsComponentLookup[gridSingletonEntity];
            var listenerPosition = SystemAPI.GetComponent<LocalToWorld>(audioListenerEntity).Position;

            int3 startCoord = PathfindingHelper.CoordinateFromWorldPoint(listenerPosition, in gridSettings);
            int3 targetCoord = PathfindingHelper.CoordinateFromWorldPoint(audioGroupData.position, in gridSettings);
            var gridBuffer = gridBufferLookup[gridSingletonEntity];

            if (audioGroupData.occlusionCoordinate.IsEqual(targetCoord))
            {
                return;
            }
            audioGroupData.occlusionCoordinate = targetCoord;

            if (PathfindingHelper.GetDistanceCost(audioGroupData.gridCoordinate, startCoord) * 0.1f > maxDistance)
            {
                return;
            }

            var targetIndex = PathfindingHelper.IndexFromCoordinate(targetCoord, gridSettings);
            var startIndex = PathfindingHelper.IndexFromCoordinate(startCoord, gridSettings);
            var startCost = PathfindingHelper.GetDistanceCost(startCoord, targetCoord);

            if (startCost > maxRange || startIndex == targetIndex)
            {
                return; // Out of range or start node is the target node.
            }

            var openSet = new NativeQueue<PathNode>(Allocator.Temp);
            var neighbours = new NativeArray<int>(6, Allocator.Temp); // 6 sides will be used for neighouring nodes

            var startNode = new PathNode
            {
                index = startIndex,
                coordinate = startCoord,
                parentIndex = -1,
                hCost = startCost
            };
            openSet.Enqueue(startNode);

            var audioOcclusion = 1f;
            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.Dequeue();

                if (currentNode.index.Equals(targetIndex))
                {
                    break;
                }
                if (currentNode.hCost > maxCost)
                {
                    break;
                }

                GridNode gridNode = gridBuffer[currentNode.index];
                if (!gridNode.open)
                {
                    audioOcclusion *= damping;
                }

#if DEBUG_AUDIO_OCCLUSION
                var nodePosition = PathfindingHelper.WorldPointFromCoordinate(currentNode.coordinate, in gridSettings);
                DebugDraw.DrawBox(nodePosition, new float3(1, 1, 1) * gridSettings.nodeDiameter * .5f, quaternion.identity, gridNode.open ? UnityEngine.Color.cyan : UnityEngine.Color.red);
#endif
                int neighbourCount = PathfindingHelper.GetNeighbourIndices(currentNode.coordinate, gridSettings, ref neighbours);

                var neighbourIndex = neighbours[0];
                GridNode gridNeighbour = gridBuffer[neighbourIndex];
                PathNode lowestCostNode = new PathNode
                {
                    index =  neighbourIndex,
                    coordinate = gridBuffer[neighbourIndex].coordinate,
                    gCost = currentNode.gCost + PathfindingHelper.GetDistanceCost(in currentNode.coordinate, in gridNeighbour.coordinate),
                    hCost = PathfindingHelper.GetDistanceCost(in gridNeighbour.coordinate, in targetCoord)
                };

                for (int i = 1; i < neighbourCount; i++)
                {
                    neighbourIndex = neighbours[i];
                    gridNeighbour = gridBuffer[neighbourIndex];

                    var neighbourNode = new PathNode
                    {
                        index = neighbourIndex,
                        coordinate = gridNeighbour.coordinate,
                        gCost = currentNode.gCost + PathfindingHelper.GetDistanceCost(in currentNode.coordinate, in gridNeighbour.coordinate),
                        hCost = PathfindingHelper.GetDistanceCost(in gridNeighbour.coordinate, in targetCoord)
                    };

#if USE_GREEDY_PATHFINDING
                    if (neighbourNode.greedyfCost < lowestCostPathNode.greedyfCost)
#else
                    if (neighbourNode.fCost < lowestCostNode.fCost || (neighbourNode.fCost == lowestCostNode.fCost && neighbourNode.hCost < lowestCostNode.hCost))
#endif
                    {
                        lowestCostNode = neighbourNode;
                    }
                }

                openSet.Enqueue(lowestCostNode);
            }
            audioGroupData.occlusion = math.max(0, 1f - audioOcclusion);
        })
#if DEBUG_AUDIO_OCCLUSION
        .WithoutBurst().Run();
#else
        .ScheduleParallel();
#endif
    }
}
