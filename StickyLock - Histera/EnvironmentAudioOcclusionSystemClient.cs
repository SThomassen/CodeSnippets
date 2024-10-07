// Author: Sjors Thomassen, StickyLock
#define DEBUG_AUDIO_OCCLUSION // Uncomment to draw debug boxes

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(BeginPresentationEntityCommandBufferSystem))]
public partial class EnvironmentAudioOcclusionSystemClient : SystemBase
{
    // 18 directions are set at PathfindingHelperClass.directions array;
    // 6 directions is only the basic directions (up, down, forward, backwards, left, right)
    // 10 directions will include horizontal diagonals
    // 14 directions will include diagonal upwards from the basic directions
    // 18 directions will include diagonal upwards from the horizontal diagonal directions
    private const int m_DIRECTIONCOUNT = 18;

    private EntityQuery m_localOwnedPlayerQuery;
    private EntityQuery m_pathfindingGridQuery;

    protected override void OnCreate()
    {
        m_localOwnedPlayerQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LocalOwnedPlayerTag, LocalToWorld>()
            .WithNone<CharacterNoClipTag>()
            .Build(this);
        m_pathfindingGridQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PathFindingGridSettings>()
            .Build(this);

        RequireForUpdate(m_localOwnedPlayerQuery);
        RequireForUpdate(m_pathfindingGridQuery);
        RequireForUpdate<LevelSingleton>();
        RequireForUpdate<LocalOwnedPlayerTag>();
        RequireForUpdate<EnvironmentAudioOcclusionState>();
    }

