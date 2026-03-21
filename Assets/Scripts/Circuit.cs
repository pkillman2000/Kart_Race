using System.Collections.Generic;
using UnityEngine;

public class Circuit : MonoBehaviour
{
    [SerializeField]
    private bool _showPath = true;
    [SerializeField]
    public GameObject[] _waypoints;

    private void Awake()
    {

        AddWaypointsToArray();
    }

    private void AddWaypointsToArray()
    {
        List<GameObject> childrenList = new List<GameObject>();

        foreach (Transform child in transform)
        {
            childrenList.Add(child.gameObject);
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

    private void OnDrawGizmos()
    {
        if (_showPath)
        {
            Vector3 currentWaypoint = new Vector3();
            Vector3 nextWaypoint = new Vector3();

            if (_waypoints == null || _waypoints.Length == 0)
                return;
            for (int i = 0; i < _waypoints.Length; i++)
            {
                currentWaypoint = _waypoints[i].transform.position;
                nextWaypoint = _waypoints[(i + 1) % _waypoints.Length].transform.position;
                Gizmos.color = Color.white;
                //Gizmos.DrawSphere(currentWaypoint, 0.5f);
                Gizmos.DrawLine(currentWaypoint, nextWaypoint);
            }

            Gizmos.DrawLine(nextWaypoint, _waypoints[0].transform.position);
        }
    }
}
