using UnityEngine;
using UnityEngine.InputSystem; // <--- NEW: Required for PlayerInput

public class CarEntrySystem : MonoBehaviour
{
    [Header("Player References")]
    public GameObject playerObject;       // Whole player prefab
    public GameObject playerCameraObject; // Main Camera

    [Header("Car References")]
    public PlayerInput carController;   // Car controller player input
    public PlayerInput playerInput; // player controller input
    public GameObject carCamerasParent;   // Camera mount point
    public Transform exitPoint;           // Where the player appears when exiting

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.F; // Key to get in/out
    public float interactionDist = 3.5f;

    private bool isDriving = false;

void Start()
    {
        // Disable CarController script so player input is a priority
        if(carController != null) 
        {
            carController.enabled = false; 
        }

        if(carCamerasParent != null) 
        {
            carCamerasParent.SetActive(false);
        }

        isDriving = false;

        if(playerObject != null) 
        {
            playerObject.SetActive(true);
        }
        if(playerCameraObject != null) 
        {
            playerCameraObject.SetActive(true);
        }

    }

    void Update()
    {
        // Simple toggle input
        if (Input.GetKeyDown(interactKey))
        {
            if (isDriving)
            {
                ExitCar();
            }
            else
            {
                // Check distance to car collider
                float dist = Vector3.Distance(transform.position, playerObject.transform.position);
                if (dist < interactionDist)
                {
                    EnterCar();
                }
            }
        }
    }

    void EnterCar()
    {
        isDriving = true;

        // Disable player character
        playerObject.SetActive(false);

        playerInput.enabled = false; // DIsable player input system
        
        // Turn off player camera mount point
        playerCameraObject.SetActive(false);

        // Turn on car camera mount point
        carCamerasParent.SetActive(true);

        // Enable car control input system
        carController.enabled = true;
    }

void ExitCar()
    {
        isDriving = false;

        // Teleport player to exit point
        playerObject.transform.position = exitPoint.position;
        playerObject.transform.rotation = exitPoint.rotation;

        // Turn off car camera mount point
        carCamerasParent.SetActive(false);

        // Turn on player camera mount point
        playerCameraObject.SetActive(true);

        // Enable player character
        playerObject.SetActive(true);

        // Disable car control input system
        carController.enabled = false;

        // Enable player input system
        playerInput.enabled = true;
    }
}