using UnityEngine;
using System.Collections.Generic;

public class AStarPathfinding : MonoBehaviour
{
    public Transform startPoint; // Start point for pathfinding
    public Transform targetPoint; // Target point for pathfinding
    private Grid grid; // Reference to the grid

    private void Start()
    {
        // Get reference to the Grid component
        grid = FindObjectOfType<Grid>();

        // Ensure the grid is initialized before proceeding
        if (grid == null)
        {
            Debug.LogError("Grid reference not found! Make sure the Grid object is in the scene.");
            return;
        }

        // Delay the pathfinding if necessary
        Invoke("BeginPathfinding", 0.1f);
    }

    private void BeginPathfinding()
    {
        if (startPoint == null || targetPoint == null)
        {
            Debug.LogError("Start or Target point is missing!");
            return;
        }

        Debug.Log("Starting pathfinding...");
        FindPath(startPoint.position, targetPoint.position);
    }

    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Ensure the grid is fully initialized before proceeding
        if (grid.grid == null || grid.grid.Length == 0)
        {
            Debug.LogError("Grid has not been created. Ensure Grid component is set up properly.");
            return;
        }

        Node startNode = grid.GetNodeFromWorldPosition(startPos);
        Node targetNode = grid.GetNodeFromWorldPosition(targetPos);

        // Open and closed lists for A* algorithm
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];

            // Look for the node with the smallest fCost (fCost = gCost + hCost)
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If we have reached the target node, retrace the path
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (neighbor.walkable && !closedList.Contains(neighbor))
                {
                    int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeighbor < neighbor.gCost || !openList.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        // Visualize the path (Optional: Debugging only)
        foreach (Node node in path)
        {
            Debug.Log("Path node at: " + node.worldPosition);
        }
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Check all adjacent nodes in a 3x3 grid
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue; // Skip the current node

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grid.width && checkY >= 0 && checkY < grid.height)
                {
                    neighbors.Add(grid.grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
