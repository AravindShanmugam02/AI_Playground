using UnityEngine;

public class CursorControl : MonoBehaviour
{
    [Header("Ground Properties")]
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private CustomGridLayout customGridLayout;

    [Header("Cursor properties")]
    [SerializeField]
    bool isCursorVisible;

    [Header("Grid Debug")]
    [SerializeField]
    Vector2Int nodeCoords;
    [SerializeField]
    Vector3 hitWorldPoint;

    [Header("Pathfinding Debug")]
    Node startNode;
    Node destinationNode;
    [SerializeField]
    Vector2Int startNodeCoords;
    [SerializeField]
    Vector2Int destinationNodeCoords;
    [SerializeField]
    bool canPathFindingBeTriggered;

    public bool IsCursorVisible { get { return isCursorVisible; } }

    void Awake()
    {
        customGridLayout = FindAnyObjectByType<CustomGridLayout>().GetComponent<CustomGridLayout>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCursorVisible = false;
        canPathFindingBeTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        CursorInput();

        if (CanPathFindingBeTriggered())
        {
            customGridLayout.TriggerPathfinding(startNode, destinationNode);
        }
    }

    void CursorInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCursorVisible = !isCursorVisible;
            Cursor.visible = isCursorVisible;
        }

        if(isCursorVisible && Cursor.lockState == CursorLockMode.Confined)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (startNode == null)
                {
                    startNode = GetTheNodePointedByCursor();
                    if (startNode != null) startNodeCoords = startNode.NodeCoordsIn2DArray;
                }
                else
                {
                    startNode = null;
                    startNodeCoords = Vector2Int.zero;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (destinationNode == null)
                {
                    destinationNode = GetTheNodePointedByCursor();
                    if (destinationNode != null) destinationNodeCoords = destinationNode.NodeCoordsIn2DArray;
                }
                else
                {
                    destinationNode = null;
                    destinationNodeCoords = Vector2Int.zero;
                }
            }
        }
    }

    bool CanPathFindingBeTriggered()
    {
        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.Confined;

            if (startNode != null && destinationNode != null)
            {
                if (!canPathFindingBeTriggered)
                {
                    canPathFindingBeTriggered = true;
                }
            }
            else
            {
                canPathFindingBeTriggered = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        return canPathFindingBeTriggered;
    }

    void FixedUpdate()
    {

    }

    Node GetTheNodePointedByCursor()
    {
        Vector3 hitPointV3 = GetWorldPositionThroughRayCastFromMousePointerToWorld();
        Node n = customGridLayout.GetNodeFromWorldPosition(hitPointV3);
        if(n != null)
        {
            nodeCoords = new Vector2Int(n.NodeCoordsIn2DArray.x, n.NodeCoordsIn2DArray.y);
        }
        else
        {
            nodeCoords = Vector2Int.zero;
        }
        return n;
    }

    Vector3 GetWorldPositionThroughRayCastFromMousePointerToWorld()
    {
        // Checking CameraTransformStruct readonly struct can be modified
        // CameraControl.GetCameraTransformStruct().CameraTransform = transform; --> This gives error as I can't chnage the value of the readonly struct's member.
        // Now, checking can I change properties of the member of readonly struct
        // CameraControl.GetCameraTransformStruct().CameraTransform.position = transform.position; --> Can change the properties of the readonly struct's member.
        // So, that is not what we want to prevent. Hence, instead of getting the transform itself, I should only get the forward direction of the Main Camera.
        // Using readonly struct isn't suitable for getting the forward direction value alone because of the nature of readonly struct.
        // Hence, used a normal basic getter and setter in CameraControl class.

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // new Ray(CameraControl.GetCameraTransformForward(), Input.mousePosition) -- Isn't working
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        hitWorldPoint = hit.point;
        return hitWorldPoint;
    }
}
