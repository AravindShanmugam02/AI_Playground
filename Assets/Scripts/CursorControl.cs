using UnityEngine;

public class CursorControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetTheNodePointedByCursor()
    {

    }

    void RayCastFromMousePointerPositionToWorld()
    {
        Ray ray = new Ray(CameraControl.GetCameraTransformStruct().CameraTransform.forward, Input.mousePosition);

        // Checking CameraTransformStruct readonly struct can be modified
        // CameraControl.GetCameraTransformStruct().CameraTransform = transform; --> This gives error as I can't chnage the value of the readonly struct's member.
        // Now, checking can I change properties of the member of readonly struct
        // CameraControl.GetCameraTransformStruct().CameraTransform.position = transform.position; --> Can change the properties of the readonly struct's member.
        // So, that is not what we want to prevent. Hence, instead of getting the transform itself, I should only get the forward direction of the Main Camera.

    }
}
