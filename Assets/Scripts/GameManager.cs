using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public PlayerInput carInput; // /car controller input
    public PlayerInput playerInput; // player/ controller input
    public GameObject playerObject; // whole player/ prefab
    public GameObject carObject; // current /car prefab

    void Start()
    {
        // pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf)
            {
                // Deactivate pause menu
                pauseMenu.SetActive(false);
                if (playerObject.GetComponent<CameraSwitcher>().IsFirstPerson || !carObject.GetComponent<CarEntrySystem>().isDriving)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    playerInput.enabled = true;
                    carInput.enabled = false;
                }
                else if(!playerObject.GetComponent<CameraSwitcher>().IsFirstPerson && !carObject.GetComponent<CarEntrySystem>().isDriving)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    playerInput.enabled = true;
                    carInput.enabled = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    playerInput.enabled = false;
                    carInput.enabled = true;
                }

            }
            // If not active, activate pause menu
            else
            {
                pauseMenu.SetActive(true);
                playerInput.enabled = false;
                carInput.enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
