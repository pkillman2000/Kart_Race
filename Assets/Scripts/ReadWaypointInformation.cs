using UnityEngine;

public class ReadWaypointInformation : MonoBehaviour
{
    private AIController _aiController;
    private GameObject _currentWaypoint;

    private void Start()
    {
        _aiController = GetComponentInParent<AIController>();
        if(_aiController == null )
        {
            Debug.LogError("AI Controller is Null!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Waypoint")
        {
            float acceleration = other.GetComponent<Waypoint>()._accleratorValue;
            float braking = other.GetComponent<Waypoint>()._brakeValue;

            _aiController.SetBrakingAndAccleration(braking, acceleration);
            _currentWaypoint = other.gameObject;
        }
    }

    public GameObject GetCurrentWaypoint()
    {
        return _currentWaypoint;
    }

}
