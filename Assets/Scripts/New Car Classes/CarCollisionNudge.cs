using UnityEngine;

/// <summary>
/// Attach this script to your car GameObjects that have a Rigidbody and Collider.
/// Handles collision detection between cars and applies a nudge force to the car in front.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarCollisionNudge : MonoBehaviour
{
    [Header("Nudge Settings")]
    [Tooltip("The force multiplier applied when nudging another car.")]
    [SerializeField] private float nudgeForce = 1000f;

    [Tooltip("The minimum relative velocity required to trigger a nudge.")]
    [SerializeField] private float minVelocityForNudge = 2f;

    [SerializeField]
    private GameObject _collisionVFXPrefab;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we collided with another car
        if (collision.gameObject.tag != "Vehicle")
            return;

        // Get the other car's rigidbody
        Rigidbody otherRb = collision.gameObject.GetComponent<Rigidbody>();
        if (otherRb == null)
            return;

        // Calculate relative velocity (how fast we're approaching the other car)
        Vector3 relativeVelocity = _rb.linearVelocity - otherRb.linearVelocity;
        float relativeSpeed = relativeVelocity.magnitude;

        // Check if we're hitting hard enough to trigger a nudge
        if (relativeSpeed < minVelocityForNudge)
            return;

        // Determine which car is in front using dot product
        // If our forward direction dot with the direction to the other car is positive,
        // it means the other car is in front of us
        Vector3 directionToOther = (collision.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToOther);

        if (dotProduct > 0.1f) // Other car is roughly in front of us
        {
            // We are the hitter, apply nudge to the car in front
            ApplyNudge(otherRb, collision, directionToOther);
            // Use ApplyVelocityBasedNudge(otherRb, collision) for a more physics-based approach
        }
    }

    // Applies a nudge force to the target car in the direction of collision.
    private void ApplyNudge(Rigidbody targetRb, Collision collision, Vector3 direction)
    {
        // Get the collision contact point normal (direction of impact)
        Vector3 collisionNormal = collision.GetContact(0).normal;

        // Calculate the nudge direction - from hitter to target, flattened on XZ plane
        Vector3 nudgeDirection = direction;
        nudgeDirection.y = 0f; // Keep the nudge horizontal (no lifting)
        nudgeDirection.Normalize();

        // Calculate force based on relative velocity
        //float impactForce = nudgeForce * collision.relativeVelocity.magnitude * 0.1f; // This scales the force based on how fast we're going, you can adjust the multiplier as needed
        float impactForce = nudgeForce; // This applies a constant force regardless of speed, you can adjust as needed
        // Apply the nudge force to the target car
        targetRb.AddForce(nudgeDirection * impactForce, ForceMode.Impulse);

        // Get the contact point
        ContactPoint contact = collision.GetContact(0);
        Vector3 contactPoint = contact.point;

        // Calculate rotation based on collision normal (particles shoot outward from surface)
        Quaternion particleRotation = Quaternion.LookRotation(contact.normal);

        // Instantiate the particle effect
        GameObject particleInstance = Instantiate(_collisionVFXPrefab, contactPoint, particleRotation);

        Destroy(particleInstance, 1f);
    }
}