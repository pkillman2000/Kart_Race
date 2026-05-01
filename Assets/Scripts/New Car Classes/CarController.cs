using System.Collections;
using UnityEngine;

[System.Serializable]
public class ManualTransmission
{
    [SerializeField]
    public string _gearName;
    [SerializeField]
    public float _torqueMultiplier;
    [SerializeField]
    public float _shiftDownSpeed;
    [SerializeField]
    public float _shiftUpSpeed;
    [SerializeField]
    public float _redlineSpeed;
    [SerializeField]
    public float _topSpeed;
}

public class CarController : MonoBehaviour
{
    [Header("Vehicle Settings")]
    [SerializeField]
    private WheelCollider[] _wheelColliders;
    [SerializeField]
    private GameObject[] _wheelMeshes;
    private bool[] _hasTorque = new bool[4];
    private bool[] _canSteer = new bool[4];
    [SerializeField]
    private float _antiRoll = 5000f;
    [SerializeField]
    private GameObject _centerOfMass;
    public Rigidbody _rigidbody;
    [SerializeField]
    public string _carName;
    public bool _hasUI = false;
    [SerializeField]
    private GameObject _carBodyParent;

    [Header("Vehicle Control Type")]
    [SerializeField]
    private bool _isNPCControlled = true;
    private PlayerController _playerController;
    private NPCController _npcController;
    [SerializeField]
    private bool _automaticTransmission = true;
    public enum NumberOfDriveWheels
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }
    public NumberOfDriveWheels _driveWheels;
    private float _lastTimeChecked;
    [SerializeField]
    private float _flipDuration = 3.0f;


    [Header("Acceleration")]
    [SerializeField]
    private float _baseTorque = 300f;
    [Tooltip("Settings for the gears. Can be customized for type of track, NPC or target")]
    [SerializeField]
    private ManualTransmission[] _gears;
    public float _currentRigidbodySpeed;
    private int _currentGear = 0;
    [SerializeField]
    private float _topReverseSpeed = 5f;
    private float _timeBeforeReverse = 0;
    [SerializeField]
    private float _baseBoostTorque = 200f;
    private float _currentBoostTorque = 0f;
    [SerializeField]
    private float _boostDuration = 2f;

    [Header("Steering")]
    [SerializeField]
    private float _maximumSteerAngle = 30f;

    [Header("Braking")]
    [SerializeField]
    private float _maximumBrakeTorque = 500f;
    [SerializeField]
    private Renderer _rightBrakeLightRenderer;
    [SerializeField]
    private Renderer _leftBrakeLightRenderer;
    [SerializeField]
    private Material _brakeLightLitMaterial;
    [SerializeField]
    private Material _brakeLightUnlitMaterial;
    [SerializeField]
    private Material _reverseLightLitMaterial;

    [Header("Audio")]
    [SerializeField]
    private AudioSource _skidSound;
    [SerializeField]
    private AudioSource _engineSound;
    [SerializeField]
    private float _lowPitch = .25f;
    [SerializeField]
    private float _highPitch = 3f;

    [Header("Wheel Smoke")]
    [SerializeField]
    private ParticleSystem _smokePrefab;
    private ParticleSystem[] _skidSmoke;

    [Header("UI")]
    [SerializeField]
    private UIManager _uiManager;

    [Header("Misc")]
    [SerializeField]
    private Light _rightHeadlight;
    [SerializeField]
    private Light _leftHeadlight;
    [SerializeField]
    private bool _headlightsOn = false;
    public int _numberOfLaps;
    public int _currentLap = 0;
    public float _currentLapPercentage = 0;
    public float _racePercentage = 0;
    public float _trackLength = 0;
    public float _lapDistance = 0;
    public float _currentRaceDistance = 0;
    public bool _raceFinished = false;
    [SerializeField]
    private GameObject _cameraContainer;
    [SerializeField]
    private float _rayDistance = 1.5f;
    private ProgressTracker _progressTracker;
    [SerializeField]
    private bool _checkForOffTrack = true;
    [SerializeField]
    private Podium _podium;
    [SerializeField]
    private CameraController _cameraController;

    void Start()
    {
        // Set steering to front wheels
        _canSteer[0] = true;
        _canSteer[1] = true;
        _canSteer[2] = false;
        _canSteer[3] = false;

        // Set drive wheels based on selected drive type
        switch (_driveWheels)
        {
            case NumberOfDriveWheels.FrontWheelDrive:
                _hasTorque[0] = true;
                _hasTorque[1] = true;
                _hasTorque[2] = false;
                _hasTorque[3] = false;
                break;
            case NumberOfDriveWheels.RearWheelDrive:
                _hasTorque[0] = false;
                _hasTorque[1] = false;
                _hasTorque[2] = true;
                _hasTorque[3] = true;
                break;
            case NumberOfDriveWheels.AllWheelDrive:
                for (int i = 0; i < _hasTorque.Length; i++)
                {
                    _hasTorque[i] = true;
                }
                break;
        }

        _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;

        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("Player Controller is Null!");
        }

        _npcController = GetComponent<NPCController>();
        if (_npcController == null)
        {
            Debug.LogError("NPC Controller is Null!");
        }
        _skidSmoke = new ParticleSystem[_wheelColliders.Length];

        _progressTracker = GetComponent<ProgressTracker>();
        if (_progressTracker == null)
        {
            Debug.LogError("Progress Tracker is Null!");
        }

        _podium = FindFirstObjectByType<Podium>();
        if (_podium == null)
        {
            Debug.LogError("Podium is Null!");
        }

        _cameraController = GetComponent<CameraController>();
        if (_cameraController == null)
        {
            Debug.LogError("Camera Controller is Null!");
        }

        // Make care target or NPC controlled
        if (_isNPCControlled)
        {
            _playerController.enabled = false;
            _npcController.enabled = true;

        }
        else
        {
            _playerController.enabled = true;
            _npcController.enabled = false;
        }

        _trackLength = FindFirstObjectByType<Circuit>()._trackLength;
        _numberOfLaps = FindFirstObjectByType<Circuit>()._numberOfLaps;

        Headlights(_headlightsOn);

        InstantiateSmokePrefabs();

        _uiManager = FindFirstObjectByType<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is Null!");
        }


        if (_hasUI)
        {
            _cameraContainer.SetActive(true);
        }
        else
        {
            _cameraContainer.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        _currentRigidbodySpeed = _rigidbody.linearVelocity.magnitude;

        if (_raceFinished)
        {
            ApplyInputs(-0.5f, 0, _maximumBrakeTorque * 5f);
            BrakeLights(1);
        }
        else
        {
            GroundWheels(_wheelColliders[0], _wheelColliders[1]); // Front wheels
            GroundWheels(_wheelColliders[2], _wheelColliders[3]); // Rear wheels
            CheckForSkid();
            RightCar();
            CheckSurfaceMaterial();
        }

        CalculateEngineSound();
        UpdateSpeed();
        UpdateWheelMeshPosition();
    }

    public void SetDriverInput(float acceleration, float braking, float steering)
    {
        acceleration = Mathf.Clamp(acceleration, 0, 1f);
        braking = Mathf.Clamp(braking, 0f, 1f);
        steering = Mathf.Clamp(steering, -1f, 1f);

        // Race must be started 
        if (RaceMonitor.racing)
        {
            // Car must be on the ground to apply acceleration, braking, or steering
            if (!IsOnGround())
            {
                acceleration = 0f;
                braking = 0f;
                steering = 0f;
            }

            // Calculate Acceleration
            if (_automaticTransmission)
            {
                CalculateGear(_currentRigidbodySpeed);
            }
            float torque = (_baseTorque * _gears[_currentGear]._torqueMultiplier * acceleration) + _currentBoostTorque;

            // CalculateBraking
            float brakeTorque = 0f;
            float avgRpm = 0f;

            foreach (var wheel in _wheelColliders)
            {
                avgRpm += wheel.rpm;
            }
            avgRpm /= _wheelColliders.Length;

            if (avgRpm <= .01 && braking > 0f) // If wheels are at almost 0 and braking is applied
            {
                if (Time.time > (_timeBeforeReverse + .25f)) // If the car has been braking for more than .25 seconds, allow it to reverse
                {

                    if (_currentRigidbodySpeed < _topReverseSpeed) // Limit top speedMPH in reverse
                    {
                        torque = -_baseTorque;
                    }
                    else
                    {
                        torque = 0f;
                    }

                    if (_hasUI)
                        _uiManager.SetCurrentGearName("R");
                    brakeTorque = 0;
                    BrakeLights(0);
                    ReverseLights();
                }
            }
            else // Apply brakes normally
            {
                _timeBeforeReverse = Time.time;
                brakeTorque = _maximumBrakeTorque * braking;
                if (_hasUI)
                    _uiManager.SetCurrentGearName(_gears[_currentGear]._gearName);
                BrakeLights(braking);
            }

            // Calculate Steering
            steering = Mathf.Clamp(steering, -1f, 1f) * _maximumSteerAngle;

            ApplyInputs(steering, torque, brakeTorque);
        }
    }

    private void ApplyInputs(float steering, float torque, float brakeTorque)
    {
        // Apply Acceleration, Braking, and Steering to wheels
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            if (_currentRigidbodySpeed < _gears[_currentGear]._topSpeed) // If below top speedMPH, apply torque
            {
                if (_hasTorque[i]) // Acclerate
                {
                    _wheelColliders[i].motorTorque = torque;
                }
                else
                {
                    _wheelColliders[i].motorTorque = 0f;
                }
            }
            else // If above top speedMPH, don't apply torque
            {
                _wheelColliders[i].motorTorque = 0f;
            }

            _wheelColliders[i].brakeTorque = brakeTorque; // Brake

            if (_canSteer[i]) // Steer
            {
                _wheelColliders[i].steerAngle = steering;
            }
            else
            {
                _wheelColliders[i].steerAngle = 0f;
            }

            //UpdateWheelMeshPosition();
        }
    }

    public void ApplyBoost()
    {
        _currentBoostTorque = _baseBoostTorque;
        StartCoroutine(ResetBoostTorque());
    }

    private IEnumerator ResetBoostTorque()
    {
        yield return new WaitForSeconds(_boostDuration);
        _currentBoostTorque = 0f;
    }

    private void UpdateWheelMeshPosition()
    {
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            // Update wheel mesh raceRank and rotation to match collider
            Vector3 position;
            Quaternion rotation;
            _wheelColliders[i].GetWorldPose(out position, out rotation);
            _wheelMeshes[i].transform.position = position;
            _wheelMeshes[i].transform.rotation = rotation;
        }
    }

    private void CalculateGear(float averageWheelSpeed)
    {
        for (int i = 0; i < _gears.Length; i++)
        {
            if (averageWheelSpeed < _gears[i]._shiftUpSpeed && averageWheelSpeed >= _gears[i]._shiftDownSpeed)
            {
                _currentGear = i;
                if (_hasUI)
                    _uiManager.SetCurrentGearName(_gears[i]._gearName);

                break;
            }
        }
    }

    // Can only manually shift gears if not using automatic transmission.
    public void ShiftGearUp()
    {
        if (!_automaticTransmission)
        {
            if (_currentGear < _gears.Length - 1)
            {
                _currentGear++;
            }
        }
        if (_hasUI)
            _uiManager.SetCurrentGearName(_gears[_currentGear]._gearName);
    }

    public void ShiftGearDown()
    {
        if (!_automaticTransmission)
        {

            if (_currentGear > 0)
            {
                _currentGear--;
            }
        }
        if (_hasUI)
            _uiManager.SetCurrentGearName(_gears[_currentGear]._gearName);
    }

    private void BrakeLights(float braking)
    {
        if (braking > 0)
        {
            _rightBrakeLightRenderer.material = _brakeLightLitMaterial;
            _leftBrakeLightRenderer.material = _brakeLightLitMaterial;
        }
        else
        {
            _rightBrakeLightRenderer.material = _brakeLightUnlitMaterial;
            _leftBrakeLightRenderer.material = _brakeLightUnlitMaterial;
        }
    }

    private void ReverseLights()
    {
        BrakeLights(0);
        _rightBrakeLightRenderer.material = _reverseLightLitMaterial;
        _leftBrakeLightRenderer.material = _reverseLightLitMaterial;

    }

    private void Headlights(bool headlightsOn)
    {
        _rightHeadlight.enabled = headlightsOn;
        _leftHeadlight.enabled = headlightsOn;
    }

    private void CalculateEngineSound()
    {
        _engineSound.pitch = Mathf.Lerp(_lowPitch, _highPitch, _currentRigidbodySpeed / _gears[_currentGear]._topSpeed);
        float speedPercent = Mathf.Lerp(0, 1, _currentRigidbodySpeed / _gears[_currentGear]._topSpeed);
        if (_hasUI)
            _uiManager.SetTachometer(speedPercent);
    }

    // Helps keep the car stable when going around corners
    private void GroundWheels(WheelCollider WR, WheelCollider WL)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        // Calculate the suspension travel for each wheel
        bool groundedL = WL.GetGroundHit(out hit);
        if (groundedL)
        {
            travelL = (-WL.transform.InverseTransformPoint(hit.point).y - WL.radius) / WL.suspensionDistance;
        }

        bool groundedR = WR.GetGroundHit(out hit);
        if (groundedR)
        {
            travelR = (-WR.transform.InverseTransformPoint(hit.point).y - WR.radius) / WR.suspensionDistance;
        }

        // Add force in an upward direction to the rigid body based on the difference in suspension travel between the two wheels
        float antiRollForce = (travelL - travelR) * _antiRoll;
        if (groundedL)
        {
            _rigidbody.AddForceAtPosition(WL.transform.up * -antiRollForce, WL.transform.position);
        }

        if (groundedR)
        {
            _rigidbody.AddForceAtPosition(WR.transform.up * antiRollForce, WR.transform.position);
        }
    }

    // Flips car back over if it has been flipped for more than a certain amount of time.
    private void RightCar()
    {
        // Check if car is flipped or has velocity
        if (transform.up.y > 0.3f || _rigidbody.angularVelocity.magnitude > .1f)
        {
            _lastTimeChecked = Time.time;
        }

        // If the car has been flipped for more than 3 seconds, flip it back over
        if (Time.time > _lastTimeChecked + _flipDuration)
        {
            // Lift car off of ground before flipping it to prevent it from getting stuck in the ground
            this.transform.position += Vector3.up;
            // Flip the car
            this.transform.rotation = Quaternion.LookRotation(this.transform.forward);
        }
    }

    private bool IsOnGround()
    {
        int groundedWheels = 0;
        foreach (var wheel in _wheelColliders)
        {
            if (wheel.isGrounded)
            {
                groundedWheels++;
            }
        }

        if (groundedWheels > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckForSkid()
    {
        int numSkidding = 0;

        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            WheelHit wheelHit;

            // Check to see if wheel collider is touching the ground
            if (_wheelColliders[i].GetGroundHit(out wheelHit))
            {
                // Check if the wheel is skidding based on the slip values to play sound and emit smoke
                if (Mathf.Abs(wheelHit.forwardSlip) > 0.4f || Mathf.Abs(wheelHit.sidewaysSlip) > 0.4f)
                {
                    numSkidding++;
                    if (!_skidSound.isPlaying)
                    {
                        _skidSound.Play();
                        _skidSmoke[i].transform.position = _wheelColliders[i].transform.position - (_wheelColliders[i].transform.up * _wheelColliders[i].radius);
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
        GameObject smokeContainer = transform.Find("Smoke Container").gameObject;
        for (int i = 0; i < _wheelColliders.Length; i++)
        {
            _skidSmoke[i] = Instantiate(_smokePrefab, smokeContainer.transform);
            _skidSmoke[i].Stop();
        }
    }

    public void CheckSurfaceMaterial()
    {
        if (_checkForOffTrack)
        {
            Ray ray = new Ray(_rigidbody.transform.position, Vector3.down);
            RaycastHit hit;

            // The 'out hit' parameter is filled with data if the ray returns true
            if (Physics.Raycast(ray, out hit, _rayDistance))
            {
                // Use the Layer to identify the surface
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    // Move vehicle to tracker raceRank if it is on the ground layer
                    GameObject trackerPosition = _progressTracker.GetTrackerPosition();
                    _rigidbody.transform.position = trackerPosition.transform.position + (Vector3.up * 2);
                    _rigidbody.transform.rotation = trackerPosition.transform.rotation;
                }
            }
        }
    }

    private void UpdateSpeed()
    {
        float speedNeedle = _currentRigidbodySpeed / _gears[_gears.Length - 1]._topSpeed; // Get speed as a value between 0 and 1 for the speedometer needle
        float speedMPH = _currentRigidbodySpeed * 2.237f; // Convert from m/s to mph
        if (_hasUI)
            _uiManager.SetSpeed(speedNeedle, Mathf.RoundToInt(speedMPH).ToString());
    }

    public void UpdateLapProgress(float distance)
    {
        if (_currentLap > 0)
        {
            _lapDistance = distance;
            _currentRaceDistance = distance + ((_currentLap - 1) * _trackLength);

            _currentLapPercentage = _lapDistance / _trackLength * 100f;
            _racePercentage = _currentRaceDistance / (_trackLength * _numberOfLaps) * 100f;
            if (_hasUI)
            {
                if (_raceFinished)
                {
                    _uiManager.SetRaceProgress(_numberOfLaps, _numberOfLaps, 0);
                }
                else
                {
                    _uiManager.SetRaceProgress(_currentLap, _numberOfLaps, _currentLapPercentage);
                }
            }
        }
    }

    public void UpdateRaceProgress()
    {
        _currentLap++;
        if (_currentLap > _numberOfLaps)
        {
            _raceFinished = true;
            StartCoroutine(MoveCarToPodium());
        }
    }

    IEnumerator MoveCarToPodium()
    {
        int raceRank = _uiManager.GetVehiclePosition(_carName);
        int podiumIndex = raceRank - 1;
        Transform podiumTransform = _podium.GetPodiumPosition(podiumIndex);

        yield return new WaitForSeconds(2.0f);

        if (_hasUI)
        {
            _cameraController.SwitchToPodiumCamera();
            _uiManager.HideUI();
        }
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _rigidbody.gameObject.transform.position = podiumTransform.position;
        _rigidbody.transform.rotation = podiumTransform.rotation;
    }
}
