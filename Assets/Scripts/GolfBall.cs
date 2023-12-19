using Unity.Collections;
using UnityEngine;

public class BallLaunch : MonoBehaviour
{
    public Rigidbody golfBall;
    // These will be determined by player
    [SerializeField]
    private float power;
    public float angle = 45f;
    public bool inHittingPhase = true;
    
    private float _maxIncrement = 30f;
    private float _incrementSpeed = 10f;
    private float _decreaseSpeed = 10f;
    private bool _isIncrementing = false;
    private bool _isDecrementing = false;
    private bool _isReadyToLaunch = false;

    void Update()
    {
        if (inHittingPhase) {
            if (_isReadyToLaunch) {
                InitiateLaunch();
                _isReadyToLaunch = false;
            }

            if (_isIncrementing) {
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
        }
    }

    // Called whenever user clicks left mouse button
    public void OnLaunchBall()
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

    void InitiateLaunch()
    {
        // Convert angle from degrees to radians
        float radAngle = angle * Mathf.Deg2Rad; 
    
        // Calculate the shot direction based on angle
        Vector3 shotDirection = new Vector3(Mathf.Cos(radAngle), Mathf.Sin(radAngle), 0f);
    
        // Apply force to the golf ball based on direction and power
        golfBall.AddForce(shotDirection * power, ForceMode.Impulse);
    }
}
