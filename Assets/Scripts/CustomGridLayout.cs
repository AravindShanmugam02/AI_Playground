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
    [SerializeField]
    private MeshCollider groundMeshCollider;

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

    // [DEBUG PURPOSE]
    [SerializeField]
    Vector3 topLeftCornerPosition;

    [Header("Node Properties")]
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

    [Header("Reference Variables")] // Just to use it for condition checking
    // LayerMask to store the Obstacle layer from inspector
    [SerializeField]
    private LayerMask obstacleLayer;

    void Awake()
    {
        // Get Plane transform
        groundTransform = GetComponent<Transform>();
        // Get Plane Mesh Collider
        groundMeshCollider = groundTransform?.GetComponent<MeshCollider>();

        // Giving initial size value for the gridLayout. Instead of scale size, I am using meshcollider.size to get the actual size of the scaled plane ground, and assign that as the gridLayout size.
        // Since, this is a mesh collider and not any primitive collider, I had the option to use meshcollider.size directly, which in turn represents the actual size of the scaled plane primitive.
        gridLayoutSizeXZ.x = groundMeshCollider.bounds.size.x;
        gridLayoutSizeXZ.y = groundMeshCollider.bounds.size.z;

        // Giving an initial value for grid height
        gridLayoutSizeY = 0.15f;

        // Giving an initial diameter for nodes by multiplying radius with 2. And Radus is 0.5f by default.
        nodeDiameter = nodeRadius * 2;

        // Dividing by a single node size (here, diameter) would give us the number of nodes that can fit into the grid size in that respective axis.
        // Rounding up the float gridLayout size value to the nearest Int value.
        noOfNodesInXAxis = Mathf.RoundToInt(gridLayoutSizeXZ.x/nodeDiameter);
        noOfNodesInZAxis = Mathf.RoundToInt(gridLayoutSizeXZ.y/nodeDiameter); // Note: Since this is a Vector2 I'm doing gridLayoutSizeXZ.y
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialise the gridLayout 2D array with the number of nodes that can fit-in in each axis.
        gridLayout = new Node[noOfNodesInXAxis, noOfNodesInZAxis];

        // Now we are going to initialise the nodes and place them in the gridLayout 2D array.
        // Q.We need to know the position of the nodes. So, how do we know each node's position?
        // A.Maybe try dividing the whole plane into multiple segments until we reach the any corner of the grid. From there we can calculate each node's position.
        // Lets, choose topleft corner. In tutorial video they choose bottom left.

        // A grid's left most point's position = centre of plane - (radius of plane converted to Vector3 using Vector3.right since it is a radius in x axis);
        Vector3 positionOfLeftMostPointOfGrid = groundTransform.position - (Vector3.right * gridLayoutSizeXZ.x / 2);

        // left most point position + radius of plane in z axis converted to Vector3 using Vector3.forward. Vector3.forward since it is a radius along z axis.
        topLeftCornerPosition = positionOfLeftMostPointOfGrid + (Vector3.forward * gridLayoutSizeXZ.y / 2); // + because z axis upwards is positve and downwards is negative.

        // In my map, topLeftCornerPosition would be the position of [0,0] node.

        // rows
        for (int x = 0; x < noOfNodesInXAxis; x++)
        {
            // coloumns
            for (int z = 0; z < noOfNodesInZAxis; z++)
            {
                // To create a node we need to know the centre position of the node. As of now we have the position of top left most point of the grid. So, every node has a radius and diameter.
                // To go to the next from from previous node through position, just use diameter.
                // To get the centre of the node use radius.

                // Since I am starting from lopleft corner, I got to subtract nodeRadius from topLeftCornerPosition.x
                //Vector3 centrePointOfNode = new Vector3(topLeftCornerPosition.x - nodeRadius, topLeftCornerPosition.y, topLeftCornerPosition.z + nodeRadius);
                // But this above stupid equation only works for [0,0] node. How do I make it useable for all the nodes upto [n,m]? Solution: below equation.
                Vector3 centrePointOfNode = topLeftCornerPosition + Vector3.right * (x * nodeDiameter + nodeRadius) - Vector3.forward * (z * nodeDiameter + nodeRadius);

                // To create a node we also need to check if the node is obstacleLayer, where there is no objects.
                // We could use the same logic we did for farming lands in Monocrop Madness by checking sphere collision with the size of radius from the centre of the node.
                // Here we are using additional layermask to only check collision against those objects with Obstacle Layer mask.
                // If Obstacle object is found in sphere collision check, that node is untraversable.
                bool isTraversable = !(Physics.CheckSphere(centrePointOfNode, nodeRadius, obstacleLayer));

                // Creating Node
                gridLayout[x, z] = new Node(isTraversable, centrePointOfNode);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Will make sure to draw only when this object is in the play mode.
        if(Application.IsPlaying(this))
        {
            #region REALISATION OF I DID A MISTAKE IN CALCULATING THE SIZE OF THE GIZMO WIRED CUBE
            // I was making a mistake here by thinking scale is the size of an object.
            // But actually size/dimension is different from scale.
            // When you scale an object, it is just scale times the actual size.
            // Example a dimension of a primitive object is V3(2,5,6) and I scale it from Scale V3(1,1,1) to Scale V3(2,2,2) and what happens is the Scaled value of V3(2,2,2)
            // is multiplied with dimention V3(2,5,6) and the dimension becomes new dimension V3(4,10,12). But what we see in the inspector is Scaled value of V3(2,2,2) and it should not be confused with the size of the object.
            // So, when we try to match the size of an object with the scaled value of another object, it will be wrong. That is why, randomly I tried dividing gridLayoutSizeXZ by 2,3,4, and then 2.5f and
            // it worked and the gizmo wired cube became the same size as the scaled plane.

            // https://discussions.unity.com/t/understanding-dimension-size-vs-scale/813678/2
            // Unity uses “Unity units” for distance. Though 1 unit = 1 meter is common, and a few settings which depend on that are set to it as default, you’re in no way required to keep to 1 unit = 1 meter.
            // But on the actual question, you’re thinking about this wrong.Scale doesn’t tell you how large a model’s size is.It is a multiple of that size(on each specific dimension individually),
            // regardless of what it actually is.So you have a model that is 10x7x27 and the scale is set to 1,1,1, it will actually appear as 10x7x27 in the scene. You change the scale to 2,1,2 and
            // it will appear as 20x7x54 in the scene.
            // The cube primitive just happens to have dimensions of 1x1x1, so coincidentally whatever multiple you set for scale is the actual size in units(1 x anything == anything),
            // but that is only the case for the cube.

            // https://discussions.unity.com/t/setting-the-actual-size-of-an-object/894991/2
            // Scale is how large an object is compared its ‘normal’ size. A scale of 1, 1, 1, means an object is true its normal size. But its true size can be anything.
            // It can be a cube with a volume of 1m3 in real terms, or it can be a flat plane with an area of 50km2.
            // If you want objects to be a consistent size, you need to make sure that they are. Most 3d programs like Blender will be able to tell you an objects dimensions regardless of its scale.
            // You also need to make sure they’re exported properly too. This generally means making sure they’re exported with a scale of 1, 1, 1, and
            // usually with zero rotation and zero position too. This isn’t a feature of Unity as it’s somewhat outside its domain.
            // Typically these built in primitives are only really used for some quick and dirty prototyping.You rarely use them otherwise, so don’t get hung up on them.
            // Typically you create your models in an external modeling program and import them into Unity.If they were scaled incorrectly in the external program,
            // you can then adjust their scale in Unity with the scale setting you are already playing with.But since scale also stretches their textures, you usually want to keep
            // to a uniform scale when possible instead of using scale to stretch them into different shapes.

            // Here, they use collider's radius. Because collider stays the same size as the object even it becomes big or small.
            // The collider too gets that size. Hence I should also be using collider rather than Scale value of the plane.

            // https://discussions.unity.com/t/keep-gizmos-the-same-with-the-object-scale/173367
            // Replace Gizmos.DrawSphere(sphereCollider.bounds.center, sphereCollider.radius) with Gizmos.DrawSphere(sphereCollider.bounds.center, sphereCollider.radius * transform.lossyScale.x)
            #endregion

            // Draw the grid layout using wire cube
            Gizmos.DrawWireCube(groundTransform.position, new Vector3(gridLayoutSizeXZ.x, gridLayoutSizeY, gridLayoutSizeXZ.y));

            // Draw the nodes within the grid layout
            if(gridLayout != null)
            {
                foreach(Node n in gridLayout)
                {
                    if (!n.IsTraversable) Gizmos.color = Color.red;
                    else Gizmos.color = Color.green;
                    Gizmos.DrawCube(n.PosInWorld, Vector3.one * (nodeDiameter - 0.2f)); // - 0.2 to make the grids visible and clear by being seperate from each other.
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
