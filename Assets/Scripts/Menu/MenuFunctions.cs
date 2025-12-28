using UnityEngine;

public class MenuFunctions : MonoBehaviour


{
    public void LoadGameAndPlay()
    {
        // load game scene and load saved data from savemanager
        SaveManager.loadAfterSceneLoad = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void StartNewGame()
    {
        // load game scene without loading saved data
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void OpenSettings()
    {
        // hide current menu and show settings menu
        Debug.Log("Settings Menu Opened");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
