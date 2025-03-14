using UnityEngine;

// This class is going to layout a gridLayout on the plane. This is a basic technique of dividing the map into grids.
// Remember that the gridLayout is going to be layed out horizontally in 2D on top of the plane ground.
// Which means it is referred in (X,Z) coords as the world is actually 3D (X,Y,Z) coords. So, Y remains as the height.

public class CustomGridLayout : MonoBehaviour
{
    // Members for getting grounds's components
    [Header("Ground Properties")]
    [SerializeField]
    private Transform groundTransform;

    // What poperties does a gridLayout have in general?
    // - CustomGridLayout Size in length and breadth. Should be (X,Z) in 3D world [Total size of the gridLayout in the world.]
    // - Height of the gridLayout. Should be (Y) in 3D world. [For better visualisation]
    // - A container to hold all the generated number of nodes that can fit inside the gridLayout layout. Maybe a 2D array because the gridLayout will primarilly be targetting only 2D (X,Z) of the plane for navigation?
    // - How many nodes will I be able to fit in to the gridLayoutSizeXZ sized CustomGridLayout?
    // - For that we need to know the size of one node:
    //      - Size can be measured in 2 dimensions for the node as we don't give importance to height at the moment.
    //      - So, the measurements we want are diameter and radius.
    //      - float variable to hold the diameter of a node. [All nodes are going to the same size]
    //      - float variable to hold the radius of a node. [Required for calculating the postion of node. The position should be the center point]

    [Header("CustomGridLayout Properties")]
    // CustomGridLayout length and breadth
    [SerializeField]
    private Vector2 gridLayoutSizeXZ;

    // Keeping it as a Range as we are going to adjust it dynamically for visualisation
    [Range(0.0f, 1.0f)]
    [SerializeField]
    public float gridLayoutSizeY;

    // To hold all the nodes generated based on how many nodes can fit in to the current gridLayout layout size
    Node[,] gridLayout;

    // Keeping node radius to be adjusted dynamically for movement accuracy
    [Range(0.5f, 3f)]
    [SerializeField]
    public float nodeRadius;
    // Diameter would be just radius*2
    [SerializeField]
    float nodeDiameter;

    // A couple of holders for number of nodes in each axis that would be rounded up to be used for gridLayout size when initialising.
    [SerializeField]
    int noOfNodesInXAxis;
    [SerializeField]
    int noOfNodesInZAxis;

    void Awake()
    {
        // Get Plane transform
        groundTransform = GetComponent<Transform>();

        // Assign the scale size of the plane to the gridLayoutSizeXZ Vector2 variable, so that it is in the same size as the plane.
        gridLayoutSizeXZ.x = groundTransform.localScale.x;
        gridLayoutSizeXZ.y = groundTransform.localScale.z;

        // Giving an initial value for grid height
        gridLayoutSizeY = 0.15f;

        // Giving an initial diameter for nodes by multiplying radius with 2. And Radus is 0.5f by default.
        nodeDiameter = nodeRadius * 2;

        // Rounding up the float gridLayout size value to the nearest Int value.
        noOfNodesInXAxis = Mathf.RoundToInt(gridLayoutSizeXZ.x/nodeDiameter); // Dividing by a single node size (here, diameter) would give us the number of nodes that can fit into the grid size in that respective axis.
        noOfNodesInZAxis = Mathf.RoundToInt(gridLayoutSizeXZ.y/nodeDiameter); // Note: Since this is a Vector2 I'm doing gridLayoutSizeXZ.y
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialise the gridLayout 2D array with the number of nodes that can fit-in in each axis.
        gridLayout = new Node[noOfNodesInXAxis, noOfNodesInZAxis];
    }

    void OnDrawGizmos()
    {
        if(Application.IsPlaying(this))
        {
            Gizmos.DrawWireCube(groundTransform.position, new Vector3((gridLayoutSizeXZ.x / 2.5f) * groundTransform.lossyScale.x, gridLayoutSizeY, (gridLayoutSizeXZ.y / 2.5f) * groundTransform.lossyScale.z)); // Dunno why dividing by 2.5f and multiplaying by lossyScale gives the correct size as the ground plane.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
