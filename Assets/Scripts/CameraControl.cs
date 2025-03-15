using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Properties")]
    public float cameraMovementSpeed;
    public float cameraMovementSmoothness;
    public float cameraRotationSpeed;
    public float cameraRotationSmoothness;

    [SerializeField]
    private Vector3 cameraNewMovementValue;
    [SerializeField]
    private Vector3 cameraNewRotationValue;
    [SerializeField]
    private float rotationAxisY, rotationAxisX, mouseX, mouseY;
    private Vector3 cameraCurrentVelocity;
    private Transform cameraTransform;

    void Awake()
    {
        cameraTransform = GetComponent<Transform>();
        cameraTransform.eulerAngles = Vector3.zero;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraMovementSpeed = 5.0f;
        cameraMovementSmoothness = 1.0f;
        cameraRotationSpeed = 5.0f;
        cameraRotationSmoothness = 1.0f;
        rotationAxisY = 0.0f;
        rotationAxisX = 0.0f;
        cameraNewMovementValue = Vector3.zero;
        cameraNewRotationValue = Vector3.zero;

        // Setting cursor visible
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotation();
        CameraMovement();
    }

    // I am using EulerAngles to calculate the rotation. Maybe that is why there happens weird uncontrollable rotations at certain values.
    // [TODO] Maybe should upgrade this to using Quaternions. But, main goal is got to understand completely both methods inner calculations.
    void CameraRotation()
    {
        {
            cameraNewRotationValue = Vector3.zero;
            cameraRotationSpeed = 5.0f;
        }

        // Rotation Input
        mouseX = Input.GetAxisRaw("Mouse X") * cameraRotationSpeed; // [TODO] To know more about the values we get from GetAxisRaw and GetAxis
        mouseY = Input.GetAxisRaw("Mouse Y") * cameraRotationSpeed;

        rotationAxisY += mouseX;
        rotationAxisX -= mouseY; // Inversing this with -= because I want the camera to look up when I move my mouse in up direction.

        // Clamping is done because I am using EulerAngles and not to make any weird random continuos rotation.
        // Clamping X axis rotation, which contributes to up-down rotation.
        rotationAxisX = Mathf.Clamp(rotationAxisX, 0.0f, 90.0f);
        // Clamping Y axis rotation, which contributes to right-left rotation.
        rotationAxisY = Mathf.Clamp(rotationAxisY, 0.0f, 359.0f);

        // Rotation Calculation -> gives new rotation
        cameraNewRotationValue = new Vector3(rotationAxisX, rotationAxisY, 0.0f);

        // Rotating Camera --> Both Lerp and SmoothDamp works. Even Slerp Works. It is just the way they smooth the movement or rotation differs.
        cameraTransform.eulerAngles = Vector3.SmoothDamp(cameraTransform.rotation.eulerAngles, cameraNewRotationValue, ref cameraCurrentVelocity, cameraRotationSmoothness * Time.deltaTime);
    }

    void CameraMovement()
    {
        {
            cameraNewMovementValue = Vector3.zero;
            cameraMovementSpeed = 5.0f;
        }

        // Controls Movement Speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraMovementSpeed *= 3.0f;
        }

        // Movement Calculation
        if (Input.GetKey(KeyCode.W))
        {
            cameraNewMovementValue += (transform.forward) * cameraMovementSpeed;
        }
        
        if(Input.GetKey(KeyCode.S))
        {
            cameraNewMovementValue += -(transform.forward) * cameraMovementSpeed;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            cameraNewMovementValue += (transform.right) * cameraMovementSpeed;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            cameraNewMovementValue += -(transform.right) * cameraMovementSpeed;
        }
        
        if (Input.GetKey(KeyCode.E))
        {
            cameraNewMovementValue += (transform.up) * cameraMovementSpeed;
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            cameraNewMovementValue += -(transform.up) * cameraMovementSpeed;
        }

        // Moving Camera
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTransform.position + cameraNewMovementValue, cameraMovementSmoothness * Time.deltaTime);
    }
}
