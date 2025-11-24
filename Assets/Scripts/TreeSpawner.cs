using UnityEngine;
using System.Collections.Generic; // Required for Lists

public class TreeGenerator : MonoBehaviour
{
    public GameObject logPrefab;
    public int treeCount = 50;
    
    [Tooltip("Minimum distance between trees")]
    public float minTreeSpacing = 2.5f; 

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
        List<Vector3> spawnedPositions = new List<Vector3>();
        int currentCount = 0;
        int safetyCounter = 0; // Prevence if area is too small

        // Keep trying until theres enough trees or theres no more space
        while (currentCount < treeCount && safetyCounter < treeCount * 10)
        {
            safetyCounter++;
            Vector3 pos = RandomPointInArea();

            if (Physics.Raycast(pos + Vector3.up * 30f, Vector3.down, out RaycastHit hit, 60f, whatIsGround))
            {
                Vector3 potentialSpot = hit.point;

                bool isTooClose = false;
                foreach (Vector3 existingPos in spawnedPositions)
                {
                    // If distance is less than minSpacing, its niot valid
                    if (Vector3.Distance(potentialSpot, existingPos) < minTreeSpacing)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                // Only spawn if it's not too close to others
                if (!isTooClose)
                {
                    SpawnTree(potentialSpot);
                    spawnedPositions.Add(potentialSpot);
                    currentCount++;
                }
            }
        }

        if (currentCount < treeCount)
        {
            Debug.LogWarning($"Only managed to spawn {currentCount} trees. Area might be too small for the spacing settings.");
        }
    }

    void SpawnTree(Vector3 position)
    {
        // Create the tree log
        GameObject tree = Instantiate(logPrefab, position, Quaternion.identity);

        float height = Random.Range(trunkHeightRange.x, trunkHeightRange.y);
        float width = Random.Range(trunkWidthRange.x, trunkWidthRange.y);

        tree.transform.localScale = new Vector3(width, height, width);

        float halfHeight = height * 0.5f;
        tree.transform.position = position + Vector3.up * (halfHeight - 0.02f);
        
        // Random Rotation
        tree.transform.rotation = Quaternion.Euler(Random.Range(4f, 8f), Random.Range(0f, 360f), 0f);

        // Setup Physics
        Rigidbody rb = tree.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float mass = height * Mathf.Pow(width, 2) * 8f;
            rb.mass = mass;
            rb.isKinematic = true;
        }

        // Setup ChoppableLog
        ChoppableLog ch = tree.GetComponent<ChoppableLog>();
        if (ch == null) ch = tree.AddComponent<ChoppableLog>();

        ch.logPiecePrefab = logPrefab;
        ch.hitsToChop = hitsToChop;
        ch.minPieceLength = minPieceLength;
        ch.playerInventory = playerInventory;
        ch.audioSource = audioSource;

        ch.isPlanted = true; // Mark as planted into the ground

        // Setup Grabbable
        GameObject playerArmature = GameObject.FindWithTag("Player");
        if (playerArmature != null)
        {
            ObjectGrabbable grab = tree.GetComponent<ObjectGrabbable>();
            CameraSwitcher cam = playerArmature.GetComponent<CameraSwitcher>();
            if (grab != null && cam != null) grab.cameraSwitcher = cam;
        }

        // Create Leaves
        if (leavesPrefab != null)
        {
            Vector3 leafPos = position + Vector3.up * (height + 0.5f);
            
            // Set as a child so it takes the tree's position and rotation
            GameObject leaves = Instantiate(leavesPrefab, leafPos, Quaternion.identity, tree.transform);

            // Adjust scale to match log size
            Vector3 inverseScale = new Vector3(
                targetLeafSize / width, 
                targetLeafSize / height, 
                targetLeafSize / width
            );
            
            leaves.transform.localScale = inverseScale;
            leaves.transform.localRotation = Quaternion.identity;

            LeafController leafCtrl = leaves.AddComponent<LeafController>();
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