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
            // If pause menu is active, deactivate it
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                playerInput.enabled = true;
                carInput.enabled = true;
                if (playerObject.GetComponent<CameraSwitcher>().IsFirstPerson || !carObject.GetComponent<CarEntrySystem>().isDriving)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if(!playerObject.GetComponent<CameraSwitcher>().IsFirstPerson && !carObject.GetComponent<CarEntrySystem>().isDriving)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
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
