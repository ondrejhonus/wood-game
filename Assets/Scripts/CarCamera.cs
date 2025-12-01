using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;       // Truck object
    public Vector3 offset; // This gets calculated at start
    public float smoothSpeed = 0.125f;

    void Start()
    {
        // Calculate offset based on starting positions, eg. where i placed the mount point
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Calculate desired position, with offset rotated to match target rotation, so it stays behind the car at all times
        Vector3 desiredPosition = target.position + target.TransformDirection(Quaternion.Inverse(target.rotation) * offset);

        // Smooth the camera movement, so it doesnt jerk around weirdly
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Always look at the car
        transform.LookAt(target);
    }
}