using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffectMultiplier = 0.5f; // Điều chỉnh tốc độ di chuyển
    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0);
        lastCameraPosition = cameraTransform.position;
    }
}
