using UnityEngine;

namespace AI
{
    public class PathfindingGrid : MonoBehaviour
    {
        public Transform Player;
        public LayerMask unwalkableMasks;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        Node[,] grid;
        float nodeDiameter;
        int gridSizeX, gridSizeY;

        private void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            grid = new Node[gridSizeX, gridSizeY];
        }

        public void CreateGrid()
        {           
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMasks));
                    grid[x, y] = new Node(walkable, worldPoint);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            int x = Mathf.RoundToInt((gridSizeX) * percentX);
            int y = Mathf.RoundToInt((gridSizeY) * percentY)-1;
            return grid[x, y];
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(Player.position
                    );
                foreach (Node n in grid)
                {
                    if (playerNode.worldPosition == n.worldPosition)
                    {
                        Gizmos.color = Color.cyan;   
                    }
                    else
                    {
                        Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }
}

