using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public float fadeDuration = 1.2f; // How long the fade takes

    private AudioSource audioSource;
    private float targetMaxVolume; // To remember the user's volume preference
    public GameObject loadingScreen;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            targetMaxVolume = audioSource.volume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Debug.Log("MainMenu music loaded");
            StartCoroutine(FadeToTrack(menuMusic));
        }
        else if (scene.name == "MainScene")
        {
            Debug.Log("MainScene music loaded");
            StartCoroutine(FadeToTrack(gameMusic));
        }
        else if (loadingScreen != null && loadingScreen.activeSelf)
        {
            StartCoroutine(FadeToTrack(null)); // No music, eg during loading
            Debug.Log("Loading screen active, no music");
        }
    }

    IEnumerator FadeToTrack(AudioClip newClip)
    {
        // 1. If the clip is already playing, don't restart it
        if (audioSource.clip == newClip && audioSource.isPlaying) yield break;

        // 2. Fade out current music
        float startVolume = audioSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.Stop();

        // 3. Switch and Play
        audioSource.clip = newClip;
        if (newClip != null)
        {
            audioSource.Play();
            // 4. Fade in to the original target volume
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, targetMaxVolume, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = targetMaxVolume;
        }
    }
}