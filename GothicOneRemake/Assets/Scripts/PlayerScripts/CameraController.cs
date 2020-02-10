using UnityEngine;

public class CameraController : MonoBehaviour {

    private Transform anchorTransform;
    private float min_Rotation, max_Rotation;

    private float sensitivity = 300f;

    private float rotationX = 0f;
    private float rotationY = 0f;
    
    private void Start() {
        anchorTransform = GetComponent<Transform>();    
    }


    // Player Inputs should always be collected during update because it is called once per frame
    private void Update() {
        Rotate();

    } // Update
        
    void Rotate() {
        // TODO: Enable Invert mouse controls
        // To look up and down the INVERT Input of the Y Axis from the mouse 
        // has to be taken and clamped by -90 to 90 degrees to ensure that 
        // the camera wont rotate too far.
        float mouseInputY = -(Input.GetAxis(MouseAxis.MOUSE_Y));
        float mouseInputX = Input.GetAxis(MouseAxis.MOUSE_X);

        mouseInputY = Mathf.Clamp(mouseInputY, -35f, 60f);

        rotationX += mouseInputY * sensitivity * Time.deltaTime;
        rotationY += mouseInputX * sensitivity * Time.deltaTime;

        anchorTransform.rotation = Quaternion.Euler(Mathf.Clamp(rotationX, -35f, 60f), rotationY, 0f);
    }
}