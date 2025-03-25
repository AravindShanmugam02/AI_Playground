using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    // Algorithm Related
    public enum Algorithm
    {
        None,
        //DFS,
        //BFS,
        Dijkstra,           // Dijkstra -> Best in Unknown Maps, Unknown Destination, Path to multiple nodes
        AStar               // A*       -> Best in Known destination, Quicker route to a marked destination, Path to single node
    }

    // [TODO] No Use Yet
    bool requestForChangeOfAlgorithmTriggered;

    // For A* I would need two data structures which can search soon to its elements.
    List<Node> openSet; // Contains nodes that are yet to be explored. // Using list for openSet because can't use openSet[0] if it was HashSet.
    HashSet<Node> closedSet; // Contains nodes that are already explored. // Using HashSet because we need only unique values in them and we are not going to lookup in closedSet like closedSet[0].

    // Getter
    public List<Node> OpenSet { get { return openSet; } }
    public List<Node> ClosedSet
    {
        get
        {
            List<Node> cSet = new List<Node>();
            if (closedSet.Count > 0)
            {
                foreach (Node n in closedSet)
                {
                    cSet.Add(n);
                }
            }
            return cSet;
        }
    }

    // Grid
    CustomGridLayout customGridLayout;

    void Start()
    {
        openSet = new List<Node>();
        closedSet = new HashSet<Node>();
        customGridLayout = GetComponent<CustomGridLayout>();
    }

    void Update()
    {

    }

    // This is basically used to calculate HCost all the time. And sometimes other costs too!
    public float GetDistanceBetweenNodes(Node fromNode, Node toNode) 
    {
        float dist;
        
        // Using Manhattan distance formula as this is a grid-based movement.
        dist = (Mathf.Abs(fromNode.NodeCoordsIn2DArray.x - toNode.NodeCoordsIn2DArray.x)) + (Mathf.Abs(fromNode.NodeCoordsIn2DArray.y - toNode.NodeCoordsIn2DArray.y));

        return dist;
    }

    public List<Node> FindPath(Node nodeA, Node nodeB, Algorithm currentAlgo)
    {
        List<Node> pathNodeList = new List<Node>();

        customGridLayout.ResetNodesCosts(); // Resetting the Costs stored in Nodes for different algorithm or for next pathfinding with same algorithm.

        switch (currentAlgo)
        {
            //case Algorithm.DFS:
            //    break;
            //case Algorithm.BFS:
            //    break;
            case Algorithm.Dijkstra:
                pathNodeList = Dijkstra(nodeA, nodeB);
                break;
            case Algorithm.AStar:
                pathNodeList = AStar(nodeA, nodeB);
                break;
            default:
                // None
                break;
        }

        return pathNodeList;
    }

    private List<Node> AStar(Node startNode, Node destinationNode)
    {
        List<Node> pathUsingAStar = new List<Node>();
        openSet = new List<Node>();
        closedSet = new HashSet<Node>();

        // Making sure GCost of the start node is 0. Doing this because before the algorithm starts or algorithm changes, we reset all the cost values of nodes to float max.
        // This is actually not part of algorithm, but doing this so that it works well according to my implementation.
        startNode.GCost = 0f;

        // A* uses additional heuristic called HCost, which is crucial to determine FCost. Making sure the HCost is calculated for start node before the FCost could be compared below with other nodes.
        // Doing this because before the algorithm starts or algorithm changes, we reset all the cost values of nodes to float max.
        // This is actually not part of algorithm, but doing this so that it works well according to my implementation.
        startNode.HCost = GetDistanceBetweenNodes(startNode, destinationNode);

        // Add startNode to openSet
        openSet.Add(startNode);

        // Iterate through the openSet to get values one by one until openSet is empty
        while (openSet.Count > 0) // [TIME COMPLEXITY: O(n)] where n is number of neighbourNode
        {
            // Assign startNode to currentNode for starting and also each time of iteration a new neighbour node becomes current node.
            Node currentNode = openSet[0];

            // Loop through the openSet to find node with lowest fCost.
            foreach (Node node in openSet) // [TIME COMPLEXITY: O(n) as this is a list] where n is number of neighbourNode
            {
                if (node == currentNode) continue; // This is to skip the 1st iteration because openSet[0] assigned to currentNode in above.

                if (node.FCost < currentNode.FCost || (node.FCost == currentNode.FCost && node.HCost < currentNode.HCost))
                {
                    currentNode = node;
                }
            }

            // Once a suitable currentNode is found with lowest fCost or maybe hCost we remove it from openSet as it is about to be explored.
            openSet.Remove(currentNode);

            // 
            closedSet.Add(currentNode);

            // If currentNode is the destinationNode, then retrace the path.
            if (currentNode == destinationNode)
            {
                pathUsingAStar = RetracePath(startNode, destinationNode);
                return pathUsingAStar;
            }

            // if not destinationNode, we now find out the listOfNeighbourNodes of the node.
            List<Node> listOfNeighbourNodes = customGridLayout.GetNeighbourNodes(currentNode);

            // Calculate the gCost for getting from currentNode to that neighbournode.
            foreach (Node neighbourNode in listOfNeighbourNodes)
            {
                // We ignore the obstacle covered nodes as they are non traversable OR (not and) the nodes that are already in closedSet (which means they have been already explored).
                if (!neighbourNode.IsTraversable || closedSet.Contains(neighbourNode)) continue; // FOUND THE SILLY MISTAKE I DID HERE... INSTEAD OF MAKING THIS A OR || CONDITION I PUT AND && CONDITION... SUCH A BLUNDER!!!!!

                float newMovementCostToNeighbour = currentNode.GCost + GetDistanceBetweenNodes(currentNode, neighbourNode);

                // If (the neighbour node is not explored yet) OR (the movement cost from current node to neightbour node is cheaper than neighbour node's GCost) --> Neighbour not explored yet takes the priority in this if condition.
                if (!openSet.Contains(neighbourNode) || newMovementCostToNeighbour < neighbourNode.GCost)
                {
                    // Assigning a new GCost as this is the shorter than it's old value (OR) giving it a value for the first time as it hasn't been added to openset yet, so that it can be chosen to be explored.
                    neighbourNode.GCost = newMovementCostToNeighbour;

                    // HCost is the distance between a node to destination node.
                    neighbourNode.HCost = GetDistanceBetweenNodes(neighbourNode, destinationNode);

                    // Setting a parent node for tracking back if this node makes it to the pathUsingAStar List.
                    neighbourNode.ParentNode = currentNode;

                    // if not in openSet add them, so that these listOfNeighbourNodes can be explored based on their FCost priority.
                    // Added in the end as any Cost calculations can be done before adding it in the list. But it doesn't make any difference as we are uisng references in C#.
                    if (!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);
                    }
                }
            }
        }

        return pathUsingAStar;
    }

    private List<Node> Dijkstra(Node startNode, Node destinationNode)
    {
        List<Node> pathNodeList = new List<Node>();
        openSet = new List<Node>();
        closedSet = new HashSet<Node>();

        // Making sure GCost of the start node is 0. Doing this because before the algorithm starts or algorithm changes, we reset all the cost values of nodes to float max.
        // This is actually not part of algorithm, but doing this so that it works well according to my implementation.
        startNode.GCost = 0f;

        // Add start node to openSet
        openSet.Add(startNode);

        // Looping until openSet has no nodes in it
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            foreach (Node n in openSet)
            {
                if (n == currentNode) continue; // Because I assigned 0 as current node above, so to not take into account for below condition.

                if (currentNode.GCost == n.GCost) continue; // Both GCost is same, so let's keep the 0th node in current node as is.
                else if (currentNode.GCost > n.GCost)
                {
                    currentNode = n;
                }
            }

            // Once a suitable currentNode is found with lowest gCost we remove it from openSet as it is about to be explored.
            openSet.Remove(currentNode);

            // 
            closedSet.Add(currentNode);

            // Before that checking if we found the path
            if (currentNode == destinationNode)
            {
                pathNodeList = RetracePath(startNode, destinationNode);
                return pathNodeList;
            }

            // If not found, find the neighbour nodes of current node and their new GCost
            List<Node> listOfNeighbourNodes = customGridLayout.GetNeighbourNodes(currentNode);

            // Calculate the gCost for getting from currentNode to that neighbournode.
            foreach (Node neighbourNode in listOfNeighbourNodes)
            {
                // We ignore the obstacle covered nodes as they are non traversable OR (not and) the nodes that are already in closedSet (which means they have been already explored).
                if (!neighbourNode.IsTraversable || closedSet.Contains(neighbourNode)) continue;

                float newMovementCostToNeighbour = currentNode.GCost + GetDistanceBetweenNodes(currentNode, neighbourNode);

                // If (the neighbour node is not explored yet) OR (the movement cost from current node to neightbour node is cheaper than neighbour node's GCost) --> Neighbour not explored yet takes the priority in this if condition.
                if (!openSet.Contains(neighbourNode) || newMovementCostToNeighbour < neighbourNode.GCost)
                {
                    // Assigning a new GCost as this is the shorter than it's old value (OR) giving it a value for the first time as it hasn't been added to openset yet, so that it can be chosen to be explored.
                    neighbourNode.GCost = newMovementCostToNeighbour;

                    // Setting a parent node for tracking back if this node makes it to the pathUsingAStar List.
                    neighbourNode.ParentNode = currentNode;

                    // if not in openSet add them, so that these listOfNeighbourNodes can be explored based on their FCost priority.
                    // Added in the end as any Cost calculations can be done before adding it in the list. But it doesn't make any difference as we are uisng references in C#.
                    if (!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);
                    }
                }
            }
        }

        return pathNodeList;
    }

    private List<Node> RetracePath(Node startNode, Node destinationNode)
    {
        Node n = destinationNode;
        List<Node> pathNodeList = new List<Node>();

        if(startNode == destinationNode)
        {
            pathNodeList.Add(startNode);
        }
        else
        {
            // Runs until it reaches startNode, and won't add startNode as it is the node where player is already on.
            while (startNode != n)
            {
                pathNodeList.Add(n);
                n = n.ParentNode;
            }
        }

        pathNodeList.Reverse();
        return pathNodeList;
    }
}
