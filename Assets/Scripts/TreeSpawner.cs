using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject logPrefab;
    public int treeCount = 50;

    [Header("Tree Size")]
    public Vector2 trunkHeightRange = new Vector2(3f, 7f);
    public Vector2 trunkWidthRange = new Vector2(0.2f, 0.5f);

    [Header("Leaves Settings")]
    public GameObject leavesPrefab;
    [Tooltip("The global size you want the leaves to be, regardless of trunk size")]
    public float targetLeafSize = 1.5f; 

    [Header("References for ChoppableLog")]
    public int hitsToChop = 3;
    public float minPieceLength = 0.3f;
    public PlayerInventory playerInventory;
    public AudioSource audioSource;

    [Header("Raycast")]
    public LayerMask whatIsGround;

    private BoxCollider area;

    void Start()
    {
        area = GetComponent<BoxCollider>();
        GenerateTrees();
    }

    void GenerateTrees()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 pos = RandomPointInArea();
            if (Physics.Raycast(pos + Vector3.up * 30f, Vector3.down, out RaycastHit hit, 60f, whatIsGround))
            {
                SpawnTree(hit.point);
            }
        }
    }

    void SpawnTree(Vector3 position)
    {
        // 1. Create the tree log
        GameObject tree = Instantiate(logPrefab, position, Quaternion.identity);

        float height = Random.Range(trunkHeightRange.x, trunkHeightRange.y);
        float width = Random.Range(trunkWidthRange.x, trunkWidthRange.y);

        tree.transform.localScale = new Vector3(width, height, width);

        float halfHeight = height * 0.5f;
        tree.transform.position = position + Vector3.up * (halfHeight - 0.001f);
        
        // Random Rotation
        tree.transform.rotation = Quaternion.Euler(Random.Range(4f, 8f), Random.Range(0f, 360f), 0f);

        // 2. Setup Physics
        Rigidbody rb = tree.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float mass = height * Mathf.Pow(width, 2) * 8f;
            rb.mass = mass;
            rb.isKinematic = true;
        }

        // 3. Setup ChoppableLog
        ChoppableLog ch = tree.GetComponent<ChoppableLog>();
        if (ch == null) ch = tree.AddComponent<ChoppableLog>();

        ch.logPiecePrefab = logPrefab;
        ch.hitsToChop = hitsToChop;
        ch.minPieceLength = minPieceLength;
        ch.playerInventory = playerInventory;
        ch.audioSource = audioSource;

        // 4. Fix Object Grabbable & UI (Your existing logic)
        GameObject playerArmature = GameObject.FindWithTag("Player");
        if (playerArmature != null)
        {
            ObjectGrabbable grab = tree.GetComponent<ObjectGrabbable>();
            CameraSwitcher cam = playerArmature.GetComponent<CameraSwitcher>();
            if (grab != null && cam != null) grab.cameraSwitcher = cam;
        }

        // ---------------------------------------------------------
        // 5. LEAVES LOGIC (Updated)
        // ---------------------------------------------------------
        if (leavesPrefab != null)
        {
            Vector3 leafPos = position + Vector3.up * (height + 0.5f);
            
            // Instantiate as child of tree so it moves/rotates with it
            GameObject leaves = Instantiate(leavesPrefab, leafPos, Quaternion.identity, tree.transform);

            // FIX SCALING: Calculate inverse scale
            // We want the global size to be 'targetLeafSize', but the parent is scaling it.
            // ChildLocal = DesiredGlobal / ParentLocal
            Vector3 inverseScale = new Vector3(
                targetLeafSize / width, 
                targetLeafSize / height, 
                targetLeafSize / width
            );
            
            leaves.transform.localScale = inverseScale;
            leaves.transform.localRotation = Quaternion.identity;

            // Add the controller so we can drop it later
            LeafController leafCtrl = leaves.AddComponent<LeafController>();
            
            // IMPORTANT: Pass this reference to your ChoppableLog!
            // You must add a 'public LeafController connectedLeaves;' variable to your ChoppableLog script.
            ch.connectedLeaves = leafCtrl;
        }
    }

    Vector3 RandomPointInArea()
    {
        Vector3 c = transform.position + area.center;
        Vector3 s = area.size;
        return new Vector3(
            Random.Range(c.x - s.x / 2, c.x + s.x / 2),
            c.y,
            Random.Range(c.z - s.z / 2, c.z + s.z / 2)
        );
    }
}