using UnityEngine;

public class MovingPlayer : MonoBehaviour {

    private Animator animator;
    private Transform playerTransform;
    private CharacterController characterController;

    private float sensitivity = 300f;
    private float rotationY = 0f;
    private float rotationX = 0f;

    private void Start() {
        // Reference the attached Animator
        // animator = GetComponent<Animator>();

        // Reference the Transform
        playerTransform = GetComponent<Transform>();

        // Reference the CharacterController
        characterController = GetComponent<CharacterController>();
    }

    private void Update() {
        MovePlayer();
        RotatePlayer();
        Jump();
    }

    void MovePlayer() {
        // animator.SetFloat("verticalInput", Input.GetAxis(Axis.VERTICAL));
        // animator.SetFloat("horizontalInput", Input.GetAxis(Axis.HORIZONTAL));

        // Get the vertical input
        Input.GetAxis(Axis.VERTICAL);
    }

    void RotatePlayer() {
        // To rotate around the Y axis of the game, the movement 
        // along the X axis of the mouse hast to be taken
        float mouseInputX = Input.GetAxis(MouseAxis.MOUSE_X);

        // Multiply by Time.deltaTime to make the rotation independend
        // of frame rate.
        rotationY += mouseInputX * sensitivity * Time.deltaTime;

        playerTransform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space))
            characterController.Move(new Vector3(0f, 300f * Time.deltaTime, 0f));
    }
}