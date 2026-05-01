using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    private GameObject _tracker;
    private int _currentTrackerWaypointIndex = 0;
    [SerializeField]
    private float _lookAheadDistance = 5f;

    private CarController _carController;
    private Circuit _circuit;


    void Start()
    {
        _carController = GetComponent<CarController>();
        if (_carController == null)
        {
            Debug.LogError("CarController is Null!");
        }

        _circuit = FindFirstObjectByType<Circuit>();
        if (_circuit == null)
        {
            Debug.LogError("Circuit is Null!");
        }

        // Create and position the tracker object
        GameObject smokeContainer = transform.Find("Smoke Container").gameObject;

        _tracker = new GameObject("Tracker");
        _tracker.transform.position = _carController._rigidbody.gameObject.transform.position;
        _tracker.transform.rotation = _carController._rigidbody.gameObject.transform.rotation;
        _tracker.transform.parent = smokeContainer.transform;
    }

    private void FixedUpdate()
    {
        MoveTracker();
    }

    // Move the tracker along the waypoints, and look at the next waypoint
    private void MoveTracker()
    {
        if (Vector3.Distance(_carController._rigidbody.gameObject.transform.position, _tracker.transform.position) < _lookAheadDistance)
        {
            _tracker.transform.LookAt(_circuit._waypoints[_currentTrackerWaypointIndex].transform.position);
            _tracker.transform.Translate(Vector3.forward);

            if (Vector3.Distance(_tracker.transform.position, _circuit._waypoints[_currentTrackerWaypointIndex].transform.position) < 1)
            {
                _currentTrackerWaypointIndex = _circuit.GetNextWaypointIndex(_currentTrackerWaypointIndex);
            }
        }
    }

    public GameObject GetTrackerPosition()
    {
        return _tracker.gameObject;
    }
}
