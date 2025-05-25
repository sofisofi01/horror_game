using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class VideoStoryManager : MonoBehaviour
{
    [Header("Video Components")]
    public RawImage screenImage;
    public VideoPlayer videoPlayer;

    [Header("One-shot Sounds")]
    public AudioClip sceneStartSound1;
    public string sceneForStartSound1;
    public AudioClip sceneStartSound2;
    public string sceneForStartSound2;
    public AudioClip sceneStartSound3;
    public string sceneForStartSound3;
    public AudioClip sceneStartSound4;
    public string sceneForStartSound4;
    public AudioClip sceneStartSound5;
    public string sceneForStartSound5;
    public AudioClip sceneStartSound6;
    public string sceneForStartSound6;

    [Header("Background Music")]
    public AudioClip sound1Clip;
    public List<string> scenesUsingSound1;
    public AudioClip sound2Clip;
    public List<string> scenesUsingSound2;
    public AudioClip sound3Clip;
    public List<string> scenesUsingSound3;
    public AudioClip sound4Clip;
    public List<string> scenesUsingSound4;
    public AudioSource additionalSfxSource;

    [Header("Scene List")]
    public List<VideoScene> scenes;

    private Dictionary<string, VideoScene> sceneDict;
    private VideoScene currentScene;

    private Coroutine showChoicesCoroutine;
    private Coroutine hideChoicesCoroutine;

    [Header("Pause")]
    public GameObject pauseOverlay;
    public Button pauseButton;
    public Button menuButton;

    private bool isPaused = false;

    void Start()
    {
        sceneDict = scenes.ToDictionary(s => s.sceneName);
        videoPlayer.loopPointReached += OnVideoEnded;
        pauseOverlay?.SetActive(false);
        pauseButton?.onClick.AddListener(TogglePause);
        menuButton.onClick.AddListener(GoToMenu);
        menuButton.gameObject.SetActive(true);
        HideAllButtonsAtStart();
        PlayScene(MenuManager.LastVideoName);
    }

    void HideAllButtonsAtStart()
    {
        foreach (var scene in scenes)
        {
            if (scene.choices == null) continue;

            foreach (var choice in scene.choices)
            {
                if (choice.button != null)
                {
                    choice.button.gameObject.SetActive(false);

                    var cg = choice.button.GetComponent<CanvasGroup>();
                    if (cg == null)
                        cg = choice.button.gameObject.AddComponent<CanvasGroup>();

                    cg.alpha = 0;
                }
            }
        }
    }

    public void PlayScene(string sceneKey)
    {
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioVolume(0, 1f);
        if (!sceneDict.TryGetValue(sceneKey, out var scene))
        {
            Debug.LogError($"Сцена '{sceneKey}' не найдена!");
            return;
        }

        
        AudioClip clipToPlay = null;

        if (scenesUsingSound1.Contains(scene.sceneName))
            clipToPlay = sound1Clip;
        else if (scenesUsingSound2.Contains(scene.sceneName))
            clipToPlay = sound2Clip;
        else if (scenesUsingSound3.Contains(scene.sceneName))
            clipToPlay = sound3Clip;
        else if (scenesUsingSound4.Contains(scene.sceneName))
            clipToPlay = sound4Clip;

        if (clipToPlay != null)
        {
            BackgroundMusicManager.Instance.PlayMusic(clipToPlay);
        }
        else
        {
            BackgroundMusicManager.Instance.FadeOutMusic(10f);
        }

        if (scene.sceneName == sceneForStartSound1 && sceneStartSound1 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound1);
        }
        else if (scene.sceneName == sceneForStartSound2 && sceneStartSound2 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound2);
        }
        else if (scene.sceneName == sceneForStartSound3 && sceneStartSound3 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound3);
        }
        else if (scene.sceneName == sceneForStartSound4 && sceneStartSound4 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound4);
        }
        else if (scene.sceneName == sceneForStartSound5 && sceneStartSound5 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound5);
        }
        else if (scene.sceneName == sceneForStartSound6 && sceneStartSound6 != null)
        {
            additionalSfxSource.PlayOneShot(sceneStartSound6);
        }


        if (scene.sceneName == "FoodLoop")
        {
            var foodGame = FindObjectOfType<FoodMiniGameManager>();
            if (foodGame != null)
            {
                foodGame.StartGame();
            }
        }

        currentScene = scene;

        videoPlayer.clip = scene.videoClip;
        videoPlayer.isLooping = scene.isLooping;
        videoPlayer.Play();

        scene.onSceneEnter?.Invoke();
        if (hideChoicesCoroutine != null)
            StopCoroutine(hideChoicesCoroutine);

        hideChoicesCoroutine = StartCoroutine(HideChoicesWithFade());

        if (scene.waitForChoices && scene.choices != null && scene.choices.Count > 0)
        {
            if (showChoicesCoroutine != null)
                StopCoroutine(showChoicesCoroutine);

            showChoicesCoroutine = StartCoroutine(ShowChoicesWithFade(scene.choices, scene.choiceButtonDelay));
        }
    }

    IEnumerator HideChoicesWithFade()
    {
        float fadeDuration = 0.5f;
        float elapsed = 0f;

        List<Choice> choicesToHide = currentScene?.choices ?? new List<Choice>();

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);

            foreach (var choice in choicesToHide)
            {
                if (choice.button != null)
                {
                    var cg = choice.button.GetComponent<CanvasGroup>();
                    if (cg != null)
                        cg.alpha = alpha;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var choice in choicesToHide)
        {
            if (choice.button != null)
            {
                var cg = choice.button.GetComponent<CanvasGroup>();
                if (cg != null)
                    cg.alpha = 0;

                choice.button.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ShowChoicesWithFade(List<Choice> choices, float delay)
    {
        yield return new WaitForSeconds(delay);

        float fadeDuration = 1f;

        foreach (var choice in choices)
        {
            if (choice.button == null) continue;

            var cg = choice.button.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = choice.button.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0;
            choice.button.gameObject.SetActive(true);

            choice.button.onClick.RemoveAllListeners();
            choice.button.onClick.AddListener(() => OnChoiceSelected(choice));

            var textComp = choice.button.GetComponentInChildren<TMP_Text>();
            if (textComp != null)
                textComp.text = choice.label;
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);

            foreach (var choice in choices)
            {
                if (choice.button == null) continue;

                var cg = choice.button.GetComponent<CanvasGroup>();
                if (cg != null)
                    cg.alpha = alpha;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var choice in choices)
        {
            if (choice.button == null) continue;

            var cg = choice.button.GetComponent<CanvasGroup>();
            if (cg != null)
                cg.alpha = 1;
        }
    }

    void OnVideoEnded(VideoPlayer vp)
    {

        if (currentScene.isEndingScene)
        {
            SceneManager.LoadScene("Menu");
            return;
        }

        if (!currentScene.waitForChoices && !string.IsNullOrEmpty(currentScene.autoNextScene))
        {
            PlayScene(currentScene.autoNextScene);
        }
    }

    void OnChoiceSelected(Choice choice)
    {
        if (showChoicesCoroutine != null)
        {
            StopCoroutine(showChoicesCoroutine);
            showChoicesCoroutine = null;
        }

        if (hideChoicesCoroutine != null)
        {
            StopCoroutine(hideChoicesCoroutine);
            hideChoicesCoroutine = null;
        }

        StartCoroutine(HideChoicesThenPlayNext(choice.nextScene));
    }

    IEnumerator HideChoicesThenPlayNext(string nextScene)
    {
        yield return HideChoicesWithFade();
        PlayScene(nextScene);
    }


    void GoToMenu()
    {
        if (currentScene != null)
        {
            MenuManager.LastVideoName = currentScene.sceneName;
        }
        SceneManager.LoadScene("Menu");
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            videoPlayer.Pause();
            pauseOverlay?.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            videoPlayer.Play();
            pauseOverlay?.SetActive(false);
        }
    }
    
    public IEnumerator LoadSceneWithMusicFade(string sceneName, float fadeDuration)
    {
        BackgroundMusicManager.Instance.FadeOutMusic(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);
    }

}
