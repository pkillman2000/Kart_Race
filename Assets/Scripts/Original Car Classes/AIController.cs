using UnityEngine;

public class AIController : MonoBehaviour
{
    private CarController _carController;

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

    private GameObject _tracker;
    private int _currentTrackerWaypointIndex = 0;
    [SerializeField]
    private float _lookAheadDistance = 5f;

    void Start()
    {
        _carController = GetComponent<CarController>();
        if (_carController == null)
        {
            Debug.LogError("CarController is Null!");
        }


        // Create and position the tracker object
        _tracker = new GameObject("Tracker");
        _tracker.transform.position = _carController._rigidbody.gameObject.transform.position;
        _tracker.transform.rotation = _carController._rigidbody.gameObject.transform.rotation;
    }

    void Update()
    {
        Vector3 localTarget;

        float targetAngle;

        // Avoidance - Based on AvoidDetector class
        if (Time.time < _carController._rigidbody.GetComponent<AvoidDetector>()._avoidTime)
        {
            // Aim right of tracker to avoid car
            localTarget = _tracker.transform.right * _carController._rigidbody.GetComponent<AvoidDetector>()._avoidPath;
        }
        else // Not avoiding
        {
            // Put target and car on same plane
            localTarget = _carController._rigidbody.gameObject.transform.InverseTransformPoint(_tracker.transform.position);
        }

        // Steering
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        // If car is in reverse, invert the target angle
        float steer = Mathf.Clamp(targetAngle * _steerSensitivity, -1f, 1f) * Mathf.Sign(_carController._currentRigidbodySpeed);

        _carController.SetDriverInput(_acceleration, _braking, steer);

        ProgressTracker();
    }

    private void ProgressTracker()
    {
        if (Vector3.Distance(_carController._rigidbody.gameObject.transform.position, _tracker.transform.position) < _lookAheadDistance)
        {
            _tracker.transform.LookAt(_circuit._waypoints[_currentTrackerWaypointIndex].transform.position);
            _tracker.transform.Translate(Vector3.forward);

            if (Vector3.Distance(_tracker.transform.position, _circuit._waypoints[_currentTrackerWaypointIndex].transform.position) < 1)
            {
                _currentTrackerWaypointIndex = _circuit.GetNextWaypointIndex(_currentTrackerWaypointIndex);
            }
        }
    }

    public GameObject GetTrackerPosition()
    {
        return _tracker.gameObject;
    }

    public void SetBrakingAndAccleration(float braking, float accleration)
    {
        _braking = braking;
        _acceleration = accleration;
    }
}
