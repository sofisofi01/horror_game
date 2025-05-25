using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FoodMiniGameManager : MonoBehaviour
{
    [Header("Food Buttons")]
    public List<Button> foodButtons;

    [Header("Choice Buttons")]
    public Button choiceButton1;
    public Button choiceButton2;
    public AudioClip choiceSound1;
    public AudioClip choiceSound2;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip introSound;
    public List<AudioClip> foodSounds;

    [Header("Next Scene")]
    public string nextSceneKey;

    private int clickedCount = 0;
    private VideoStoryManager storyManager;

    void Start()
    {
        storyManager = FindObjectOfType<VideoStoryManager>();
        HideAllButtons(); 
    }

    public void StartGame()
    {
        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        HideAllButtons();

        if (introSound != null)
        {
            audioSource.clip = introSound;
            audioSource.Play();
            yield return new WaitForSeconds(introSound.length);
        }

        clickedCount = 0;
        SetupFoodButtons();
        ShowButtonsWithFade(foodButtons);
    }

    void SetupFoodButtons()
    {
        for (int i = 0; i < foodButtons.Count; i++)
        {
            int index = i;
            Button btn = foodButtons[i];

            btn.gameObject.SetActive(true);
            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnFoodButtonClicked(index));

            SetupCanvasGroup(btn);
        }
    }

    void OnFoodButtonClicked(int index)
    {
        Button btn = foodButtons[index];
        btn.interactable = false;

        if (index < foodSounds.Count && foodSounds[index] != null)
        {
            audioSource.clip = foodSounds[index];
            audioSource.Play();
        }

        StartCoroutine(WaitAndHideFoodButton(btn, audioSource.clip != null ? audioSource.clip.length : 0.5f));
    }

    IEnumerator WaitAndHideFoodButton(Button btn, float delay)
    {
        yield return new WaitForSeconds(delay);

        var cg = btn.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        clickedCount++;

        if (clickedCount >= foodButtons.Count)
        {
            yield return new WaitForSeconds(0.5f);
            ShowChoiceButtons();
        }
    }

    void ShowChoiceButtons()
    {
        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);

        SetupCanvasGroup(choiceButton1);
        SetupCanvasGroup(choiceButton2);

        var cg1 = choiceButton1.GetComponent<CanvasGroup>();
        var cg2 = choiceButton2.GetComponent<CanvasGroup>();
        cg1.alpha = cg2.alpha = 1;
        cg1.interactable = cg2.interactable = true;
        cg1.blocksRaycasts = cg2.blocksRaycasts = true;

        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        choiceButton1.onClick.AddListener(() => OnChoiceButtonClicked(choiceSound1));
        choiceButton2.onClick.AddListener(() => OnChoiceButtonClicked(choiceSound2));
    }

    void OnChoiceButtonClicked(AudioClip clip)
    {
        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);

        StartCoroutine(PlayChoiceAndProceed(clip));
    }

    IEnumerator PlayChoiceAndProceed(AudioClip clip)
    {
    if (clip != null)
    {
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
    }

    storyManager.PlayScene(nextSceneKey);
    }

    void ShowButtonsWithFade(List<Button> buttons)
    {
        StartCoroutine(FadeInButtons(buttons));
    }

    IEnumerator FadeInButtons(List<Button> buttons)
    {
        float duration = 1f;
        float elapsed = 0f;

        foreach (var btn in buttons)
        {
            SetupCanvasGroup(btn);
            btn.gameObject.SetActive(true);
        }

        while (elapsed < duration)
        {
            float alpha = elapsed / duration;

            foreach (var btn in buttons)
            {
                var cg = btn.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = alpha;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (var btn in buttons)
        {
            var cg = btn.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
    }

    void HideAllButtons()
    {
        foreach (var btn in foodButtons)
        {
            btn.gameObject.SetActive(false);
        }

        choiceButton1.gameObject.SetActive(false);
        choiceButton2.gameObject.SetActive(false);
    }

    void SetupCanvasGroup(Button btn)
    {
        var cg = btn.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = btn.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}
