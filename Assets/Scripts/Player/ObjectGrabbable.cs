using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform grabPoint;
    private Vector3 cameraOffset; 
    private Vector3 localAnchor;
    private Quaternion rotationOffset;
    [SerializeField] private float followSpeed = 5f; 
    [SerializeField] private float thirdPersonFollowSpeed = 3f; 
    [SerializeField] private float massScaling = 1f; 
    [SerializeField] private float velocityLerp = 0.2f; 
    [SerializeField] public CameraSwitcher cameraSwitcher;
    public System.Action<ObjectGrabbable> OnDestroyed;

    private Collider myCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        }
    }

    public void Grab(Transform grabPointTransform, Vector3 hitPoint)
    {
        grabPoint = grabPointTransform;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero; // Reset velocity
        
        if(cameraSwitcher != null) cameraSwitcher.currentHeldObject = this;

        // Ignore player collision to prevent jitters/explosion
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null && player.GetComponent<Collider>() != null) 
            Physics.IgnoreCollision(myCollider, player.GetComponent<Collider>(), true);

        // Store precise offsets relative to Camera and Object
        cameraOffset = grabPoint.InverseTransformPoint(hitPoint);
        localAnchor = transform.InverseTransformPoint(hitPoint);
        rotationOffset = Quaternion.Inverse(grabPoint.rotation) * transform.rotation;
    }

    public void Drop()
    {
        grabPoint = null;
        rb.useGravity = true;
        
        if(cameraSwitcher != null) cameraSwitcher.currentHeldObject = null;

        // Restore collision
        GameObject player = GameObject.FindWithTag("Player");
        if(player != null && player.GetComponent<Collider>() != null) 
            Physics.IgnoreCollision(myCollider, player.GetComponent<Collider>(), false);
    }

    // New method to keep object steady during camera switch
    public void RecalculateGrabOffsets(Transform newCamera)
    {
        if (grabPoint == null) return;
        
        // Find where the object anchor is currently in the world
        Vector3 currentWorldPos = transform.TransformPoint(localAnchor);
        
        grabPoint = newCamera;
        
        // Calculate new offset relative to the new camera so it stays visually in place
        cameraOffset = grabPoint.InverseTransformPoint(currentWorldPos);
        rotationOffset = Quaternion.Inverse(grabPoint.rotation) * transform.rotation;
    }

    private void FixedUpdate()
    {
        if (grabPoint != null)
        {
            // Calculate Target Position & Rotation
            Vector3 targetPos = grabPoint.TransformPoint(cameraOffset);
            Quaternion targetRot = grabPoint.rotation * rotationOffset;

            // Adjust target to respect the local anchor (the handle)
            Vector3 rotatedAnchor = targetRot * localAnchor;
            Vector3 finalPos = targetPos - rotatedAnchor;

            // Apply Rotation
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 20f * Time.fixedDeltaTime));

            // Adjust speed based on mass
            float adjustedSpeed;
            float safeMass = Mathf.Max(0.1f, rb.mass * massScaling); // prevent divide by zero

            if (!cameraSwitcher.IsFirstPerson)
            {
                adjustedSpeed = thirdPersonFollowSpeed / safeMass;
            }
            else
            {
                adjustedSpeed = followSpeed / safeMass;
            }

            // Smooth out movement
            Vector3 velocity = (finalPos - transform.position) * adjustedSpeed;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, velocity, velocityLerp);
        }
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
        if(cameraSwitcher != null && cameraSwitcher.currentHeldObject == this)
            cameraSwitcher.currentHeldObject = null;
    }
}