using NUnit.Framework;
using UnityEngine;

public class PlayerPickupDrop : MonoBehaviour
{
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private Transform objectGrabPointTransform; // point in front of camera where object will be held
    [SerializeField] private LayerMask PickUpLayerMask;
    [SerializeField] private float firstPersonDistance = 5f;
    [SerializeField] private float thirdPersonDistance = 16f; // needs to be further away, coz the camera is behind the player
    [SerializeField] private GameObject grabEffectPrefab;
    private GameObject grabPointInstance;
    private Vector3 grabPointOffset;


    private float grabDistance;
    private bool grabDistanceInitialized = false;
    private bool lastIsFP = true; // to track changes in camera mode

    private ObjectGrabbable objectGrabbable;

    private void HandleGrabbedObjectDestroyed(ObjectGrabbable obj)
    {
        if (grabPointInstance != null)
        {
            Destroy(grabPointInstance);
            grabPointInstance = null;
        }
        objectGrabbable = null;
    }

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

        if (objectGrabbable != null)
        {
            // Dont allow camera switch while holding an object
            cameraSwitcher.enabled = false;
        }
        else
        {
            cameraSwitcher.enabled = true;
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

        if (Input.GetKey(KeyCode.E))
        {
            if (objectGrabbable == null)
            {
                Ray ray = isFP ? currentCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)) : currentCam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, grabDistance, PickUpLayerMask))
                {
                    ShopItem shopItem = hit.transform.GetComponent<ShopItem>();
                    if (shopItem != null)
                    {
                        shopItem.OnPickedUp(); // notify shop iten that its been moved by th e player
                    }
                    if (hit.transform.TryGetComponent(out objectGrabbable) && (!shopItem.isPurchased || shopItem == null))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform, hit.point);

                        if (grabEffectPrefab != null)
                        {
                            // Store the local offset when grabbing
                            grabPointOffset = hit.point - objectGrabbable.transform.position;

                            // spawn the grab point effect
                            grabPointInstance = Instantiate(grabEffectPrefab, hit.point, Quaternion.identity);
                        }
                        objectGrabbable.OnDestroyed += HandleGrabbedObjectDestroyed;
                    }
                }
                // Dont allow camera switch while holding an object
                cameraSwitcher.enabled = false;
                
            }
        }
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (objectGrabbable != null)
                {
                    // Drop the object
                    objectGrabbable.Drop();
                    // Unsubscribe from the event
                    objectGrabbable.OnDestroyed -= HandleGrabbedObjectDestroyed;
                    objectGrabbable = null;
                    // reset grab distance according to current camera mode
                    grabDistance = isFP ? firstPersonDistance : thirdPersonDistance;
                    // Allow camera switching again
                    cameraSwitcher.enabled = true;

                    // Destroy grab effect when dropping
                    if (grabEffectPrefab != null && grabPointInstance != null)
                    {
                        Destroy(grabPointInstance);
                        grabPointInstance = null;
                    }
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

                // Update grab effect position
                if (grabEffectPrefab != null && grabPointInstance != null)
                {
                    grabPointInstance.transform.position = objectGrabbable.transform.position + grabPointOffset;
                }
            }
        }
    }