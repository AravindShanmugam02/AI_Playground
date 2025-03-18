using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    // Algorithm Related
    public enum Algorithm
    {
        DFS,
        BFS,
        Dijkstra,           // Dijkstra -> Best in Unknown Maps, Unknown Destination, Path to multiple nodes
        AStar,              // A*       -> Best in Known destination, Quicker route to a marked destination, Path to single node
        None
    }

    [SerializeField]
    Algorithm algo;

    // [TODO] No Use Yet
    bool requestForChangeOfAlgorithmTriggered;

    // For A* I would need two data structures which can search soon to its elements.
    List<Node> openSet; // Contains nodes that are yet to be explored. // Using list for openSet because can't use openSet[0] if it was HashSet.
    HashSet<Node> closedSet; // Contains nodes that are already explored. // Using HashSet because we need only unique values in them and we are not going to lookup in closedSet like closedSet[0].

    // Path
    [SerializeField]
    List<Node> pathNodeList;

    // Grid
    CustomGridLayout customGridLayout;

    private void Awake()
    {

    }

    private void Start()
    {
        customGridLayout = GetComponent<CustomGridLayout>();

        // [TODO] Hardcoding it for now
        algo = Algorithm.AStar;
    }

    private void Update()
    {

    }

    // This is basically used to calculate HCost all the time. And sometimes other costs too!
    public float GetDistanceBetweenNodes(Node fromNode, Node toNode)
    {
        float dist = 0.0f;
        
        // Using Manhattan distance formula as this is a grid-based movement.
        dist = (Mathf.Abs(fromNode.NodeCoordsIn2DArray.x - toNode.NodeCoordsIn2DArray.x)) + (Mathf.Abs(fromNode.NodeCoordsIn2DArray.y - toNode.NodeCoordsIn2DArray.y));
        return dist;
    }

    public List<Node> FindPath(Node nodeA, Node nodeB)
    {
        pathNodeList = new List<Node>();
        pathNodeList.Clear();

        switch (algo)
        {
            case Algorithm.DFS:
                break;
            case Algorithm.BFS:
                break;
            case Algorithm.Dijkstra:
                break;
            case Algorithm.AStar:
                AStar(nodeA, nodeB);
                break;
            default:
                break;
        }

        return pathNodeList;
    }

    private void AStar(Node startNode, Node destinationNode)
    {
        List<Node> pathUsingAStar = new List<Node>();
        openSet = new List<Node>();
        closedSet = new HashSet<Node>();

        // Add startNode to openSet
        openSet.Add(startNode);

        // Iterate through the openSet to get values one by one until openSet is empty
        while(openSet.Count > 0) // [TIME COMPLEXITY: O(neighbourNode)]
        {
            // Assign startNode to currentNode. Just as formality for first start node.
            Node currentNode = openSet[0];

            // Loop through the openSet to find node with lowest fCost. Iterating from 1 as 0 is assigned to currentNode in above line.
            for(int i = 1; i < openSet.Count; i++) // [TIME COMPLEXITY: O(neighbourNode)]
            {
                if(openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            // Once a suitable currentNode is found with lowest fCost or maybe hCost we remove it from openSet as it is about to be explored.
            openSet.Remove(currentNode);

            // 
            closedSet.Add(currentNode);

            // If currentNode is the destinationNode, then retrace the path.
            if(currentNode == destinationNode)
            {
                RetracePath(startNode, destinationNode);
                return;
            }

            // if not destinationNode, we now find out the neighbours of the node.
            List<Node> neighbours = customGridLayout.GetNeighbourNodes(currentNode);

            // Calculate the gCost for getting from currentNode to that neighbournode.
            foreach(Node neighbourNode in neighbours)
            {
                // We ignore the obstacle covered nodes as they are non traversable and the nodes that are already in closedSet, which means they have been already explored.
                if (!neighbourNode.IsTraversable && closedSet.Contains(neighbourNode)) continue;

                float newMovementCostToNeighbour = currentNode.GCost + GetDistanceBetweenNodes(currentNode, neighbourNode);

                // If (the movement cost from current node to neightbour node is cheaper than neighbour node's GCost) OR (the neighbour node is not explored yet)
                if (newMovementCostToNeighbour < neighbourNode.GCost || !openSet.Contains(neighbourNode))
                {
                    // Assigning a new GCost as this is the shorter than it's old value (OR) giving it a value for the first time as it hasn't been added to openset yet, so that it can be chosen to be explored.
                    neighbourNode.GCost = newMovementCostToNeighbour;

                    // HCost is the distance between a node to destination node.
                    neighbourNode.HCost = GetDistanceBetweenNodes(neighbourNode, destinationNode);

                    // Setting a parent node for tracking back if this node makes it to the pathUsingAStar List.
                    neighbourNode.ParentNode = currentNode;

                    // if not in openSet add them, so that these neighbours can be explored based on their FCost priority
                    if(!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node destinationNode)
    {
        Node n = destinationNode;

        // Runs until it reaches startNode, and won't add startNode as it is the node where player is already on.
        while(startNode != n)
        {
            pathNodeList.Add(n);
            n = n.ParentNode;
        }
        pathNodeList.Reverse();
    }
}
