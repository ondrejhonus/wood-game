using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public GameObject firstPersonTarget;
    public GameObject thirdPersonTarget;
    public Camera playerCamera;

    public SkinnedMeshRenderer playerMesh;

    public bool IsFirstPerson { get; private set; }

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
        // Set the camera's parent to the first person target
        playerCamera.transform.SetParent(firstPersonTarget.transform);
        // Reset local position and rotation to align with the character
        playerCamera.transform.localPosition = Vector3.zero;
        // Reset local rotation to align with the character
        playerCamera.transform.localRotation = Quaternion.identity;

        // Hide the player mesh in first person view
        playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void SwitchToThirdPerson()
    {
        IsFirstPerson = false;

        // Enable third person target and disable first person target (camera position basically)
        firstPersonTarget.SetActive(false);
        thirdPersonTarget.SetActive(true);

        // Switch the camera view to the third person target
        playerCamera.transform.SetParent(thirdPersonTarget.transform);
        // Reset local position and rotation to align with the character
        playerCamera.transform.localPosition = Vector3.zero;
        // Reset local rotation to align with the character
        playerCamera.transform.localRotation = Quaternion.identity;

        // Show the player mesh in third person view
        playerMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        // Unlock + show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Center the cursor on switch
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.None; // "teleports" mouse to center
    }
}
