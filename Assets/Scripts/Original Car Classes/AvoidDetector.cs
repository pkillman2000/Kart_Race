using UnityEngine;

/*
 * This class HAS to be on the same Game Object as the collider.
 * It detects when the car is colliding with another car and sets
 * the avoid path and time for the NPCController to use.
 */
public class AvoidDetector : MonoBehaviour
{
    public float _avoidPath = 0f;
    public float _avoidTime = 0f;
    [SerializeField]
    public float _wanderDistance = 0.5f; // Avoiding distance
    [SerializeField]
    public float _avoidLength = 1f; // How long car is in avoid mode in seconds


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Vehicle")
        {
            Rigidbody otherCar = collision.gameObject.GetComponent<Rigidbody>();
            _avoidTime = Time.time + _avoidLength;

            Vector3 otherCarLocalTarget = transform.InverseTransformPoint(otherCar.transform.position);
            float otherCarAngle = Mathf.Atan2(otherCarLocalTarget.x, otherCarLocalTarget.z);
            /*
             * This calculates an avoid path based on location of other car.
             * The car in front will move one direction and the car in the
             * rear will move in the opposite direction.
            */
            _avoidPath = _wanderDistance * -Mathf.Sign(otherCarAngle);
        }
    }

    // Cancel avoiding once no longer in collision    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            _avoidTime = 0f;
        }
    }
}
