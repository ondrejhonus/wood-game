using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour
{
    public static TreeGenerator instance;

    public GameObject logPrefab;
    public int treeCount = 50;
    public float minTreeSpacing = 2.5f;

    [Header("Regrowth Settings")]
    public bool treesCanRegrow = true;
    public float timeBeforeRegrow = 300f;   // Time stump sits there (300 = 5 minutes)
    public float growthDuration = 300f;    // Time to grow a full tree (300 = 5 minutes)

    [Header("Tree Size")]
    public Vector2 trunkHeightRange = new Vector2(3f, 7f);
    public Vector2 trunkWidthRange = new Vector2(0.2f, 0.5f);
    public GameObject leavesPrefab;
    public float targetLeafSize = 1.5f;

    [Header("References")]
    public int hitsToChop = 3;
    public float minPieceLength = 0.3f;
    public PlayerInventory playerInventory;
    public AudioSource audioSource;
    public LayerMask whatIsGround;
    private BoxCollider area;

    void Awake() { instance = this; }

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

    public void SpawnTree(Vector3 position, bool animateGrowth = false)
    {
        GameObject tree = Instantiate(logPrefab, position, Quaternion.identity);

        // Calculate random final tree size
        float finalHeight = Random.Range(trunkHeightRange.x, trunkHeightRange.y);
        float finalWidth = Random.Range(trunkWidthRange.x, trunkWidthRange.y);

        // Set the sapling size and position
        if (animateGrowth)
        {
            // Start tiny!
            tree.transform.localScale = Vector3.zero; 
            // Position at ground level
            tree.transform.position = position; 
        }
        else
        {
            // no animation, set final size directly, this happens at the start of the game
            tree.transform.localScale = new Vector3(finalWidth, finalHeight, finalWidth);
            float halfHeight = finalHeight * 0.5f;
            tree.transform.position = position + Vector3.up * (halfHeight - 0.001f);
        }

        // Random Y Rotation
        tree.transform.rotation = Quaternion.Euler(Random.Range(4f, 8f), Random.Range(0f, 360f), 0f);

        // Setup Physics
        Rigidbody rb = tree.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate final mass
            float mass = finalHeight * Mathf.Pow(finalWidth, 2) * 8f;
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
        ch.isPlanted = true; // IMPORTANT

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
            // Position relative to the FINAL height
            // This is so the leaves grow with the trunk during animation
            Vector3 localLeafPos = Vector3.up * 0.5f; // Top of the cube is 0.5f because pivot is in center, so we only go half way up
            
            GameObject leaves = Instantiate(leavesPrefab, tree.transform);
            leaves.transform.localPosition = localLeafPos; 
            leaves.transform.localRotation = Quaternion.identity;

            // Calculate inverse scale based on FINAL dimensions
            Vector3 inverseScale = new Vector3(
                targetLeafSize / finalWidth, 
                targetLeafSize / finalHeight, 
                targetLeafSize / finalWidth
            );
            leaves.transform.localScale = inverseScale;

            LeafController leafCtrl = leaves.AddComponent<LeafController>();
            ch.connectedLeaves = leafCtrl;
        }

        // 3. Start Animation if requested
        if (animateGrowth)
        {
            StartCoroutine(GrowTreeRoutine(tree.transform, finalWidth, finalHeight, position));
        }
    }

    // Grow animation routine
    IEnumerator GrowTreeRoutine(Transform tree, float targetWidth, float targetHeight, Vector3 groundPos)
    {
        float timer = 0f;

        while (timer < growthDuration)
        {
            timer += Time.deltaTime;
            float t = timer / growthDuration;
            
            // Smoothstep makes it grow fast at start and end, slow in middle
            t = Mathf.SmoothStep(0f, 1f, t); 

            float currentHeight = Mathf.Lerp(0.1f, targetHeight, t);
            float currentWidth = Mathf.Lerp(0.05f, targetWidth, t);

            if (tree == null) yield break; // Check if tree was destroyed mid-animation

            // Apply Scale at each frame
            tree.localScale = new Vector3(currentWidth, currentHeight, currentWidth);

            // Apply Position
            // explanation: Because the pivot is in the CENTER, 
            // the position must be Ground + Half of Current Height.
            tree.position = groundPos + Vector3.up * (currentHeight * 0.5f);

            yield return null;
        }

        // set final values to ensure accuracy
        if (tree != null)
        {
            tree.localScale = new Vector3(targetWidth, targetHeight, targetWidth);
            tree.position = groundPos + Vector3.up * (targetHeight * 0.5f);
        }
    }
Vector3 RandomPointInArea()
    {
        Vector3 c = transform.position + area.center;
        Vector3 s = area.size;
        return new Vector3(Random.Range(c.x - s.x / 2, c.x + s.x / 2), c.y, Random.Range(c.z - s.z / 2, c.z + s.z / 2));
    }
}