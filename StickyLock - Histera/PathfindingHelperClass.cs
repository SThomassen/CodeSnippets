//Author: David Maris, StickyLock
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

public class PathfindingHelper
{
    private static readonly int3[] m_NEIGHBOURS =
    {
        new int3(1, 0, 0),
        new int3(-1,0, 0),
        new int3(0, 1, 0),
        new int3(0,-1, 0),
        new int3(0, 0, 1),
        new int3(0, 0,-1)
    };

    public static readonly int3[] directions =
        {
            new int3(1, 0, 0),
            new int3(-1,0, 0),
            new int3(0, 1, 0),
            new int3(0,-1, 0),
            new int3(0, 0, 1),
            new int3(0, 0,-1),
            // horizontal diagonal
            new int3(1, 0, 1),
            new int3(1, 0,-1),
            new int3(-1,0, 1),
            new int3(-1,0,-1),
            // upwards
            new int3(1, 1, 0),
            new int3(-1,1, 0),
            new int3(0, 1, 1),
            new int3(0, 1,-1),
            // upwards diagonal
            new int3(1, 1, 1),
            new int3(1, 1,-1),
            new int3(-1,1, 1),
            new int3(-1,1,-1),
        };

    [BurstCompile]
    public static int3 CoordinateFromWorldPoint(float3 worldPosition, in PathFindingGridSettings grid)
    {
        worldPosition -= grid.Position - new float3(grid.nodeDiameter * 0.5f, grid.nodeDiameter * 0.5f, grid.nodeDiameter * 0.5f);
        float fractionX = (worldPosition.x + (grid.Size * grid.nodeDiameter * 0.5f)) * grid.OneOverSize;
        float fractionY = worldPosition.y * grid.OneOverHeight;
        float fractionZ = (worldPosition.z + (grid.Size * grid.nodeDiameter * 0.5f)) * grid.OneOverSize;
        fractionX = math.saturate(fractionX);
        fractionY = math.saturate(fractionY);
        fractionZ = math.saturate(fractionZ);
        int x = (int)((grid.Size - 1) * fractionX);
        int y = (int)((grid.Height - 1) * fractionY);
        int z = (int)((grid.Size - 1) * fractionZ);
        return (new int3(x, y, z));
    }

    [BurstCompile]
    public static float3 WorldPointFromCoordinate(int3 coordinate, in PathFindingGridSettings grid)
    {
        return grid.CornerPosition + ((float3)coordinate * grid.nodeDiameter);
    }

    [BurstCompile]
    public static int IndexFromCoordinate(in int3 coordinate, in PathFindingGridSettings grid)
    {
        return (coordinate.y * grid.Size * grid.Size) + (coordinate.x * grid.Size) + coordinate.z;
    }

    [BurstCompile]
    public static int3 CoordinateFromIndex(in int index, in PathFindingGridSettings grid)
    {
        int y = (int)(index * grid.OneOverSize * grid.OneOverSize * grid.nodeDiameter * grid.nodeDiameter);
        int x = (int)((index - (y * grid.Size * grid.Size)) * grid.OneOverSize * grid.nodeDiameter);
        int z = index - (y * grid.Size * grid.Size) - (x * grid.Size);
        return new int3(x, y, z);
    }

    [BurstCompile]
    public static bool InsideBounds(in int3 coordinate, in PathFindingGridSettings grid)
    {
        return (coordinate.x >= 0 && coordinate.x < grid.Size && coordinate.y >= 0 && coordinate.y < grid.Height && coordinate.z >= 0 && coordinate.z < grid.Size);
    }

    [BurstCompile]
    public static int GetNeighbourIndices(int3 coordinate, in PathFindingGridSettings gridSettings, ref NativeArray<int> neighbours)
    {
        int currentIndex = 0;
        for (int i = 0; i < m_NEIGHBOURS.Length; i++)
        {
            neighbours[currentIndex] = -1;
            int3 checkCoordinate = coordinate + m_NEIGHBOURS[i];
            if (InsideBounds(checkCoordinate, gridSettings))
            {
                neighbours[currentIndex++] = (IndexFromCoordinate(checkCoordinate, gridSettings));
            }
        }
        return currentIndex;
    }

