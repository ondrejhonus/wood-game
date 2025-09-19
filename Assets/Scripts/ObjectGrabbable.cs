using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform grabPoint;
    private float grabDistance;
    private Vector3 grabOffset;

    [SerializeField] private float followSpeed = 5f; // lower = slower
    [SerializeField] private float thirdPersonFollowSpeed = 3f; // lower = slower
    [SerializeField] private float massScaling = 1f; // (higher = mass slows it down more)
    [SerializeField] private float velocityLerp = 0.2f; // (lower = more smoothing)
    [SerializeField] public CameraSwitcher cameraSwitcher;
    public System.Action<ObjectGrabbable> OnDestroyed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform grabPointTransform, Vector3 hitPoint)
    {
        grabPoint = grabPointTransform;
        rb.useGravity = false;

        // Keep initial distance from camera
        grabDistance = Vector3.Dot(transform.position - grabPoint.position, grabPoint.forward);

        // Calculate grab point offset
        grabOffset = transform.position - hitPoint;
    }

    public void Drop()
    {
        // Release object
        grabPoint = null;
        // reset its gravity
        rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (grabPoint != null)
        {
            // Keep object at fixed distance in front of grab point
            Vector3 targetPos = grabPoint.position + grabPoint.forward * grabDistance;

            targetPos += grabOffset;

            // Adjust speed based on mass
            float adjustedSpeed;
            if (!cameraSwitcher.IsFirstPerson)
            {
                adjustedSpeed = thirdPersonFollowSpeed / (rb.mass * massScaling);
            }
            else
            {
                adjustedSpeed = followSpeed / (rb.mass * massScaling);
            }


            // Smooth out movement
            Vector3 velocity = (targetPos - transform.position) * adjustedSpeed;

            // Apply linear velocity to rigidbody
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, velocity, velocityLerp);
        }
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
}
