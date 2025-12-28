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
            StartCoroutine(FadeToTrack(menuMusic));
        else if (scene.name == "MainScene")
            StartCoroutine(FadeToTrack(gameMusic));
    }

    IEnumerator FadeToTrack(AudioClip newClip)
    {
        if (audioSource.clip == newClip) yield break;

        // Fade out
        float currentTime = 0;
        float startVolume = audioSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / fadeDuration);
            yield return null;
        }

        // Change audio clip
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        currentTime = 0;
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            yield return null;
        }
    }
}