    [BurstCompile]
    public static int GetLOSNeighbours(int3 coordinate, in int allowedDirections, in PathFindingGridSettings gridSettings, ref NativeArray<LOSPathNode> neighbours)
    {
        int currentIndex = 0;
        for (int i = 0; i < m_NEIGHBOURS.Length; i++)
        {
            neighbours[currentIndex] = new LOSPathNode
            {
                index = -1
            };

            if ((allowedDirections & (1 << i)) != 0) // check if a bit is set
            {
                int3 checkCoordinate = coordinate + m_NEIGHBOURS[i];
                if (InsideBounds(checkCoordinate, gridSettings))
                {
                    neighbours[currentIndex++] = new LOSPathNode
                    {
                        index = IndexFromCoordinate(checkCoordinate, gridSettings),
                        coordinate = checkCoordinate,
                        myDirection = (1 << i)
                    };
                }
            }
        }
        return currentIndex;
    }

    [BurstCompile]
    public static int GetDistanceCost(in int3 pointA, in int3 pointB)
    {
        int3 distance = new int3(
            math.abs(pointA.x - pointB.x),
            math.abs(pointA.y - pointB.y),
            math.abs(pointA.z - pointB.z));

        int biggestIndex = 0;
        int middleIndex = 0;
        int smallestIndex = 0;
        for (int i = 1; i < 3; i++)
        {
            if (distance[i] > distance[biggestIndex])
            {
                biggestIndex = i;
            }
            if (distance[i] < distance[smallestIndex])
            {
                smallestIndex = i;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (biggestIndex != i && smallestIndex != i)
            {
                middleIndex = i;
                break;
            }
        }
        // NOTE StickyLock (Sjors): 10 is cost in 1 axis, 14 is cost in 2 axes (sqroot(10^2 + 10^2)), 17 is cost in 3 axes (sqroot(14^2 + 10^2) 
        return (17 * distance[smallestIndex]) + (14 * (distance[middleIndex] - distance[smallestIndex])) + (10 * (distance[biggestIndex] - distance[middleIndex]));
    }

    [BurstCompile]
    public struct UpdateGridJob : IJob
    {
        public DynamicBuffer<GridNode> GridBuffer;
        [ReadOnly]
        public DynamicBuffer<GridNodeData> SectorGridDataBuffer;
        public EntityCommandBuffer CommandBuffer;
        public Entity SectorGridDataEntity;
        public bool Clear;

        public void Execute()
        {
            foreach (var element in SectorGridDataBuffer)
            {
                var gridNode = GridBuffer[element.index];
                gridNode.open = Clear ? true : element.open;
                gridNode.inside = element.inside;
                GridBuffer[element.index] = gridNode;
            }
            CommandBuffer.AddComponent<DataHasBeenLoadedTag>(SectorGridDataEntity);
        }
    }

    [BurstCompile]
    public static int GetCornerIndex(NativeList<PathNode> path)
    {
        // returns the first corner in the path, starting from the beginning of the path.
        int startFcost = path[path.Length - 1].fCost;
        for (int i = path.Length - 2; i > -1; i--)
        {
            if (path[i].fCost <= startFcost)
            {
                continue;
            }

            if (path[i].fCost == path[i - 1].fCost)
            {
                return i;
            }
        }
        return 0;
    }

    public static int GetCornerIndex(int3 input)
    {
        int directions = 0;
        if (input.x > 0)
        {
            directions |= 1 << 0;
        }
        else
        {
            directions |= 1 << 1;
        }
        if (input.y >= 0)
        {
            directions |= 1 << 2;
        }
        else
        {
            directions |= 1 << 3;
        }
        if (input.z > 0)
        {
            directions |= 1 << 4;
        }
        else
        {
            directions |= 1 << 5;
        }
        switch (directions)
        {
            case (1 << 0) | (1 << 2) | (1 << 4):
                return (0);// x+1,  y+1, z+1     
            case (1 << 0) | (1 << 2) | (1 << 5):
                return (1);// x+1, y+1, z-1 
            case (1 << 1) | (1 << 2) | (1 << 4):
                return (2);// x-1, y+1, z+1 
            case (1 << 1) | (1 << 2) | (1 << 5):
                return (3);// x-11, y+1, z-1
            case (1 << 0) | (1 << 3) | (1 << 4):
                return (4);// x+1, y-1, z+1 
            case (1 << 0) | (1 << 3) | (1 << 5):
                return (5);// x+1, y-1, z-1
            case (1 << 1) | (1 << 3) | (1 << 4):
                return (6);// x-1, y-1, z+1   
            case (1 << 1) | (1 << 3) | (1 << 5):
                return (7);// x-1, y-1, z-1  
            default:
                return (0);
        }
    }
}

