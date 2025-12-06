using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ChopPoint
{
    public float chopPercentage;
    public int currentHits;
    public Transform progressPivot;
    public Transform progressBar;
}

public class ChoppableLog : MonoBehaviour
{
    [Header("Chopping Settings")]
    public GameObject logPiecePrefab;
    public int hitsToChop = 6;
    public float minPieceLength = 0.01f;

    public float chopRange = 3.5f; 
    public LayerMask chopLayer = ~0; 

    public LeafController connectedLeaves;

    private Transform progressBar;
    private Transform progressPivot;
    private BoxCollider boxCollider;
    private List<ChopPoint> chopPoints = new List<ChopPoint>();
    public PlayerInventory playerInventory;
    public AudioSource audioSource;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        // Find progress bar cube
        Transform pivot = transform.Find("ProgressBarPivot");
        if (pivot != null)
        {
            progressPivot = pivot;
            progressBar = pivot.Find("ProgressBar");
            if (progressBar != null)
            {
                // Keep active but scale X = 0 so its invisible at first
                Vector3 s = progressBar.localScale;
                progressBar.localScale = new Vector3(0f, s.y, s.z);
            }
        }
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerInventory = player.GetComponent<PlayerInventory>();
        }
    }

    private void Update()
    {
        // Check for mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            // Only check raycast against chopLayer, 100f is max distance
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, chopLayer))
            {
                // Check if player is within range of log
                if (hit.collider.gameObject == gameObject && Vector3.Distance(playerInventory.transform.position, hit.point) <= chopRange)
                {
                    Debug.Log("Chop point hit: " + hit.point);
                    GameObject heldItem = playerInventory.GetSelectedItem();
                    if (heldItem != null && heldItem.CompareTag("Axe"))
                    {
                        HandleChop(hit.point);
                        if (audioSource != null)
                        {
                            // Play chop sound with random pitch
                            audioSource.Play();
                            audioSource.pitch = Random.Range(0.8f, 1.2f);
                        }
                    }
                }
            }
        }
    }

    public void HandleChop(Vector3 hitPoint)
    {
        // If the tree is stil growing, break it in one chop
        if (transform.localScale.y < 2.0f)
        {
            SplitLog(new ChopPoint { chopPercentage = 0.5f });
            return;
        }

        Vector3 localHit = transform.InverseTransformPoint(hitPoint);
        float colliderCenterY = boxCollider.center.y;
        float colliderExtentY = boxCollider.size.y * 0.5f;
        float normalizedY = (localHit.y - (colliderCenterY - colliderExtentY)) / boxCollider.size.y;
        float chopPercentage = Mathf.Clamp01(normalizedY);

        ChopPoint target = null;
        foreach (var cp in chopPoints)
        {
            if (Mathf.Abs(cp.chopPercentage - chopPercentage) < 0.05f)
            {
                target = cp;
                break;
            }
        }

        if (target == null)
        {
            ChopPoint newCp = new ChopPoint();
            newCp.chopPercentage = chopPercentage;
            newCp.currentHits = 0;

            Transform pivotTemplate = transform.Find("ProgressBarPivot");
            // check if template exists
            if (pivotTemplate != null)
            {
                Transform pivotInstance = Instantiate(pivotTemplate, transform);
                pivotInstance.localPosition = new Vector3(
                    pivotTemplate.localPosition.x,
                    localHit.y,
                    pivotTemplate.localPosition.z
                );

                // Set up progress bar references
                newCp.progressPivot = pivotInstance;
                // Find progress bar
                newCp.progressBar = pivotInstance.Find("ProgressBar");
                // Set initial scale
                Vector3 s = newCp.progressBar.localScale;
                // Set X scale to 0 to make it invisible at first
                newCp.progressBar.localScale = new Vector3(0f, s.y, s.z);
                // ACtivate it
                newCp.progressBar.gameObject.SetActive(true);
            }

            chopPoints.Add(newCp);
            target = newCp;
        }

        target.currentHits++;
        if (target.progressBar != null) UpdateProgressBar(target);

        if (target.currentHits >= hitsToChop)
            SplitLog(target);
    }


    void UpdateProgressBar(ChopPoint cp)
    {
        // Scale progress bar based on hits
        float progress = (float)cp.currentHits / hitsToChop;
        Vector3 s = cp.progressBar.localScale;
        s.x = progress;
        cp.progressBar.localScale = s;
    }

    // Indicate if the log is planted in the ground, if yes, then its a tree stump
    public bool isPlanted = false;

    void SplitLog(ChopPoint cp)
    {
        if (cp.progressBar != null)
            cp.progressBar.gameObject.SetActive(false);

        float totalLength = boxCollider.size.y * transform.localScale.y;
        float cutY = totalLength * cp.chopPercentage;

        float part1Length = cutY;
        float part2Length = totalLength - cutY;

        if (part1Length < minPieceLength || part2Length < minPieceLength)
        {
            // Debug.LogWarning("One part would be too small. Aborting split.");
            return;
        }

        if (connectedLeaves != null)
        {
            connectedLeaves.DropLeaves();
            connectedLeaves = null; // Clear reference so we don't call it twice
        }

        // Spawn pieces new logic
        CreateLogPiece(part1Length, -totalLength * 0.5f + part1Length * 0.5f, isPlanted); 
        CreateLogPiece(part2Length, -totalLength * 0.5f + part1Length + part2Length * 0.5f, false);

        Destroy(gameObject);
    }

    void CreateLogPiece(float length, float localYOffset, bool planted)
    {
        GameObject piece = Instantiate(logPiecePrefab);
        piece.transform.position = transform.position + transform.up * localYOffset;
        piece.transform.rotation = transform.rotation;
        // Set scale
        piece.transform.localScale = new Vector3(transform.localScale.x, length, transform.localScale.z);

        Rigidbody pieceRb = piece.GetComponent<Rigidbody>();
        if (pieceRb != null)
        {
            float width = piece.transform.localScale.x;
            float mass = length * Mathf.Pow(width, 2) * 8f;
            pieceRb.mass = mass;
            pieceRb.isKinematic = planted; // If planted, make kinematic (no physics)
        }

        Transform pivot = piece.transform.Find("ProgressBarPivot");
        if (pivot != null)
        {
            Transform bar = pivot.Find("ProgressBar");
            if (bar != null)
            {
                Vector3 s = bar.localScale;
                bar.localScale = new Vector3(0f, s.y, s.z);
                bar.gameObject.SetActive(false);
            }
        }

        ChoppableLog ch = piece.GetComponent<ChoppableLog>();
        if (ch == null)
        {
            ch = piece.AddComponent<ChoppableLog>();
        }

        // Pass all the settings
        ch.logPiecePrefab = logPiecePrefab;
        ch.hitsToChop = hitsToChop;
        ch.minPieceLength = minPieceLength;
        ch.playerInventory = playerInventory;
        ch.audioSource = audioSource;
        ch.chopLayer = chopLayer; // pass layer to cut in
        ch.chopRange = chopRange; // pass range to cut in

        // set if the piece is planted
        ch.isPlanted = planted;

        GameObject playerArmature = GameObject.FindWithTag("Player");
        if (playerArmature != null)
        {
            ObjectGrabbable grabbable = piece.GetComponent<ObjectGrabbable>();

            if (planted)
            {
                if (grabbable != null)
                    Destroy(grabbable);
                // Start regrow routine
                if (TreeGenerator.instance != null && TreeGenerator.instance.treesCanRegrow)
                {
                    ch.StartCoroutine(ch.RegrowRoutine());
                }
            }
            else
            {
                CameraSwitcher camSwitcher = playerArmature.GetComponent<CameraSwitcher>();
                if (camSwitcher != null && grabbable != null)
                    grabbable.cameraSwitcher = camSwitcher;
            }
        }
    }

    public IEnumerator RegrowRoutine()
    {
        // Wait for some time before regrowing a new tree
        yield return new WaitForSeconds(TreeGenerator.instance.timeBeforeRegrow);

        // Transform.position is the center of the log piece 
        // We subtract Half Height to find the exact point on the dirt where the tree should regrow
        float halfHeight = transform.localScale.y * 0.5f;
        // Calculate Ground Position
        Vector3 groundPos = transform.position - (transform.up * halfHeight);

        // Spawn new tree at this position with growth animation
        if (TreeGenerator.instance != null)
        {
            TreeGenerator.instance.SpawnTree(groundPos, true);
        }

        // Destroy the remaining stump
        Destroy(gameObject);
    }
}