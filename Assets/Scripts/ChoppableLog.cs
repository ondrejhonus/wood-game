using System.Collections.Generic;
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
    private Transform progressBar; // this should be the **cube child of the pivot**
    private Transform progressPivot; // the pivot that will be moved to the click position
    private BoxCollider boxCollider;
    private List<ChopPoint> chopPoints = new List<ChopPoint>();
    public PlayerInventory playerInventory; // Assign in Inspector or find at runtime

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        // Find progress bar cube (child of the pivot)
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
            playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        // Check for mouse click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            // Convert mouse position to a ray that is casted into the scene
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    // Only allow chop if holding an axe
                    GameObject heldItem = playerInventory.GetSelectedItem();
                    // Check if the held item is an axe
                    if (heldItem != null && heldItem.CompareTag("Axe"))
                    {
                        HandleChop(hit.point);
                    }
                    else
                    {
                        Debug.Log("You need an axe to chop this log!");
                    }
                }
            }
        }
    }

    public void HandleChop(Vector3 hitPoint)
    {
        // Convert hit point to normalized Y position
        Vector3 localHit = transform.InverseTransformPoint(hitPoint);
        float colliderCenterY = boxCollider.center.y;
        float colliderExtentY = boxCollider.size.y * 0.5f;
        float normalizedY = (localHit.y - (colliderCenterY - colliderExtentY)) / boxCollider.size.y;
        float chopPercentage = Mathf.Clamp01(normalizedY);

        // Check if near an existing chop point
        ChopPoint target = null;
        foreach (var cp in chopPoints)
        {
            if (Mathf.Abs(cp.chopPercentage - chopPercentage) < 0.05f) // 5% tolerance
            {
                target = cp;
                break;
            }
        }

        // If not found, create a new chop point
        if (target == null)
        {
            ChopPoint newCp = new ChopPoint();
            newCp.chopPercentage = chopPercentage;
            newCp.currentHits = 0;

            // Create a new pivot + bar
            Transform pivotTemplate = transform.Find("ProgressBarPivot");
            Transform pivotInstance = Instantiate(pivotTemplate, transform);
            pivotInstance.localPosition = new Vector3(
                pivotTemplate.localPosition.x,
                localHit.y,
                pivotTemplate.localPosition.z
            );

            newCp.progressPivot = pivotInstance;
            newCp.progressBar = pivotInstance.Find("ProgressBar");
            Vector3 s = newCp.progressBar.localScale;
            newCp.progressBar.localScale = new Vector3(0f, s.y, s.z);
            newCp.progressBar.gameObject.SetActive(true);

            chopPoints.Add(newCp);
            target = newCp;
        }

        // Apply hit
        target.currentHits++;
        UpdateProgressBar(target);

        // Split if done
        if (target.currentHits >= hitsToChop)
            SplitLog(target);
    }


    void UpdateProgressBar(ChopPoint cp)
    {
        float progress = (float)cp.currentHits / hitsToChop;
        Vector3 s = cp.progressBar.localScale;
        s.x = progress;
        cp.progressBar.localScale = s;
    }

    void SplitLog(ChopPoint cp)
    {
        // Hide progress bar
        if (cp.progressBar != null)
            cp.progressBar.gameObject.SetActive(false);

        // Calculate lengths of the two new pieces
        float totalLength = boxCollider.size.y * transform.localScale.y;
        float cutY = totalLength * cp.chopPercentage;

        float part1Length = cutY;
        float part2Length = totalLength - cutY;

        // Make sure pieces are not too small
        if (part1Length < minPieceLength || part2Length < minPieceLength)
        {
            Debug.LogWarning("One part would be too small. Aborting split.");
            return;
        }

        // Spawn pieces
        CreateLogPiece(part1Length, -totalLength * 0.5f + part1Length * 0.5f);
        CreateLogPiece(part2Length, -totalLength * 0.5f + part1Length + part2Length * 0.5f);

        // Destroy original log
        Destroy(gameObject);
    }

    void CreateLogPiece(float length, float localYOffset)
    {
        // Create a new log piece
        GameObject piece = Instantiate(logPiecePrefab);
        // Position it correctly
        piece.transform.position = transform.position + transform.up * localYOffset;
        // Match rotation and scale (scale.y will be adjusted below)
        piece.transform.rotation = transform.rotation;
        // Match X and Z scale, set Y scale to the length of the piece
        piece.transform.localScale = new Vector3(transform.localScale.x, length, transform.localScale.z);
        Rigidbody pieceRb = piece.GetComponent<Rigidbody>();
        if (pieceRb != null)
        {
            // Calculate mass of the log (length * width^2)
            float width = piece.transform.localScale.x;
            float mass = length * Mathf.Pow(width, 2);
            pieceRb.mass = mass;
        }

        // Add physics to the new piece with Rigidbody
        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // Disable progress bar on spawned pieces
        Transform pivot = piece.transform.Find("ProgressBarPivot");
        if (pivot != null)
        {
            Transform bar = pivot.Find("ProgressBar");
            if (bar != null)
            {
                // Reset scale to 0, so it's invisible at first
                Vector3 s = bar.localScale;
                bar.localScale = new Vector3(0f, s.y, s.z);
                bar.gameObject.SetActive(false);
            }
        }

        // Add ChoppableLog script to pieces so they can be chopped furthermore
        if (piece.GetComponent<ChoppableLog>() == null)
        {
            ChoppableLog ch = piece.AddComponent<ChoppableLog>();
            ch.logPiecePrefab = logPiecePrefab;
            ch.hitsToChop = hitsToChop;
            ch.minPieceLength = minPieceLength;
        }

        // Find the player armature in the scene
        GameObject playerArmature = GameObject.FindWithTag("Player");
        if (playerArmature != null)
        {
            ObjectGrabbable grabbable = piece.GetComponent<ObjectGrabbable>();
            CameraSwitcher camSwitcher = playerArmature.GetComponent<CameraSwitcher>();
            // Set the player armature as the camera switcher
            if (camSwitcher != null)
                grabbable.cameraSwitcher = camSwitcher;

        }
    }
}
