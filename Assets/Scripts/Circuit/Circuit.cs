using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    /*
     * Checkpoints are for respawning
     * Waypoint are for the tracker to follow
    */

    [SerializeField]
    public GameObject[] _waypoints;

    public float _trackLength;
    public int _numberOfLaps = 5;

    private void Awake()
    {
        AddWaypointsToArray();
        CalculateTrackLength();
    }

    // Waypoints
    private void AddWaypointsToArray()
    {
        int index = 0;
        List<GameObject> childrenList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();
            childrenList.Add(child.gameObject);
            waypoint.waypointIndex = index;
            index++;
        }
        _waypoints = childrenList.ToArray();
    }

    public int GetPreviousWaypointIndex(int currentWaypointIndex)
    {
        if (currentWaypointIndex > 0)
        {
            return currentWaypointIndex - 1;
        }
        else
        {
            return _waypoints.Length - 1;
        }
    }

    public int GetNextWaypointIndex(int currentWaypointIndex)
    {
        if (currentWaypointIndex < _waypoints.Length - 1)
        {
            return currentWaypointIndex + 1;
        }
        else
        {
            return 0;
        }
    }

    private void CalculateTrackLength()
    {
        float legDistance = 0f;
        int nextWaypointIndex = 0;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            nextWaypointIndex = GetNextWaypointIndex(i);
            Vector3 currentWaypointPosition = _waypoints[i].transform.position;
            Vector3 nextWaypointPosition = _waypoints[nextWaypointIndex].transform.position;
            legDistance = Vector3.Distance(currentWaypointPosition, nextWaypointPosition);
            _trackLength += legDistance;
            _waypoints[nextWaypointIndex].GetComponent<Waypoint>()._cumulativeTrackDistance = _trackLength;
        }
    }
}
