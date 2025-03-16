using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; 
    public float smoothSpeed = 0.125f;
    public float yOffset = 0f; 

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = new Vector3(player.position.x, yOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
