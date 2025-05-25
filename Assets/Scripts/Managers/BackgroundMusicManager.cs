using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    public AudioMixerGroup musicMixerGroup;
    private AudioSource audioSource;
    public AudioMixer mainMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = musicMixerGroup;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (audioSource.clip == newClip && audioSource.isPlaying)
            return;

        StartCoroutine(FadeAndSwitch(newClip));
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        if (mainMixer.GetFloat("MusicVolume", out float startDb))
        {
            float endDb = -80f;
            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                float curvedT = Mathf.SmoothStep(0f, 1f, t);
                float currentDb = Mathf.Lerp(startDb, endDb, curvedT);
                mainMixer.SetFloat("MusicVolume", currentDb);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            mainMixer.SetFloat("MusicVolume", endDb);
            audioSource.Stop();
            mainMixer.SetFloat("MusicVolume", startDb);
        }
    }


    private IEnumerator FadeAndSwitch(AudioClip newClip)
    {
        float duration = 1.5f;
        float startVolume = GetCurrentVolume();

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float vol = Mathf.Lerp(startVolume, 0f, t / duration);
            SetVolume(vol);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float vol = Mathf.Lerp(0f, startVolume, t / duration);
            SetVolume(vol);
            yield return null;
        }

        SetVolume(startVolume);
    }

    private float GetCurrentVolume()
    {
        if (mainMixer.GetFloat("MusicVolume", out float dbVolume))
        {
            return Mathf.Pow(10f, dbVolume / 20f);
        }

        return 1f;
    }

    private void SetVolume(float linearVolume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f);
    }
}
