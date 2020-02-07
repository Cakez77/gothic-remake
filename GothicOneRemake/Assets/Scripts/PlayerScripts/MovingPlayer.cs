using UnityEngine;

public class MovingPlayer : MonoBehaviour {
    
    private Animator animator;

    private void Start() {
        // Reference the attached Animator
        animator = GetComponent<Animator>();
    }

    private void Update() {
        PlayerMovement();
    }

    void PlayerMovement() {
        animator.SetFloat("verticalInput", Input.GetAxis(Axis.VERTICAL));
    }
}