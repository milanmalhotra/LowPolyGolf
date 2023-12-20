using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Range(1, 20)]
    public float sensitivity = 1f;
    
    private PlayerInput _playerInput;
    private Animator _animator;
    private Vector3 _moveDirection;
    private float _groundCheckDistance = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _playerInput = gameObject.GetComponent<PlayerInput>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameVariables.setHittingPhase(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool inHittingPhase = GameVariables.getHittingPhase();
        if (!inHittingPhase)
        {
            Move();
            Rotate();
        }
    }

    /*
    * Method handles with movement and animation of player
    * TODO: make controller not have to hold down left stick click, just on click sets bool to the opposite
    */
    void Move() {
        bool isWalking = _playerInput.actions["Move"].IsInProgress();
        bool isSprinting = _playerInput.actions["Sprint"].IsInProgress();

        if (isWalking && !isSprinting) {
            _animator.SetBool("isWalking", true);
        } 

        if (!isWalking) {
            _animator.SetBool("isWalking", false);
        }

        if (isSprinting) {
            _animator.SetBool("isSprinting", true);
            _animator.SetBool("isWalking", false);
        }

        if (!isSprinting) {
            _animator.SetBool("isSprinting", false);
        }
    }

    // Method handles players rotation based off of mouse movements
    void Rotate() {
        Vector2 lookInput = _playerInput.actions["Look"].ReadValue<Vector2>();
        float mouseX = lookInput.x;

        transform.Rotate(Vector3.up * mouseX * sensitivity / 150);
    }
}
