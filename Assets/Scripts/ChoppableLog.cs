using UnityEngine;
using UnityEngine.InputSystem;

public class ChoppableLog : MonoBehaviour
{
    [Header("Chopping Settings")]
    public GameObject logPiecePrefab; // prefab must have ChoppableLog attached
    public int hitsToChop = 6;
    public float minPieceLength = 0.01f;

    private int currentHitCount = 0;
    private float chopPercentage = -1f;

    private Transform progressBar; // this should be the **cube child of the pivot**
    private Transform progressPivot; // the pivot that will be moved to the click position
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider)
            Debug.LogError("ChoppableLog: Missing BoxCollider.");

        // Find progress bar cube (child of the pivot)
        Transform pivot = transform.Find("ProgressBarPivot");
        if (pivot != null)
        {
            progressPivot = pivot;
            progressBar = pivot.Find("ProgressBar");
            if (progressBar != null)
            {
                // Keep active but scale X = 0 so it’s invisible initially
                Vector3 s = progressBar.localScale;
                progressBar.localScale = new Vector3(0f, s.y, s.z);
            }
        }
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    HandleChop(hit.point);
                }
            }
        }
    }

    public void HandleChop(Vector3 hitPoint)
    {
        if (chopPercentage < 0f)
        {
            Bounds bounds = boxCollider.bounds;
            float logLength = bounds.size.y;

            // Calculate hit point in local space relative to the collider center
            Vector3 localHit = transform.InverseTransformPoint(hitPoint);
            float colliderCenterY = boxCollider.center.y;
            float colliderExtentY = boxCollider.size.y * 0.5f;

            // Normalize hit position between 0 (bottom) and 1 (top) of the collider
            float normalizedY = (localHit.y - (colliderCenterY - colliderExtentY)) / boxCollider.size.y;
            chopPercentage = Mathf.Clamp01(normalizedY);

            Debug.Log($"Chop Percentage: {chopPercentage * 100f:F1}% along log length (Y).");

            // Move the progress bar pivot to the clicked local Y so the bar appears at click
            if (progressPivot != null)
            {
                Vector3 p = progressPivot.localPosition;
                p.y = localHit.y;
                progressPivot.localPosition = p;
            }

            // Show progress bar on first hit
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                Vector3 s = progressBar.localScale;
                progressBar.localScale = new Vector3(0f, s.y, s.z); // zeroed
            }
        }

        currentHitCount++;
        UpdateProgressBar();

        if (currentHitCount >= hitsToChop)
        {
            SplitLog();
        }
    }

    void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            float progress = (float)currentHitCount / hitsToChop;
            Vector3 s = progressBar.localScale;
            s.x = progress; // scale along X, grows from pivot
            progressBar.localScale = s;
        }
    }

    void SplitLog()
    {
        // Hide progress bar immediately
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        float totalLength = boxCollider.size.y * transform.localScale.y;
        float cutY = totalLength * chopPercentage;

        float part1Length = cutY;
        float part2Length = totalLength - cutY;

        if (part1Length < minPieceLength || part2Length < minPieceLength)
        {
            Debug.LogWarning("One part would be too small. Aborting split.");
            return;
        }

        CreateLogPiece(part1Length, -totalLength * 0.5f + part1Length * 0.5f);
        CreateLogPiece(part2Length, -totalLength * 0.5f + part1Length + part2Length * 0.5f);

        Destroy(gameObject);
    }

    void CreateLogPiece(float length, float localYOffset)
    {
        GameObject piece = Instantiate(logPiecePrefab);
        piece.transform.position = transform.position + transform.up * localYOffset;
        piece.transform.rotation = transform.rotation;
        piece.transform.localScale = new Vector3(transform.localScale.x, length, transform.localScale.z);

        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        // Disable progress bar on spawned pieces
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

        // Add ChoppableLog script to pieces so they can be chopped further
        if (piece.GetComponent<ChoppableLog>() == null)
        {
            ChoppableLog ch = piece.AddComponent<ChoppableLog>();
            ch.logPiecePrefab = logPiecePrefab;
            ch.hitsToChop = hitsToChop;
            ch.minPieceLength = minPieceLength;
        }
    }
}
