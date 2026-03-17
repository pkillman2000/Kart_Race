using UnityEngine;

public class Drive : MonoBehaviour
{
    [SerializeField]
    private WheelCollider[] _wheelCollider;
    [SerializeField]
    private GameObject[] _wheelMesh;
    [SerializeField]
    private bool[] _canSteer;
    [SerializeField]
    private bool[] _canBrake;

    // Forward and Reverse
    [SerializeField]
    private float _torque = 200f;

    // Steering
    [SerializeField]
    private float _maximumSteerAngle = 30f;

    // Braking
    [SerializeField]
    private float _maximumBrakeTorque = 500f;

    public void Go(float acceleration, float steering, float brake)
    {
        // Apply motor torque to the wheel collider based on the acceleration input
        acceleration = Mathf.Clamp(acceleration, -1f, 1f);
        float _thrustTorque = acceleration * _torque;

        // Loop through each wheel collider and apply the motor torque and steering
        for (int i = 0; i < _wheelCollider.Length; i++)
        {
            // Forward and Reverse
            _wheelCollider[i].motorTorque = _thrustTorque;

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

            // Rotate wheel meshes to match the wheel collider
            Quaternion quat;
            Vector3 position;
            _wheelCollider[i].GetWorldPose(out position, out quat);
            _wheelMesh[i].transform.position = position;
            _wheelMesh[i].transform.rotation = quat;
        }
    }
}
