using UnityEngine;

public class Drive : MonoBehaviour
{
    [SerializeField]
    private WheelCollider[] _wheelCollider;
    [SerializeField]
    private GameObject[] _wheelMesh;
    [SerializeField]
    private Rigidbody _rigidbody;

    // Forward and Reverse
    [SerializeField]
    private float _torque = 200f;
    [SerializeField]
    private bool[] _hasTorque;
    [SerializeField]
    private float _maximumSpeed = 70f;
    private float _currentSpeed;

    // Steering
    [SerializeField]
    private float _maximumSteerAngle = 30f;
    [SerializeField]
    private bool[] _canSteer;

    // Braking
    [SerializeField]
    private float _maximumBrakeTorque = 500f;
    [SerializeField]
    private bool[] _canBrake;
    [SerializeField]
    private Material _brakeLightMaterial;

    // Audio
    [SerializeField]
    private AudioSource _skidSound;
    [SerializeField]
    private AudioSource _engineSound;


    // Wheel Smoke
    [SerializeField]
    private ParticleSystem _smokePrefab;
    private ParticleSystem[] _skidSmoke;

    // Shifting Gears
    [SerializeField]
    private float _lowPitch = .25f;
    [SerializeField]
    private float _highPitch = 3f;
    [SerializeField]
    private int _numberOfGears = 5;

    private void Start()
    {
        _skidSmoke = new ParticleSystem[_wheelCollider.Length];
        InstantiateSmokePrefabs();

        _brakeLightMaterial.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        CheckForSkid();
        CalculateEngineSound();
    }

    public void Go(float acceleration, float steering, float brake)
    {
        // Apply motor torque to the wheel collider based on the acceleration input
        acceleration = Mathf.Clamp(acceleration, -1f, 1f);
        float _thrustTorque = acceleration * _torque;

        // Loop through each wheel collider and apply the motor torque and steering
        for (int i = 0; i < _wheelCollider.Length; i++)
        {
            // Forward and Reverse
            _currentSpeed = _rigidbody.angularVelocity.magnitude;

            if (_currentSpeed < _maximumSpeed)
            {
                if (_hasTorque[i])
                {
                    _wheelCollider[i].motorTorque = _thrustTorque;
                }
            }
            else
            {
                Debug.Log("Max Speed Reached");
            }

            // Steering
            if (_canSteer[i])
            {
                steering = Mathf.Clamp(steering, -1f, 1f) * _maximumSteerAngle;
                _wheelCollider[i].steerAngle = steering;
            }

            // Braking
            brake = Mathf.Clamp(brake, 0f, 1f) * _maximumBrakeTorque;
            if (_canBrake[i])
            {
                _wheelCollider[i].brakeTorque = brake;
            }

            // Turn brake light on/off based on brake input
            if (brake > 0f)
            {
                _brakeLightMaterial.EnableKeyword("_EMISSION");
            }
            else
            {
                _brakeLightMaterial.DisableKeyword("_EMISSION");
            }

            // Position and rotate wheel meshes to match the wheel collider
            Quaternion quat;
            Vector3 position;
            _wheelCollider[i].GetWorldPose(out position, out quat);
            _wheelMesh[i].transform.position = position;
            _wheelMesh[i].transform.rotation = quat;
        }
    }

    private void CheckForSkid()
    {
        int numSkidding = 0;

        for (int i = 0; i < _wheelCollider.Length; i++)
        {
            WheelHit wheelHit;

            // Check to see if wheel collider is touching the ground
            if (_wheelCollider[i].GetGroundHit(out wheelHit))
            {
                // Check if the wheel is skidding based on the slip values
                if (Mathf.Abs(wheelHit.forwardSlip) > 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) > 0.4f)
                {
                    numSkidding++;
                    if (!_skidSound.isPlaying)
                    {
                        _skidSound.Play();
                        _skidSmoke[i].transform.position = _wheelCollider[i].transform.position - _wheelCollider[i].transform.up * _wheelCollider[i].radius;
                        _skidSmoke[i].Emit(1);
                    }
                }
            }
        }

        // Turn off sound if we are no longer skidding
        if (numSkidding == 0 && _skidSound.isPlaying)
        {
            _skidSound.Stop();
        }
    }

    // Instantiate the skid smoke particle systems for each wheel
    private void InstantiateSmokePrefabs()
    {
        for (int i = 0; i < _wheelCollider.Length; i++)
        {
            _skidSmoke[i] = Instantiate(_smokePrefab);
            _skidSmoke[i].Stop();
        }
    }

    private void CalculateEngineSound()
    {
        _currentSpeed = _rigidbody.linearVelocity.magnitude;
        float range = _maximumSpeed / _numberOfGears;

        if (_currentSpeed < range) // First Gear
        {
            _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, _currentSpeed / range);
        }
        else if (_currentSpeed < range * 2) // Second Gear
        {
            _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, (_currentSpeed - range) / (range * 2));
        }
        else if (_currentSpeed < range * 3) // Third Gear
        {
            _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, (_currentSpeed - (range * 2)) / (range * 3));
        }
        else if (_currentSpeed < range * 4) // Fourth Gear
        {
            _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, (_currentSpeed - (range * 3)) / (range * 4));
        }
        else if(_currentSpeed < range * 5) // Fifth Gear
        {
            _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, (_currentSpeed - (range * 4)) / (range * 5));
        }
    }
}
