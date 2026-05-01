using UnityEngine;

public class NPCController : MonoBehaviour
{
    private CarController _carController;

    [Header("Car AI")]
    [SerializeField]
    private Circuit _circuit;
    /*
     * The higher the _steerSensitivity, the more the 
     * front wheels, 'wobble', trying to constantly 
     * correct the direction of travel.  If you
     * have the sideways friction set too high, you
     * will lose a LOT of speed because of this.
    */
    [SerializeField]
    private float _steerSensitivity = 0.01f;
    private float _braking = 0f;
    private float _acceleration = 1f;
    private ProgressTracker _progressTracker;

    [Header("Unstick")]
    private Rigidbody _rigidbody;
    private float _lastTimeUnstickChecked;
    [SerializeField]
    private float _stickDuration = 3.0f;

    void Start()
    {
        _circuit = FindFirstObjectByType<Circuit>();
        if (_circuit == null)
        {
            Debug.LogError("Circuit is Null!");
        }

        _carController = GetComponent<CarController>();
        if (_carController == null)
        {
            Debug.LogError("CarController is Null!");
        }

        _progressTracker = GetComponent<ProgressTracker>();
        if (_progressTracker == null)
        {
            Debug.LogError("ProgressTracker is Null!");
        }

        _rigidbody = _carController._rigidbody;
    }

    void Update()
    {
        if (!_carController._raceFinished)
        {
            Vector3 localTarget;
            float targetAngle;
            // Steering:

            localTarget = _carController._rigidbody.gameObject.transform.InverseTransformPoint(_progressTracker.GetTrackerPosition().transform.position);
            targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;


            // If car is in reverse, invert the target angle
            float steer = Mathf.Clamp(targetAngle * _steerSensitivity, -1f, 1f) * Mathf.Sign(_carController._currentRigidbodySpeed);

            if (RaceMonitor.racing == true)
            {
                // Check if car has velocity
                if (_carController._currentRigidbodySpeed > 1f)
                {
                    _lastTimeUnstickChecked = Time.time;
                }

                // Check if the car has been flipped or stuck for more than _stickDuration seconds
                if (Time.time > (_lastTimeUnstickChecked + _stickDuration))
                {
                    Unstick();
                }
            }
            else
            {
                _lastTimeUnstickChecked = Time.time;
            }

            _carController.SetDriverInput(_acceleration, _braking, steer);
        }
    }

    // This is called by the ReadWaypointInformation script when the car enters a waypoint collider.
    // It sets the braking and acceleration values for the car based on the waypoint information.
    public void SetBrakingAndAccleration(float braking, float accleration)
    {
        // If car is going slow, don't use brake and acceleration values from waypoints
        if (_carController._currentRigidbodySpeed > 15f)
        {
            _braking = braking;
            _acceleration = accleration;
        }
        else
        {
            _braking = 0f;
            _acceleration = 1f;
        }
    }

    // Move to last waypoint
    private void Unstick()
    {
        GameObject trackerPosition = _progressTracker.GetTrackerPosition();
        _rigidbody.transform.position = trackerPosition.transform.position + (Vector3.up * 2);
        _rigidbody.transform.rotation = trackerPosition.transform.rotation;
        // If car is set to brake with no acceleration by a waypoint, it will not move after being unstuck
        _braking = 0f;
        _acceleration = 1f;
    }
}
