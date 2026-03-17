using UnityEngine;
using UnityEngine.InputSystem;

public class ManualController : MonoBehaviour
{
    private InputActions _inputActions;
    private float _forwardReverse = 0f;
    private float _steer = 0f;
    private float _brake = 0f;

    private Drive _drive;

    private void Start()
    {
        _drive = GetComponent<Drive>();
        if(_drive == null)
        {
            Debug.LogError("Drive is Null!");
        }
    }

    private void Update()
    {
        _forwardReverse = _inputActions.Driving.ForwardReverse.ReadValue<float>();
        _steer = _inputActions.Driving.Steer.ReadValue<float>();
        _brake = _inputActions.Driving.Brake.ReadValue<float>();

        Debug.Log("Brake Value: " + _brake);

        _drive.Go(_forwardReverse, _steer, _brake);
    }

    /*
    * New Input System
    */

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
