using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro; // Make sure you are using TextMeshPro for the dropdowns

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer; // Assign your Master Mixer here

    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Menu Panels")]
    public GameObject mainMenu;
    public GameObject settingsMenu;

    private Resolution[] resolutions;

    void Start()
    {
        SetupResolutions();
        
        // Load existing quality level
        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    // res
    void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    // graphics qualit yset
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex); // 0 = low, 1 = high
    }

    // fullscreen set
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // audio
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
    }
    
    public void BackToMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}