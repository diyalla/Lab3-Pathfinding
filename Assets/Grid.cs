using UnityEngine;

public class Grid : MonoBehaviour
{
    public int width = 200; // Width of the grid
    public int height = 200; // Height of the grid
    public float nodeRadius = 2f; // Radius of each node
    public Node[,] grid; // 2D array to hold nodes

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[width, height]; // Initialize the grid array
        Vector3 startPos = transform.position; // Starting position of the grid

        // Create nodes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPos = startPos + new Vector3(x * nodeRadius, 0, y * nodeRadius);
                grid[x, y] = new Node(worldPos, x, y); // Create a new node
            }
        }
        Debug.Log("Grid created successfully!");
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        // Calculate the node indices based on world position
        int x = Mathf.RoundToInt((worldPosition.x - transform.position.x) / nodeRadius);
        int y = Mathf.RoundToInt((worldPosition.z - transform.position.z) / nodeRadius);

        // Clamp values to avoid out of bounds errors
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return grid[x, y];
    }
}

public class Node
{
    public Vector3 worldPosition; // Position of the node in the world
    public int gridX; // Node's X index in the grid
    public int gridY; // Node's Y index in the grid
    public bool walkable = true; // Whether the node is walkable

    public int gCost; // Cost from the start node
    public int hCost; // Heuristic cost to the target node
    public Node parent; // Reference to the parent node (used to retrace the path)

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(Vector3 worldPos, int x, int y)
    {
        worldPosition = worldPos;
        gridX = x;
        gridY = y;
    }
}
