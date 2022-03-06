using UnityEngine;

namespace AI
{
    public class PathfindingGrid : MonoBehaviour
    {
        public LayerMask unwalkableMasks;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        
        Node[,] grid;
        float nodeDiameter;
        int gridSizeX, gridSizeY;


        private void Start()
        {
            
        }

        public void CreateGrid()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
            for(int x = 0; x < gridSizeX; x++)
            {
                for(int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMasks));
                    grid[x, y] = new Node(walkable, worldPoint);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x + 11, 1, gridWorldSize.y + 58));
            if (grid != null)
            {
                foreach(Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red; 
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }

    }
}

