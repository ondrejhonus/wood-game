using UnityEngine;

public class LeafController : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        // Ensure components exist or add them
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();

        // Disable physics so they dont fall down
        rb.isKinematic = true;
        // Disable collider so it doesn't interfere with chopping raycasts
        col.enabled = false; 
    }

    public void DropLeaves()
    {
        // Detach from the tree
        transform.SetParent(null);

        // Enable physics
        rb.isKinematic = false;
        col.enabled = true;

        // Add a little push so they don't fall perfectly straight down
        rb.AddForce(Vector3.up * 2f + Random.insideUnitSphere * 2f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        // 4. Destroy leaves after 10 seconds
        Destroy(gameObject, 10f);
    }
}