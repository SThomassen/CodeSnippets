<template>
	<v-container class="line-numbers">
<pre><code class="language-js line-numbers" style="white-space: pre-wrap; background-color: transparent;">
public class Node {

    private bool m_walkable;
    private readonly Vector3 m_worldPosition;
    private readonly int m_gridX, m_gridY;
    private Node m_parent;


    #region GET/SET
    public int GridX { get { return m_gridX; } }
    public int GridY { get { return m_gridY; } }
    public bool Walkable { get { return m_walkable; } }
    public Vector3 Position { get { return m_worldPosition; } }
    public Node Parent { get { return m_parent; } }

    public int gCost, hCost;
    public int fCost { get { return gCost + hCost; } }

    public void SetParent(Node a_parent) { m_parent = a_parent; }
    #endregion

    public Node(bool a_walkable, Vector3 a_worldPos, int a_gridX, int a_gridY)
    {
        m_walkable = a_walkable;
        m_worldPosition = a_worldPos;
        m_gridX = a_gridX;
        m_gridY = a_gridY;
    } 
}

public class Pathfinder : MonoBehaviour {

    [SerializeField] private Transform m_player;
    private GridBase m_grid;

    public Transform Player
    {
        get
        {
            return m_player;
        }
    }

    /// <summary>
    /// Find the way across the planks towards the targets position
    /// </summary>
    public bool FindPath(Vector3 a_target)
    {
        m_grid.SetPath(new List< Node>());

        // Get the node from position
        Node startNode = m_grid.NodeFromWorldPoint(m_player.position);
        Node targetNode = m_grid.NodeFromWorldPoint(a_target);

        List< Node> openSet = new List< Node>();
        HashSet< Node> closedSet = new HashSet< Node>();
        openSet.Clear();
        closedSet.Clear();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                // change current node to open node when it has a smaller cost
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Close Pathfinding if the current node is the target node
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return true;
            }

            // Check the neigbouring nodes if it walkable
            foreach (Node neighbour in m_grid.GetNeighbours(currentNode))
            {
                if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    continue;

                // Caclulate movement cost
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    // set neighbour costs
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    // Set parent for retracing
                    neighbour.SetParent(currentNode);

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Retrace the path steps and store it reversed to get the players path.
    /// </summary>
    void RetracePath(Node a_start, Node a_end)
    {
        List< Node> path = new List< Node>();
        Node currentNode = a_end;

        while (currentNode != a_start)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        m_grid.SetPath(path);
    }

    int GetDistance(Node a_start, Node a_end)
    {
        int dstX = Mathf.Abs(a_start.GridX - a_end.GridX);
        int dstY = Mathf.Abs(a_start.GridY - a_end.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        else
            return 14 * dstX + 10 * (dstY - dstX);
    }
}

// 2D Grid used to calculate the walkable path
public class GridBase : MonoBehaviour {

    [SerializeField] private LayerMask m_walkableMask;
    [SerializeField] private Vector2 m_gridWorldSize;
    [SerializeField] private float m_nodeRadius;
    [SerializeField] private bool m_debugMode = false;
    private Node[,] m_grid;

    private float m_nodeDiameter;
    private int m_gridSizeX, m_gridSizeY;

    private List< Node> m_path = new List< Node>();

    #region Get/Set
    public void SetPath(List< Node> a_path)
    {
        m_path = a_path;
    }

    public List< Node> Path
    {
        get
        {
            return m_path;
        }
    }
   
    // Get Neighbouring nodes
    public List< Node> GetNeighbours(Node a_node)
    {
        List< Node> neighbours = new List< Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    continue;

                int checkX = a_node.GridX + x;
                int checkY = a_node.GridY + y;

                if (checkX >= 0 && checkX < m_gridSizeX && checkY >= 0 && checkY < m_gridSizeY)
                {
                    neighbours.Add(m_grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    #endregion

    // Use this for initialization
    void Start()
    {
        m_nodeDiameter = m_nodeRadius * 2;
        m_gridSizeX = Mathf.RoundToInt(m_gridWorldSize.x / m_nodeDiameter);
        m_gridSizeY = Mathf.RoundToInt(m_gridWorldSize.y / m_nodeDiameter);

        CreateGrid();
    }

    // Create grid and check if the node is walkable
    public void CreateGrid()
    {
        m_grid = new Node[m_gridSizeX, m_gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * m_gridWorldSize.x / 2 - Vector3.forward * m_gridWorldSize.y / 2;

        for (int x = 0; x < m_gridSizeX; x++)
        {
            for (int y = 0; y < m_gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) + Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);
                bool walkable = (Physics.CheckSphere(worldPoint, m_nodeRadius, m_walkableMask));
                m_grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Get Node from world position
    public Node NodeFromWorldPoint(Vector3 a_worldPoint)
    {
        float percentX = (a_worldPoint.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x;
        float percentY = (a_worldPoint.z + m_gridWorldSize.y / 2) / m_gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);

        return m_grid[x, y];
    }

    private void OnDrawGizmos()
    {
        if (!m_debugMode) return;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x,1, m_gridWorldSize.y));

        if (m_grid != null)
        {
            foreach (Node n in m_grid)
            {
                Gizmos.color = (n.Walkable) ? Color.white : Color.red;
                if (m_path != null)
                    if (m_path.Contains(n))
                        Gizmos.color = Color.black;

                Gizmos.DrawCube(n.Position, Vector3.one * (m_nodeDiameter - .01f));
            }
        }
    }
}
</code></pre>
</v-container>
</template>

<script>
import Prism from 'prismjs'

export default {
    mounted() {
        Prism.highlightAll();
    }
}
</script>