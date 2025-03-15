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
    }
}
