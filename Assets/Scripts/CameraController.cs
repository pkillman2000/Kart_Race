using UnityEngine;
using UnityEngine.InputSystem;


public class CameraController : MonoBehaviour
{
    private InputActions _inputActions;

    [SerializeField]
    private GameObject[] _cameras;

    private int _cameraIndex = 0;

    private void Awake()
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

    void Start()
    {
        DisableAllCameras();
        _cameras[0].SetActive(true);
    }

    private void OnEnable()
    {
        _inputActions.Driving.Camera.performed += SwitchCameraPerformed;
    }

    private void OnDisable()
    {
        _inputActions.Driving.Camera.performed -= SwitchCameraPerformed;
    }

    private void SwitchCameraPerformed(InputAction.CallbackContext context)
    {
        _cameraIndex++;
        if (_cameraIndex >= _cameras.Length)
        {
            _cameraIndex = 0;
        }
        DisableAllCameras();
        _cameras[_cameraIndex].SetActive(true);
    }

    private void DisableAllCameras()
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            if (_cameras[i] != null)
            {
                _cameras[i].SetActive(false);
            }
        }
    }

}
