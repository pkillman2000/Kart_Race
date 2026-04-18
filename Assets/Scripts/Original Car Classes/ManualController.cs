using UnityEngine;

public class ManualController : MonoBehaviour
{
    private InputActions _inputActions;
    private float _acceleration = 0f;
    private float _steer = 0f;
    private float _brake = 0f;

    private CarController _carController;

    private void Start()
    {
        _carController = GetComponent<CarController>();
        if (_carController == null)
        {
            Debug.LogError("CarController is Null!");
        }
    }

    private void Update()
    {
        // Read input values from the InputActions
        _acceleration = _inputActions.Driving.Accelerate.ReadValue<float>();
        _steer = _inputActions.Driving.Steer.ReadValue<float>();
        _brake = _inputActions.Driving.Brake.ReadValue<float>();

        if (_inputActions.Driving.ShiftGearUp.WasPressedThisFrame())
        {
            _carController.ShiftGearUp();
        }

        if (_inputActions.Driving.ShiftGearDown.WasPressedThisFrame())
        {
            _carController.ShiftGearDown();
        }

        _carController.SetDriverInput(_acceleration, _brake, _steer);
        Debug.Log($"Acceleration: {_acceleration}, Steer: {_steer}, Brake: {_brake}");
    }

    //Enable/Disable New Input System
    private void OnEnable()
    {
        _inputActions = new InputActions();
        if (_inputActions == null)
        {
            Debug.LogError("InputActions is Null!");
        }
        else
        {
            _inputActions.Driving.Enable();
        }
    }

    private void OnDisable()
    {
        _inputActions.Driving.Disable();
    }

}
