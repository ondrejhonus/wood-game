using NUnit.Framework;
using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask PickUpLayerMask;
    [SerializeField] private float firstPersonDistance = 3f;
    [SerializeField] private float thirdPersonDistance = 7f; // needs to be further away, coz the camera is behind the player

    private ObjectGrabbable objectGrabbable;

    void Update()
    {
        Camera currentCam = cameraSwitcher.playerCamera;

        bool isFP = cameraSwitcher.IsFirstPerson;

        float grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;


        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                Ray ray = isFP ? currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)) : currentCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, PickUpLayerMask))
                {
                    if (hit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            }
            else
            {
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }

        // Update grab point position while holding an object
        if (objectGrabbable != null)
        {
            Ray grabRay = cameraSwitcher.IsFirstPerson
                ? currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0))
                : currentCam.ScreenPointToRay(Input.mousePosition);

            objectGrabPointTransform.position = grabRay.origin + grabRay.direction * grabDistance;
        }
    }
}
