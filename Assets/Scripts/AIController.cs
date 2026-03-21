using UnityEngine;

public class AIController : MonoBehaviour
{
    private Drive _drive;
    private bool[] _hasTorque;
    private WheelCollider[] _wheelCollider;
    private bool _antiSkid = true;

    [SerializeField]
    private Circuit _circuit;
    [SerializeField]
    private float _steerSensitivity = 0.01f;

    Vector3 _target;
    int _currentWaypointIndex = 0;  


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
    }


    void Update()
    {
        // Put target and car on same plane
        Vector3 localTarget = _drive._rigidbody.gameObject.transform.InverseTransformPoint(_target);

        float distanceToTarget = Vector3.Distance(_target, _drive._rigidbody.gameObject.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        // If car is in reverse, invert the target angle
        float steer = Mathf.Clamp(targetAngle * _steerSensitivity, -1f, 1f) * Mathf.Sign(_drive._currentSpeed);

        float acceleration = 1f;
        float brake = 0f;

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


            _drive.Go(acceleration, steer, brake);

        // Check if the car is close enough to the target waypoint to switch to the next one
        if (distanceToTarget < 4f)
        {
            _currentWaypointIndex = _circuit.GetNextWaypointIndex(_currentWaypointIndex);
            _target = _circuit._waypoints[_currentWaypointIndex].transform.position;
        }

        _drive.CheckForSkid();
        _drive.CalculateEngineSound();
    }
}
