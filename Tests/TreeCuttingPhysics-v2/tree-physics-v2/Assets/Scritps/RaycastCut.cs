using UnityEngine;

public class PlayerAxe : MonoBehaviour
{
    public Camera cam;
    public float reach = 10f;
    public LayerMask cutMask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, reach, cutMask))
            {
                TreeChunk chunk = hit.collider.GetComponentInParent<TreeChunk>();
                if (chunk != null)
                {
                    chunk.CutAtPoint(hit.point);
                }
            }
        }
    }
}