using UnityEngine;

public class AIController : MonoBehaviour
{
    private Drive _drive;
    private bool[] _hasTorque;
    private WheelCollider[] _wheelCollider;
    private bool _antiSkid = true;

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

    Vector3 _target;
    int _currentWaypointIndex = 0;

    private GameObject _tracker;
    private int _currentTrackerWaypointIndex = 0;
    [SerializeField]
    private float _lookAheadDistance = 5f;

    void Start()
    {
        _drive = GetComponent<Drive>();
        if (_drive == null )
        {
            Debug.LogError("Drive is Null!");
        }
        else
        {
            _wheelCollider = new WheelCollider[_drive._wheelCollider.Length];
            for (int i = 0; i < _drive._wheelCollider.Length; i++)
            {
                if (_drive._wheelCollider[i] != null)
                {
                    _wheelCollider[i] = _drive._wheelCollider[i];
                }
            }

            _hasTorque = new bool[_drive._hasTorque.Length];
            for (int i = 0; i < _drive._hasTorque.Length; i++)
            {
                _hasTorque[i] = _drive._hasTorque[i];
            }
        }

        _target = _circuit._waypoints[_currentWaypointIndex].transform.position;

        // Create and position the tracker object
        _tracker = new GameObject("Tracker");
        //DestroyImmediate(_tracker.GetComponent<Collider>());
        _tracker.transform.position = _drive._rigidbody.gameObject.transform.position;
        _tracker.transform.rotation = _drive._rigidbody.gameObject.transform.rotation;
    }

    void Update()
    {
        Vector3 localTarget;

        float targetAngle;

        // Avoidance - Based on AvoidDetector class
        if (Time.time < _drive._rigidbody.GetComponent<AvoidDetector>()._avoidTime)
        {
            // Aim right of tracker to avoid car
            localTarget = _tracker.transform.right * _drive._rigidbody.GetComponent<AvoidDetector>()._avoidPath;
        }
        else // Not avoiding
        {
            // Put target and car on same plane
            localTarget = _drive._rigidbody.gameObject.transform.InverseTransformPoint(_tracker.transform.position);
        }

        // Steering
        targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        // If car is in reverse, invert the target angle
        float steer = Mathf.Clamp(targetAngle * _steerSensitivity, -1f, 1f) * Mathf.Sign(_drive._currentSpeed);

        // If wheel is skidding, cut the torque to that wheel
        for (int i = 0; i < _wheelCollider.Length; i++)
        {
            WheelHit wheelHit;
                if (_hasTorque[i])
                {
                    // If the wheel is slipping sideways, cut the torque
                    if (_wheelCollider[i].GetGroundHit(out wheelHit))
                    {
                        if (_antiSkid && Mathf.Abs(wheelHit.sidewaysSlip) > 0.4f)
                        {
                            _wheelCollider[i].motorTorque = 0.7f;
                        }
                    }
                }
        }

        _drive.Go(_acceleration, steer, _braking);

        _drive.CheckForSkid();
        _drive.CalculateEngineSound();
        ProgressTracker();
    }

    private void ProgressTracker()
    {
        if (Vector3.Distance(_drive._rigidbody.gameObject.transform.position, _tracker.transform.position) < _lookAheadDistance)
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
