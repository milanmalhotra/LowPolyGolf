using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    [Range(1, 20)]
    public float sensitivity = 1f;
    public float rotationSpeed = 5f;

    public bool enableHittingPhase = false;
    private PlayerInput _playerInput;
    private static Animator _animator;
    private Vector3 _moveDirection;
    private Vector3 _startOfAnimPos;
    private Vector3 _endOfAnimPos;
    private Vector2 _movementInput;
    private InputAction _moveAction;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _playerInput = gameObject.GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        // When pressing WASD/left joystick set the Vector2 of the input (corresponds to which key/joystick direction) 
        _moveAction.performed += ctx => _movementInput = ctx.ReadValue<Vector2>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameVariables.setHittingPhase(enableHittingPhase);
    }
    
    void FixedUpdate()
    {
        bool inHittingPhase = GameVariables.getHittingPhase();
        if (!inHittingPhase)
        {
            Move();
        }
    }
    
    // Method handles with movement and animation of player
    // TODO: make controller not have to hold down left stick click, just on click sets bool to the opposite
    void Move() {
        Vector2 moveDirection = _playerInput.actions["Move"].ReadValue<Vector2>();
        bool isWalking = _playerInput.actions["Move"].IsInProgress();
        bool isSprinting = _playerInput.actions["Sprint"].IsInProgress();
        
        if (moveDirection != Vector2.zero)
        {
            RotatePlayer();
        }

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

    void RotatePlayer()
    {
        Vector3 direction = new Vector3(_movementInput.y, 0f, -_movementInput.x);

        // Get the current orbiting angle of the camera
        // float orbitingAngle = freeLookCamera.GetRigLatitudinalInputState().Input.Angle;

        // Rotate the direction based on the camera angle
        // direction = Quaternion.AngleAxis(orbitingAngle, Vector3.up) * direction;

        
        // Get the camera's forward and right vectors in world space
        Camera mainCamera = Camera.main;
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;
        
        // Transform the direction vector into camera space
        direction = new Vector3(
            Vector3.Dot(direction, cameraRight), 0f,
            Vector3.Dot(direction, cameraForward));
        
        if (direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public static void DisableCharacterAnimator()
    {
        _animator.SetLayerWeight(1, 0);
        _animator.SetBool("isWalking", false);
        _animator.SetBool("isSprinting", false);
    }

    public static void EnableCharacterAnimator()
    {
        _animator.SetLayerWeight(1, 1);
    }

    public static void EnableDriveAnimation()
    {
        _animator.SetTrigger("doDrive");
        _animator.SetBool("inSetup", false);
    }

    public static void EnableSetupAnimation()
    {
        DisableCharacterAnimator();
        _animator.SetBool("inSetup", true);
    }
}
