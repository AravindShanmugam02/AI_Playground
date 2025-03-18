// A node in a world is just a point where something is represented. Now since we are creating a gridLayout based world for AI movement, let us make the grids consists of nodes.
// What does a node have as properties?
// - Position in the world (X,Y,Z) and if 2D it is just (X,Y). [if you think about a gridLayout representing a world position, then it is 2D]
// - A bool representing whether the node is obstacleLayer. [Is the node obstacleLayer? Can AI use the node for moving to any other node or can it move to that node itself?]

using UnityEngine;
using System.Collections;

public class Node
{
    // - A bool representing whether the node is obstacleLayer. [Is the node obstacleLayer? Can AI use the node for moving to any other node or can it move to that node itself?]
    private bool isTraversable;
    // - Position in the world (X,Y,Z) and if 2D it is just (X,Y). [if you think about a gridLayout representing a world position, then it is 2D]
    private Vector3 posInWorld;
    // - A Vecto2Int representing node coords in the 2D array
    private Vector2Int nodeCoordsIn2DArray;

    // Pathfinding Related
    // - A Node to hold the parent node value - useful when pathfinding
    private Node parentNode;
    // - A float to store the gCost -> gCost is the distance/cost from current node to neighbour node. In Graphs, we can say this as the weight of an edge.
    private float gCost;
    // - A float to store the hCost -> hCost is the distance between the neighbour node to the target node. It is used in A*.
    private float hCost;

    public Node(bool _isTraversable, Vector3 _posInWorld, Vector2Int _nodeCoordsIn2DArray)
    {
        IsTraversable = _isTraversable;
        PosInWorld = _posInWorld;
        NodeCoordsIn2DArray = _nodeCoordsIn2DArray;
    }

    public bool IsTraversable { get { return isTraversable; } set { isTraversable = value; } }
    public Vector3 PosInWorld { get { return posInWorld; } set { posInWorld = value; } }
    public Vector2Int NodeCoordsIn2DArray { get { return nodeCoordsIn2DArray; } set { nodeCoordsIn2DArray = value; } }
    public Node ParentNode { get { return parentNode; } set { parentNode = value; } }
    public float GCost { get { return gCost; } set { gCost = value; } }
    public float HCost { get { return hCost; } set { hCost = value; } }

    // fCost is the sum of gCost and hCost of a node. It is the factor which decides which node to become the next current node. It is because the smaller the fCost, the efficient the path is. Again, used in A*.
    public float FCost { get { return gCost + hCost; } }
}
