using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    /*
     * Checkpoints are for respawning
     * Waypoint are for the tracker to follow
    */

    [SerializeField]
    public GameObject[] _waypoints;
    [SerializeField]
    public GameObject[] _checkpoints;

    private void Awake()
    {

        AddWaypointsToArray();
        AddCheckpointsToArray();
    }

    // Waypoints
    private void AddWaypointsToArray()
    {
        int index = 0;
        List<GameObject> childrenList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();
            if (waypoint.isCheckpoint)
            {
                childrenList.Add(child.gameObject);
                waypoint.waypointIndex = index;
                index++;
            }
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

    // Checkpoints
    private void AddCheckpointsToArray()
    {
        List<GameObject> childrenList = new List<GameObject>();
        int index = 0;
        foreach (Transform child in transform)
        {
            Waypoint waypoint = child.GetComponent<Waypoint>();
            waypoint.checkpointIndex = index;
            childrenList.Add(child.gameObject);
            index++;
        }

        _checkpoints = childrenList.ToArray();
    }

    public int GetPreviousCheckpointIndex(int currentCheckpointIndex)
    {
        if (currentCheckpointIndex > 0)
        {
            return currentCheckpointIndex - 1;
        }
        else
        {
            return _checkpoints.Length - 1;
        }
    }

    public int GetNextCheckpointIndex(int currentCheckpointIndex)
    {
        if (currentCheckpointIndex < _checkpoints.Length - 1)
        {
            return currentCheckpointIndex + 1;
        }
        else
        {
            return 0;
        }
    }

}
