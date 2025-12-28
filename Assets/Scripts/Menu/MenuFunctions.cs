using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuFunctions : MonoBehaviour
{
    [Header("Menu screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [Header("Slider")]
    [SerializeField] private Slider loadingSlider;

    public void StartNewGame(string sceneName)
    {
        // load game scene without loading saved data
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName, false));
    }

    public void LoadGameButton(string sceneName)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        SaveManager.loadAfterSceneLoad = true;
        StartCoroutine(LoadSceneAsync(sceneName, true));
    }

    IEnumerator LoadSceneAsync(string sceneName, bool loadData)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, loadData ? LoadSceneMode.Single : LoadSceneMode.Additive);

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
        Debug.Log("Settings Menu Opened");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
