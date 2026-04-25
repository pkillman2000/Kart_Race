using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target; // Drag your player Car Body Parent here


    void LateUpdate()
    {
        if (target != null)
        {
            // Set camera position to target's X and Z, keep camera's current Y
            Vector3 newPosition = target.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            float objectRotationX = transform.eulerAngles.x;
            float objectrotationZ = transform.eulerAngles.z;
            float playerRotationY = target.eulerAngles.y;
            transform.rotation = Quaternion.Euler(objectRotationX, playerRotationY, objectrotationZ);
        }
    }
}
