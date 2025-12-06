using UnityEngine;
using UnityEngine.InputSystem; // Added for robust input support

public class CameraSwitcher : MonoBehaviour
{
    public GameObject firstPersonTarget;
    public GameObject thirdPersonTarget;
    public Camera playerCamera;

    public SkinnedMeshRenderer playerMesh;

    public bool IsFirstPerson { get; private set; }

    // New reference to handle seamless object switching
    public ObjectGrabbable currentHeldObject;

    void Start()
    {
        SwitchToFirstPerson(); // Set first person as the default view
    }

    void Update()
    {
        // If C is pressed, switch camera views
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (IsFirstPerson) SwitchToThirdPerson();
            else SwitchToFirstPerson();
        }
    }

    void SwitchToFirstPerson()
    {
        IsFirstPerson = true;

        // Enable first person target and disable third person target (camera position basically)
        firstPersonTarget.SetActive(true);
        thirdPersonTarget.SetActive(false);

        // Switch the camera view to the first person target
        playerCamera.transform.SetParent(firstPersonTarget.transform);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        playerCamera.transform.localEulerAngles = Vector3.zero;

        // Hide the player mesh in first person view
        playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Update held object immediately
        if (currentHeldObject != null) currentHeldObject.RecalculateGrabOffsets(playerCamera.transform);
    }

    void SwitchToThirdPerson()
    {
        IsFirstPerson = false;

        // Enable third person target and disable first person target (camera position basically)
        firstPersonTarget.SetActive(false);
        thirdPersonTarget.SetActive(true);

        // Switch the camera view to the third person target
        playerCamera.transform.SetParent(thirdPersonTarget.transform);
        playerCamera.transform.localPosition = Vector3.zero;
        playerCamera.transform.localRotation = Quaternion.identity;
        playerCamera.transform.localEulerAngles = Vector3.zero;

        // Show the player mesh in third person view
        playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        // Unlock + show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.None; // "teleports" mouse to center

        // Update held object immediately
        if (currentHeldObject != null) currentHeldObject.RecalculateGrabOffsets(playerCamera.transform);
    }
}