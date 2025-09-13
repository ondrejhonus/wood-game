using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform grabPoint;
    private float grabDistance;

    [SerializeField] private float followSpeed = 5f; // lower = slower

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.None;
    }

    public void Grab(Transform grabPointTransform)
    {
        grabPoint = grabPointTransform;
        rb.useGravity = false;
        // Keep initial distance from camera
        grabDistance = Vector3.Dot(transform.position - grabPoint.position, grabPoint.forward);
    }

    public void Drop()
    {
        grabPoint = null;
        rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (grabPoint != null)
        {
            // Keep object at fixed distance in front of grab point
            Vector3 targetPos = grabPoint.position + grabPoint.forward * grabDistance;

            // Smooth out movement
            Vector3 velocity = (targetPos - transform.position) * followSpeed;

            // Apply linear velocity to rigidbody
            rb.linearVelocity = velocity;
        }
    }
}
