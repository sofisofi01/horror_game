using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;
using System.Collections.Generic;
using TMPro;

public class MenuManager : MonoBehaviour
{
public static string LastVideoName { get; set; } = "intro";

    [Header("Panels")]
    public GameObject creditsPanel;
    public GameObject settingsPanel;

    [Header("Settings Controls")]
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button backButton;

    [Header("Audio")]
    public AudioSource menuAudioSource;
    public AudioMixer mainMixer;

    [Header("Menu Buttons")]
    public Button playButton;
    public TextMeshProUGUI playButtonText;
    public Button quitButton;
    public Button settingsButton;
    public Button creditsButton;

    [Header("Credits Video")]
    public VideoPlayer creditsVideoPlayer;
    public VideoClip creditsVideoClip;

    private Resolution[] filteredResolutions;

    void Awake()
    {
        filteredResolutions = new Resolution[3];
        filteredResolutions[0] = new Resolution { width = 1920, height = 1080 };
        filteredResolutions[1] = new Resolution { width = 1280, height = 720 };
        filteredResolutions[2] = new Resolution { width = 854, height = 480 };
    }

    void OnEnable()
    {
        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
        backButton.onClick.AddListener(HideSettings);
        settingsButton.onClick.AddListener(ShowSettings);
        creditsButton.onClick.AddListener(ShowCredits);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        if (creditsVideoPlayer != null)
            creditsVideoPlayer.loopPointReached += OnCreditsVideoEnded;
    }

    void OnDisable()
    {
        playButton.onClick.RemoveListener(PlayGame);
        quitButton.onClick.RemoveListener(QuitGame);
        settingsButton.onClick.RemoveListener(ShowSettings);
        creditsButton.onClick.RemoveListener(ShowCredits);
        backButton.onClick.RemoveListener(HideSettings);

        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);

        if (creditsVideoPlayer != null)
            creditsVideoPlayer.loopPointReached -= OnCreditsVideoEnded;
    }

    void Start()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        SetupResolutionDropdown();

        fullscreenToggle.isOn = Screen.fullScreen;
        float musicVolume, sfxVolume;
        if (mainMixer.GetFloat("MusicVolume", out musicVolume))
            musicSlider.value = Mathf.Pow(10f, musicVolume / 20f);
        if (mainMixer.GetFloat("SFXVolume", out sfxVolume))
            sfxSlider.value = Mathf.Pow(10f, sfxVolume / 20f);
        UpdatePlayButton();
    }

    private void UpdatePlayButton()
    {
        bool hasProgress = LastVideoName != "intro";
        playButtonText.text = hasProgress ? "Продолжить" : "Играть";
    }

    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < filteredResolutions.Length; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            if (filteredResolutions[i].width == Screen.currentResolution.width &&
                filteredResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= filteredResolutions.Length) return;
        Resolution res = filteredResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");
        Application.Quit();
    }

    public void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        if (creditsPanel == null || creditsVideoPlayer == null || creditsVideoClip == null)
            return;

        creditsPanel.SetActive(true);
        creditsVideoPlayer.Stop();
        creditsVideoPlayer.clip = creditsVideoClip;
        creditsVideoPlayer.time = 0;
        creditsVideoPlayer.Prepare();
        creditsVideoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        creditsVideoPlayer.Play();
        creditsVideoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    public void HideCredits()
    {
        if (creditsVideoPlayer != null && creditsVideoPlayer.isPlaying)
            creditsVideoPlayer.Stop();

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    private void OnCreditsVideoEnded(VideoPlayer vp)
    {
        HideCredits();
    }
}
