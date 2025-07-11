using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycastExample : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                // Example: check for ChoppableLog component
                ChoppableLog log = hit.collider.GetComponent<ChoppableLog>();
                if (log != null)
                {
                    log.HandleChop(hit.point);
                }
            }
        }
    }
}