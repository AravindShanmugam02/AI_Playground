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

    public Node(bool _isTraversable, Vector3 _posInWorld, Vector2Int _nodeCoordsIn2DArray)
    {
        IsTraversable = _isTraversable;
        PosInWorld = _posInWorld;
        NodeCoordsIn2DArray = _nodeCoordsIn2DArray;
    }

    public bool IsTraversable { get { return isTraversable; } set { isTraversable = value; } }
    public Vector3 PosInWorld { get { return posInWorld; } set { posInWorld = value; } }
    public Vector2Int NodeCoordsIn2DArray { get { return nodeCoordsIn2DArray; } set { nodeCoordsIn2DArray = value; } }
}
