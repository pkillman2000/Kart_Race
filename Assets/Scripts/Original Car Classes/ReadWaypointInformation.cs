using UnityEngine;

/*
 * This class HAS to be on the same Game Object as the collider.
 * It reads information from the waypoint and sends it to 
 * the NPCController and UI Manager.
*/

public class ReadWaypointInformation : MonoBehaviour
{
    private NPCController _npcController;
    private GameObject _currentWaypoint;
    private CarController _carController;

    private void Start()
    {
        _npcController = GetComponentInParent<NPCController>();
        if (_npcController == null)
        {
            Debug.LogError("NPC Controller is Null!");
        }

        _carController = GetComponentInParent<CarController>();
        if (_carController == null)
        {
            Debug.LogError("Car Controller is Null!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            Waypoint waypoint = other.GetComponent<Waypoint>();
            float acceleration = waypoint._accleratorValue;
            float braking = waypoint._brakeValue;
            int waypointIndex = waypoint.waypointIndex;

            _npcController.SetBrakingAndAccleration(braking, acceleration);
            _currentWaypoint = other.gameObject;

            _carController.UpdateLapProgress(waypoint._cumulativeTrackDistance);

            if (waypoint.isStartingLine)
            {
                _carController.UpdateRaceProgress();
            }
        }
    }

    public GameObject GetCurrentWaypoint()
    {
        return _currentWaypoint;
    }
}
