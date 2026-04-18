using System;
using UnityEngine;

public class FlipCar : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _lastTimeChecked;
    [SerializeField]
    private float _flipDuration = 3.0f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if(_rigidbody == null)
        {
            Debug.LogError("Rigidbody is Null!");
        }
    }


    void Update()
    {
        // Check if car is flipped or has velocity
        if(transform.up.y > 0.3f || _rigidbody.angularVelocity.magnitude > .1f)
        {
            _lastTimeChecked = Time.time;
        }

        // If the car has been flipped for more than 3 seconds, flip it back over
        if (Time.time > _lastTimeChecked + _flipDuration)
        {
            RightCar();
        }
    }

    private void RightCar()
    {
        // Lift car off of ground before flipping it to prevent it from getting stuck in the ground
        this.transform.position += Vector3.up;
        // Flip the car
        this.transform.rotation = Quaternion.LookRotation(this.transform.forward);
    }
}