    protected override void OnUpdate()
    {
        // Get required data 
        var eyeHeightComponentLookup = GetComponentLookup<AimData>(true);
        var localOwnedPlayerEntity = m_localOwnedPlayerQuery.GetSingletonEntity();
        var localPlayerPosition = eyeHeightComponentLookup[localOwnedPlayerEntity].eyeWorldPosition;

        var gridSettingsComponentLookup = GetComponentLookup<PathFindingGridSettings>(true);
        var gridSingletonEntity = m_pathfindingGridQuery.GetSingletonEntity();
        var gridSettings = gridSettingsComponentLookup[gridSingletonEntity];

        var environmentAudioOcclusionEntity = SystemAPI.GetSingletonEntity<EnvironmentAudioOcclusionState>();
        var environmentAudioOcclusionState = GetComponentLookup<EnvironmentAudioOcclusionState>(false)[environmentAudioOcclusionEntity];

        var levelSingletonComponentLookup = GetComponentLookup<LevelSingleton>(true);
        var levelSingleton = SystemAPI.GetSingletonEntity<LevelSingleton>();

        // Check if player has moved
        var startCoordinates = PathfindingHelper.CoordinateFromWorldPoint(localPlayerPosition, gridSettings);
        if (Equals(startCoordinates, environmentAudioOcclusionState.playerCoordinate) || levelSingletonComponentLookup[levelSingleton].IsGlitchOngoing())
        {
            // Current position is already calculated
            return;
        }
        environmentAudioOcclusionState.playerCoordinate = startCoordinates;
        SystemAPI.SetComponent(environmentAudioOcclusionEntity, environmentAudioOcclusionState);

        var gridBufferLookup = GetBufferLookup<GridNode>(true);
        var gridBuffer = gridBufferLookup[gridSingletonEntity];
        
        // Initiate array containers
        var insideOcclusionValues = new NativeArray<float>(m_DIRECTIONCOUNT, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var outsideOcclusionValues = new NativeArray<float>(m_DIRECTIONCOUNT, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var roomSizeValues = new NativeArray<int>(m_DIRECTIONCOUNT, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var reverbValues = new NativeArray<float>(m_DIRECTIONCOUNT, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        var damping = GlobalConfigVars.occlusionDamping.FloatValue;
        var nodeRange = GlobalConfigVars.occlusionRange.IntValue;

        var jobHandles = new NativeArray<JobHandle>(m_DIRECTIONCOUNT, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        // Create threads for audio occlusion
        for (int i = 0; i < m_DIRECTIONCOUNT; i++)
        {
            EnvironmentAudioNodeStateJob nodeStateJob = new EnvironmentAudioNodeStateJob()
            {
                range = nodeRange,
                wallDamping = damping,
                currentCornerIndex = i,
                startCoordinates = startCoordinates,
                gridSettings = gridSettings,
                gridNodeBuffer = gridBuffer,
                insideOcclusion = insideOcclusionValues,
                outsideOcclusion = outsideOcclusionValues,
                roomSize = roomSizeValues,
                reverb = reverbValues,
            };

#if !DEBUG_AUDIO_OCCLUSION
            JobHandle handle = nodeStateJob.Schedule();
            jobHandles[i] = handle;
#else
            nodeStateJob.Run();
#endif
        }
#if !DEBUG_AUDIO_OCCLUSION
        var dependencies = JobHandle.CombineDependencies(jobHandles);
        jobHandles.Dispose();
#endif

#if !DEBUG_AUDIO_OCCLUSION
        var resultJob =
#endif
        Entities
        #region ReadOnly
        .WithReadOnly(reverbValues)
        .WithReadOnly(roomSizeValues)
        .WithReadOnly(insideOcclusionValues)
        .WithReadOnly(outsideOcclusionValues)
#endregion
#region DisposeOnCompletion
        .WithDisposeOnCompletion(reverbValues)
        .WithDisposeOnCompletion(roomSizeValues)
        .WithDisposeOnCompletion(insideOcclusionValues)
        .WithDisposeOnCompletion(outsideOcclusionValues)
#endregion
        .ForEach((ref DynamicBuffer<GridNode> gridNodeBuffer, ref EnvironmentAudioOcclusionState occlusionState) =>
        {
            float reverb = 0;
            int roomSize = 0;
            float insideOcclusion = 0;
            float outsideOcclusion = 0;
            for (int i = 0; i < m_DIRECTIONCOUNT; i++)
            {
                reverb += reverbValues[i];
                roomSize += roomSizeValues[i];
                insideOcclusion += insideOcclusionValues[i];
                outsideOcclusion += outsideOcclusionValues[i];
            }

            occlusionState.roomSize = roomSize;
            occlusionState.reverb = reverb / m_DIRECTIONCOUNT;
            occlusionState.insideOcclusion = insideOcclusion / m_DIRECTIONCOUNT;
            occlusionState.outsideOcclusion = outsideOcclusion / m_DIRECTIONCOUNT;
        })
#if !DEBUG_AUDIO_OCCLUSION
        .Schedule(dependencies); // scheduled after jobs are done
        EnvironmentAudioOcclusionCompleteSystemClient.EnvironmentOcclusionJob = resultJob;
#else
    .WithoutBurst().Run();
#endif

    }
}

// Note (Marco): This system will probably be moved to its own file and become more generic (JobCompleteSystemClient or something)
//               This will happen in pt2 of the pull request.
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
// Schedule system as late during simulation group as possible, so that the spawned jobs (which can be quite long-running jobs) have a lot of time to complete.
// This is limited by having to run before the first structural change after the line of sight system.
[UpdateInGroup(typeof(HisteraClientPresentationSystemGroup), OrderFirst = true)]
public partial class EnvironmentAudioOcclusionCompleteSystemClient : SystemBase
{
    public static JobHandle EnvironmentOcclusionJob { get; set; }

    protected override void OnUpdate()
    {
        EnvironmentOcclusionJob.Complete();
    }
}

public struct EnvironmentAudioNodeStateJob : IJob
{
    public float wallDamping;

    public int range;
    public int currentCornerIndex;

    public int3 startCoordinates;

    public PathFindingGridSettings gridSettings;
    [NativeDisableContainerSafetyRestriction] // pathfinding only reads from the gridNodeBuffer
    public DynamicBuffer<GridNode> gridNodeBuffer;

    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float> insideOcclusion;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float> outsideOcclusion;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float> reverb;
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<int> roomSize;

    public void Execute()
    {
        // Initial values
        float insideNodes = 0;
        float outsideNodes = 0;
        float reverbNodes = 0;
        int roomNodes = 0;

        var startIndex = PathfindingHelper.IndexFromCoordinate(startCoordinates, gridSettings);
        var direction = PathfindingHelper.directions[currentCornerIndex];

#if DEBUG_AUDIO_OCCLUSION
        var startPosition = PathfindingHelper.WorldPointFromCoordinate(startCoordinates, gridSettings);
        DebugDraw.Line(startPosition, startPosition + (direction * range), UnityEngine.Color.magenta);
#endif

        float damping = 1;
        int processedNodes = 0;
        for (int i = 0; i < range; i++)
        {
            var nodeCoordinates = startCoordinates + (direction * i);
            // Check if node is still in range of the level
            if (PathfindingHelper.InsideBounds(nodeCoordinates, gridSettings))
            {
                processedNodes++;
                var nodeIndex = PathfindingHelper.IndexFromCoordinate(nodeCoordinates, gridSettings);
                GridNode gridNode = gridNodeBuffer[nodeIndex];
                damping *= (gridNode.open) ? 1 : wallDamping; // Apply damping if previous nodes were blocked
                if (gridNode.open)
                {
                    if (gridNode.inside)
                    {
                        insideNodes += damping;

                        if (damping >= 1)
                        {
                            roomNodes++;
                        }

#if DEBUG_AUDIO_OCCLUSION
                        var gridPosition = PathfindingHelper.WorldPointFromCoordinate(gridNode.coordinate, gridSettings);
                        var color = UnityEngine.Color.Lerp(UnityEngine.Color.yellow, UnityEngine.Color.magenta, 1f - damping);
                        DebugDraw.DrawBox(gridPosition, new float3(1, 1, 1) * gridSettings.nodeDiameter * .5f, quaternion.identity, color);
#endif
                    }
                    else
                    {
                        outsideNodes += damping;
                        if (damping >= 1)
                        {
                            reverbNodes++;
                        }

#if DEBUG_AUDIO_OCCLUSION
                        var gridPosition = PathfindingHelper.WorldPointFromCoordinate(gridNode.coordinate, gridSettings);
                        DebugDraw.DrawBox(gridPosition, new float3(1, 1, 1) * gridSettings.nodeDiameter * .5f, quaternion.identity, UnityEngine.Color.blue);
#endif
                    }
                }
#if DEBUG_AUDIO_OCCLUSION
                else
                {
                    var gridPosition = PathfindingHelper.WorldPointFromCoordinate(gridNode.coordinate, gridSettings);
                    DebugDraw.DrawBox(gridPosition, new float3(1, 1, 1) * gridSettings.nodeDiameter * .5f, quaternion.identity, UnityEngine.Color.red);
                }
#endif
            }
        }
        
        roomSize[currentCornerIndex] = roomNodes;
        reverb[currentCornerIndex] = reverbNodes / processedNodes;
        insideOcclusion[currentCornerIndex] = 1f - (insideNodes / processedNodes);
        outsideOcclusion[currentCornerIndex] = 1f - (outsideNodes / processedNodes);
    }
}
