using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField]
    private CarController _drive;
    private Rigidbody _rigidbody;

    // Wheel Colliders
    [SerializeField]
    private WheelCollider _wheelColliderFrontLeft;
    [SerializeField]
    private WheelCollider _wheelColliderFrontRight;
    [SerializeField]
    private WheelCollider _wheelColliderRearLeft;
    [SerializeField]
    private WheelCollider _wheelColliderRearRight;
    private GameObject _centerOfMass;
    private float _antiRoll = 5000f;


    void Start()
    {
        _rigidbody = _drive._rigidbody;
        _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;
    }


    void FixedUpdate()
    {
        GroundWheels(_wheelColliderFrontLeft, _wheelColliderFrontRight);
        GroundWheels(_wheelColliderRearLeft, _wheelColliderRearRight);
    }

    private void GroundWheels(WheelCollider WL, WheelCollider WR)
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
}
