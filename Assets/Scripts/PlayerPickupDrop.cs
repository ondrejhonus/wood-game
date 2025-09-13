using NUnit.Framework;
using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask PickUpLayerMask;
    [SerializeField] private float firstPersonDistance = 3f;
    [SerializeField] private float thirdPersonDistance = 7f; // needs to be further away, coz the camera is behind the player

    private float grabDistance;
    private bool grabDistanceInitialized = false;
    private bool lastIsFP = true; // to track changes in camera mode

    private ObjectGrabbable objectGrabbable;

    void Update()
    {
        Camera currentCam = cameraSwitcher.playerCamera;

        bool isFP = cameraSwitcher.IsFirstPerson;

        // Initialize grabDistance only once
        if (!grabDistanceInitialized)
        {
            grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;
            grabDistanceInitialized = true;
        }

        // Adjust grab distance when switching between first and third person
        if (lastIsFP != isFP)
        {
            if (isFP)
            {
                // Switching from TP to FP: keep distance but decrease by 3, clamp to FP range
                grabDistance = Mathf.Clamp(grabDistance - 3f, 1f, 8f);
            }
            else
            {
                // Switching from FP to TP: keep distance but increase by 3, clamp to TP range
                grabDistance = Mathf.Clamp(grabDistance + 3f, 1f, 15f);
            }
            lastIsFP = isFP;
        }

        // Reduce grab distance on wheel down, increase on wheel up
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            grabDistance += scroll * .8f;
            if (isFP)
            {
                grabDistance = Mathf.Clamp(grabDistance, 1f, 8f);
            }
            else
            {
                grabDistance = Mathf.Clamp(grabDistance, 1f, 15f);
            }
        }

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
                grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;
            }
        }

        // Update grab point position while holding an object
        if (objectGrabbable != null)
        {
            Ray grabRay;

            if (cameraSwitcher.IsFirstPerson)
            {
                grabRay = currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            }
            else
            {
                // TODO: Fix issue where when i switch to TP, the object doesnt go to the mouse position, but it goes to top right until i move the mouse
                Vector3 mousePos = Input.mousePosition;
                grabRay = currentCam.ScreenPointToRay(mousePos);
            }

            objectGrabPointTransform.position = grabRay.origin + grabRay.direction * grabDistance;
        }
    }
}
