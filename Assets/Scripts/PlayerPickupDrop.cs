using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask PickUpLayerMask;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float pickUpDistance = 3f;
            Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, PickUpLayerMask);
        }
    }
}
