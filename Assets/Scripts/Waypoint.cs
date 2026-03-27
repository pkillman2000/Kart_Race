using UnityEngine;

public class Waypoint : MonoBehaviour
{
    /*
     * Checkpoints are for respawning
     * Waypoint are for the tracker to follow
    */
    public bool isCheckpoint;

    public float _brakeValue = 0f;

    public float _accleratorValue = 1.0f;

    public int checkpointIndex;
    public int waypointIndex;
}
