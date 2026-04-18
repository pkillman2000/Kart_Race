using UnityEngine;

/*
 * This class HAS to be on the same Game Object as the collider.
 * It reads information from the waypoint and sends it to the NPCController.
*/

public class ReadWaypointInformation : MonoBehaviour
{
    private NPCController _npcController;
    private GameObject _currentWaypoint;

    private void Start()
    {
        _npcController = GetComponentInParent<NPCController>();
        if (_npcController == null)
        {
            Debug.LogError("NPC Controller is Null!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            float acceleration = other.GetComponent<Waypoint>()._accleratorValue;
            float braking = other.GetComponent<Waypoint>()._brakeValue;

            _npcController.SetBrakingAndAccleration(braking, acceleration);
            _currentWaypoint = other.gameObject;
        }
    }

    public GameObject GetCurrentWaypoint()
    {
        return _currentWaypoint;
    }

}
