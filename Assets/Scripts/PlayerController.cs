using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    public LayerMask groundLayer;
    [Range(1, 20)]
    public float sensitivity = 1f;

    private PlayerInput playerInput;
    private Animator animator;
    private GameObject test;
    private Vector3 _moveDirection;
    private float _groundCheckDistance = 1.5f;
    
    

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    /*
    * Method handles with movement and animation of player
    * TODO: make controller not have to hold down left stick click, just on click sets bool to the opposite
    */
    void Move() {
        bool isWalking = playerInput.actions["Move"].IsInProgress();
        bool isSprinting = playerInput.actions["Sprint"].IsInProgress();

        if (isWalking && !isSprinting) {
            animator.SetBool("isWalking", true);
        } 

        if (!isWalking) {
            animator.SetBool("isWalking", false);
        }

        if (isSprinting) {
            animator.SetBool("isSprinting", true);
            animator.SetBool("isWalking", false);
        }

        if (!isSprinting) {
            animator.SetBool("isSprinting", false);
        }
    }

    // Method handles players rotation based off of mouse movements
    void Rotate() {
        Vector2 lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
        float mouseX = lookInput.x;

        transform.Rotate(Vector3.up * mouseX * sensitivity / 150);
    }
}
