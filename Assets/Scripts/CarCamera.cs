using UnityEngine;

public class CarCameraMovement : MonoBehaviour
{
    public Transform target;        // Drag your Truck here
    
    [Header("Settings")]
    public float distance = 6.0f;   // Distance from car
    public float height = 2.0f;     // Height above car
    public float mouseSpeed = 3.0f; // How fast you look around

    private float currentX = 0.0f;  // Stores your mouse X movement
    private float currentY = 0.0f;  // Stores your mouse Y movement

    void LateUpdate()
    {
        if (!target) return;

        // Get mouse input
        currentX += Input.GetAxis("Mouse X") * mouseSpeed;
        currentY -= Input.GetAxis("Mouse Y") * mouseSpeed;

        // dont let camera go below ground or too far up
        currentY = Mathf.Clamp(currentY, -10, 60);

        // convert mouse position to rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Calculate target position, we look at the center of the target, plus height, then back off by distance
        Vector3 targetCenter = target.position + Vector3.up * height;
        
        // Move the camera BACKWARDS from the rotation
        Vector3 finalPosition = targetCenter - (rotation * Vector3.forward * distance);

        // Apply position and rotation
        transform.position = finalPosition;
        transform.LookAt(targetCenter);
    }
}