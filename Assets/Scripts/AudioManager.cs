using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audioSources;//0 is background, 1 is 
    private AudioSource currentActiveSource;
    public AudioSource introSource;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayAudio(0);//play background
    }

    public void PlayVideo(int index)
    {
        TransitionToAudio(audioSources[index]);
    }

    public void ReturnToBackground()
    {
        TransitionToAudio(audioSources[0]);
    }

    public void MuteBackground()
    {
        currentActiveSource.Stop();
    }

    public void SmoothBackgroundDown()
    {
        StartCoroutine(VolumeUp(currentActiveSource));
    }

    public void SmoothBackgroundUp()
    {
        StartCoroutine(VolumeDown(currentActiveSource));
    }

    private void TransitionToAudio(AudioSource newAudioSource)
    {
        if (currentActiveSource != null)
        {
            StartCoroutine(FadeAudioOut(currentActiveSource, 0.5f));
        }
        StartCoroutine(FadeAudioIn(newAudioSource, 0.5f));
        currentActiveSource = newAudioSource;
    }

    private IEnumerator FadeAudioOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeAudioIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0.2f;
        audioSource.volume = 0;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }
    }

    private IEnumerator VolumeUp(AudioSource audio)
    {
        float startVolume = audio.volume;
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            audio.volume = Mathf.Lerp(startVolume, startVolume * 3, t / 0.5f);
            yield return null;
        }
    }

    private IEnumerator VolumeDown(AudioSource audio)
    {
        float startVolume = audio.volume;
        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            audio.volume = Mathf.Lerp(startVolume, startVolume/3, t / 0.5f);
            yield return null;
        }
    }

    void PlayAudio(int index)
    {
        AudioSource audio = audioSources[index];

        if (currentActiveSource != null && currentActiveSource.isPlaying)
        {
            currentActiveSource.Stop();
        }

        //play the new one
        currentActiveSource = audio;
        currentActiveSource.Play();
    }
}