using System;
using UnityEngine;

public class UnstickCar : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _lastTimeChecked;
    private ReadWaypointInformation _readWaypointInformation;
    private AIController _aiController;
    [SerializeField]
    private float _stickDuration = 3.0f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody is Null!");
        }

        _readWaypointInformation = GetComponent<ReadWaypointInformation>();
        if (_readWaypointInformation == null)
        {
            Debug.LogError("Read Waypoint Information is Null!");
        }

        _aiController = GetComponentInParent<AIController>();
        if (_aiController == null)
        {
            Debug.LogError("AI Controller is Null!");
        }
    }

    void Update()
    {
        if (RaceMonitor.racing == true)
        {
            // Check if car has velocity
            if (_rigidbody.linearVelocity.magnitude > 1f)
            {
                _lastTimeChecked = Time.time;
            }

            // Check if the car has been flipped or stuck for more than _stickDuration seconds
            if (Time.time > (_lastTimeChecked + _stickDuration))
            {
                Unstick();
            }
        }
        else
        {
            _lastTimeChecked = Time.time;
        }
    }

    // Move to last waypoint
    private void Unstick()
    {
        GameObject trackerPosition = _aiController.GetTrackerPosition();
        _rigidbody.transform.position = trackerPosition.transform.position + (Vector3.up * 2);
        _rigidbody.transform.rotation = trackerPosition.transform.rotation;
        // If car is set to brake with no acceleration by a waypoint, it will not move after being unstuck
        _aiController.SetBrakingAndAccleration(0f, 1f);
    }
}
