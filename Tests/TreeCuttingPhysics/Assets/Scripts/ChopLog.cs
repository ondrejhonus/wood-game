using UnityEngine;
using UnityEngine.InputSystem;

public class ChoppableLog : MonoBehaviour
{
[Header("Chopping Settings")]
public GameObject logPiecePrefab;
public int hitsToChop = 3;
public float minPieceLength = 0.05f;
private int currentHitCount = 0;
private float chopPercentage = -1f;

private BoxCollider boxCollider;

private void Awake()
{
    boxCollider = GetComponent<BoxCollider>();
    if (!boxCollider)
        Debug.LogError("ChoppableLog: Missing BoxCollider.");
}

private void Update()
{
    if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 2f);

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
        float logLength = bounds.size.z;
        float localHitZ = transform.InverseTransformPoint(hitPoint).z;

        float normalizedZ = (localHitZ + (logLength * 0.5f)) / logLength;
        normalizedZ = Mathf.Clamp01(normalizedZ);

        chopPercentage = normalizedZ;

        Debug.Log($"Chop Percentage: {chopPercentage * 100f:F1}% along log length.");
    }

    currentHitCount++;
    Debug.Log("Hit count: " + currentHitCount);

    if (currentHitCount >= hitsToChop)
    {
        SplitLog();
    }
}

void SplitLog()
{
    float totalLength = boxCollider.size.z * transform.localScale.z;
    float cutZ = totalLength * chopPercentage;

    float part1Length = cutZ;
    float part2Length = totalLength - cutZ;

    if (part1Length < minPieceLength || part2Length < minPieceLength)
    {
        Debug.LogWarning("One part would be too small. Aborting split.");
        return;
    }

    CreateLogPiece(part1Length, -totalLength * 0.5f + part1Length * 0.5f);
    CreateLogPiece(part2Length, -totalLength * 0.5f + part1Length + part2Length * 0.5f);

    Destroy(gameObject);
}

void CreateLogPiece(float length, float localZOffset)
{
    GameObject piece = Instantiate(logPiecePrefab);
    piece.transform.position = transform.position + transform.forward * localZOffset;
    piece.transform.rotation = transform.rotation;
    piece.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, length);

    Rigidbody rb = piece.GetComponent<Rigidbody>();
    if (rb != null) rb.isKinematic = false;
}
}