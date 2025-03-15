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
    [SerializeField]
    private Vector3 cameraCurrentVelocity;

    #region To Write [TODO]
    // How to make the cameraTransform readonly when it is being sent as reference/object to other classes?
    // Answer is to think for C# solution and I found one called readonly struct.
    // In short, a readonly struct doesn't allow for modification to it's members except for one time when the members are initialised by the constructor of readonly struct.
    public readonly struct CameraTransformStruct
    {
        public CameraTransformStruct(Transform t) => CameraTransform = t;

        // Using Shorthand Syntax for varibale properties in C# automatically creates a private field implicitly.
        /* Sample Code Snippet
            Automatic Properties (Short Syntax)
            C# allows shorthand syntax for properties with implicit backing fields.

                            public string FirstName { get; set; } // Automatically creates a private field

            Equivalent to:
                            private string firstName;
                            public string FirstName
                            {
                                get { return firstName; }
                                set { firstName = value; }
                            }
         */
        public Transform CameraTransform { get; }
    }

    // Making the below two as static because I need to access the GetCameraTransformStruct() from CursorControl class without having to create an instance of CameraControl there.
    private static CameraTransformStruct cameraTransformStruct; // This should be private
    public static CameraTransformStruct GetCameraTransformStruct() => cameraTransformStruct; // returns immutable struct.
    #endregion

    #region To Write [TODO]
    //private Transform cameraTransform;
    //void SetCameraTransform(Transform c) { cameraTransform = c; }

    // C# doesn't have friend keyword like C++. Hence ChatGPT suggested few ways of programming in C# to achieve the same behaviour.
    // 1. Use internal Access Modifier - You can declare members as internal, which allows access to those members within the same assembly.
    /* Sample Code Snippet
        internal class MyClass
        {
            internal int SecretData = 42;
        }

        class FriendlyClass
        {
            void AccessSecret()
            {
                MyClass obj = new MyClass();
                Console.WriteLine(obj.SecretData); // Allowed since it's in the same assembly
            }
        }
     */
    // If you need to share internal members with another assembly, use the [InternalsVisibleTo] attribute.
    /* Sample Code Snippet
        // File: AssemblyA (MyLibrary.dll)
        using System.Runtime.CompilerServices;

        [assembly: InternalsVisibleTo("FriendAssembly")] // Grant access to FriendAssembly.dll

        internal class MyClass
        {
            internal int SecretData = 42;
        }

        // File: AssemblyB (FriendAssembly.dll)
        class FriendlyClass
        {
            void AccessSecret()
            {
                MyClass obj = new MyClass();
                Console.WriteLine(obj.SecretData); // Allowed because of InternalsVisibleTo
            }
        }
     */
    // Without [InternalsVisibleTo], FriendlyClass in a different assembly would not be able to access SecretData.
    // 2. Use Properties with Controlled Access - Instead of exposing private members directly, you can create properties with controlled access.
    /* Sample Code Snippet
        class MyClass
        {
            private int secretData = 42;

            public int GetSecretData(FriendlyClass friend)
            {
                return friend != null ? secretData : 0;
            }
        }

        class FriendlyClass
        {
            void AccessSecret()
            {
                MyClass obj = new MyClass();
                Console.WriteLine(obj.GetSecretData(this));
            }
        }
     */
    // 3. Use Nested Classes - A nested class can access private members of its containing class.
    /* Sample Code Snippet
        class OuterClass
        {
            private int secretData = 42;

            public class NestedFriend
            {
                public void AccessSecret(OuterClass obj)
                {
                    Console.WriteLine(obj.secretData);
                }
            }
        }
     */
    // I am going to use 2nd option as it fits my small personal experiment project.
    // Hence, the below GetCameraTransformStruct

    //public Transform GetCameraTransformStruct(CursorControl cc)
    //{
    //    if (cc.GetType() == typeof(CursorControl) && cameraTransform != null)
    //    {
    //        return cameraTransform;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
    #endregion

    void Awake()
    {
        cameraTransformStruct = new CameraTransformStruct(GetComponent<Transform>());

        // Directly using transform as this script is assigned to Main Camera
        transform.eulerAngles = Vector3.zero;
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
        transform.eulerAngles = Vector3.SmoothDamp(transform.rotation.eulerAngles, cameraNewRotationValue, ref cameraCurrentVelocity, cameraRotationSmoothness * Time.deltaTime);
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
        transform.position = Vector3.Lerp(transform.position, transform.position + cameraNewMovementValue, cameraMovementSmoothness * Time.deltaTime);
    }
}
