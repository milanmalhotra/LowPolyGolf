using System;
using Cinemachine;
using UnityEngine;

public class BallLaunch : MonoBehaviour
{
    public Rigidbody golfBall;
    public CinemachineVirtualCamera golfShotCamera;
    public CinemachineVirtualCamera thirdPersonCamera;
    public CinemachineVirtualCamera golfBallCamera;
    public GameObject golfClub;
    public GameObject player;
    public Transform flag;
    public float angle = 45f;
    public float power;

    private Animator _golferAnimator;
    private float _maxIncrement = 30f;
    private float _incrementSpeed = 10f;
    private float _decreaseSpeed = 10f;
    private bool _isIncrementing = false;
    private bool _isDecrementing = false;
    private bool _isReadyToLaunch = false;
    private bool _inHittingPhase;
    private bool _hasEntered = true;
    
    void Update()
    {
        _inHittingPhase = GameVariables.getHittingPhase();
        if (_inHittingPhase) {
            if (_isReadyToLaunch) {
                InitiateLaunch();
                _isReadyToLaunch = false;
            }

            if (_isIncrementing)
            {
                IncrementPower();
            }
        }
        
        // Check if ball has stopped moving, then rotate to flag
        if (golfBall.velocity.magnitude <= 0.01f) { RotateToFlag(); }
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name == "Golfer" && _hasEntered)
        {
            
            GameVariables.setHittingPhase(true);
            _golferAnimator = other.gameObject.GetComponent<Animator>();
            
            _golferAnimator.SetLayerWeight(1, 0);
            _golferAnimator.SetBool("inSetup", true);
            CalculateWalkUpStartPos();
            golfShotCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
            golfClub.SetActive(true);
            _hasEntered = false;
        }
    }

    // Called whenever user clicks left mouse button
    public void OnLaunchBall()
    {
        if (_inHittingPhase)
        {
            if (_isIncrementing) {
                // Save the current value and stop incrementing
                _isIncrementing = false;
                _isReadyToLaunch = true;
            } else {
                // Reset values and start incrementing
                power = 0f;
                _isIncrementing = true;
            }
        }
    }

    void InitiateLaunch()
    {
        _golferAnimator.SetTrigger("doDrive");
        
        // Enable shot tracer and rb position
        transform.GetChild(0).gameObject.SetActive(true);
        golfBall.constraints = RigidbodyConstraints.None;
        // Convert angle from degrees to radians
        float radAngle = angle * Mathf.Deg2Rad; 
    
        // Calculate the shot direction based on angle
        Vector3 shotDirection = new Vector3(Mathf.Cos(radAngle), Mathf.Sin(radAngle), 0f);
    
        // Apply force to the golf ball based on direction and power
        golfBall.AddForce(shotDirection * power, ForceMode.Impulse);
        
        Invoke("EnableGolfBallCamera", 0.5f);
    }

    void IncrementPower()
    {
        // Increment the float value until it reaches the maximum
        if (power < _maxIncrement && !_isDecrementing) {
            power += _incrementSpeed * Time.deltaTime;
        } else {
            _isDecrementing = true;
            // If the value reaches the maximum, start decreasing
            power -= _decreaseSpeed * Time.deltaTime;
            if (power <= 0f) {
                // Stop incrementing once it returns to 0
                power = 0f;
                _isIncrementing = false;
            }
        }
    }

    void RotateToFlag()
    {
        Vector3 direction = flag.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
    }

    void EnableGolfBallCamera()
    {
        golfBallCamera.gameObject.SetActive(true);
        golfShotCamera.gameObject.SetActive(false);
    }

    // Method calculates where the player should be placed at start of animation based off of ball pos and rotation
    void CalculateWalkUpStartPos()
    {
        float xEndPosOffset = 0.142f;
        float zEndPosOffset = 0.713f;
        float xStartPosOffset = 1.46f;
        float zStartPosOffset = 1.03f;
        
        Vector3 playerEndPosition = new Vector3(transform.position.x - xEndPosOffset, transform.position.y, transform.position.z + zEndPosOffset);
        Vector3 playerStartPosition =
            new Vector3(playerEndPosition.x - xStartPosOffset, playerEndPosition.y, playerEndPosition.z - zStartPosOffset);
        
        Vector3 positionOffset = transform.position - playerEndPosition;
        Vector3 calculatedStartPosition = playerStartPosition - positionOffset;
        
        player.transform.position = calculatedStartPosition;
        player.transform.rotation = transform.rotation;
    }
}
