using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isStartingLine = false;

    public float _brakeValue = 0f;

    public float _accleratorValue = 1.0f;

    public int waypointIndex = 0;

    public float _cumulativeTrackDistance = 0f;
}
