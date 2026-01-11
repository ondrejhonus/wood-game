using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public PlayerInput carInput; // /car controller input
    public PlayerInput playerInput; // player/ controller input
    public GameObject playerObject; // whole player/ prefab
    public GameObject carObject; // current /car prefab

    public GameObject settingsMenu;
    public GameObject mainMenu;

    void Start()
    {
        // pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                mainMenu.SetActive(true);
            }
        }
        if (carObject.GetComponent<CarEntrySystem>().isDriving && !pauseMenu.activeSelf && !settingsMenu.activeSelf && carInput.enabled == false)
        {
            carInput.enabled = true;
            playerInput.enabled = false;
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf && !settingsMenu.activeSelf)
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
            else if (!playerObject.GetComponent<CameraSwitcher>().IsFirstPerson && !carObject.GetComponent<CarEntrySystem>().isDriving)
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

    public void QuitToMainMenu(string sceneName)
    {
        Time.timeScale = 1f; // reset time scale
        SceneManager.LoadScene(sceneName);
    }

    public void OpenSettingsMenu()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
}
