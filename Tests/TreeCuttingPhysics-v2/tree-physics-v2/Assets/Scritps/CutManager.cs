using UnityEngine;

public class CutManager : MonoBehaviour
{
    public Camera cam;
    public float cutDistance = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Cut attempt");
            if (Physics.Raycast(cam.transform.position, cam.transform.forward,
                                out RaycastHit hit, cutDistance))
            {
                Debug.Log("Cut hit: " + hit.collider.name);
                LogSegment seg = hit.collider.GetComponent<LogSegment>();
                if (seg != null)
                    seg.treeChunk.Cut(seg);
            }
        }
    }
}
