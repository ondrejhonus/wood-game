using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuFunctions : MonoBehaviour
{
    [Header("Menu screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsMenu.activeSelf)
        {
            // if in settings menu, go back to main menu
            OpenSettings();
        }
    }

    public void StartNewGame(string sceneName)
    {
        // load game scene without loading saved data
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadGameButton(string sceneName)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SaveManager.loadAfterSceneLoad = true;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }

    public void OpenSettings()
    {
        // hide current menu and show settings menu
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        Debug.Log("Settings Menu Opened");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
