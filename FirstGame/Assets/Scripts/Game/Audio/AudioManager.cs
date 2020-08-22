using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {Master, Sfx, Music };

    public float masterVolPercent { get; private set; }
    public float sfxVolPercent { get; private set; }
    public float musicVolPercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            library = GetComponent<SoundLibrary>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSources = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusicSources.AddComponent<AudioSource>();
                newMusicSources.transform.parent = transform;
            }
            GameObject newSfx2DSource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if (FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }

            masterVolPercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolPercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolPercent = PlayerPrefs.GetFloat("music vol", 1);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolPercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolPercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolPercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolPercent * masterVolPercent;
        musicSources[1].volume = musicVolPercent * masterVolPercent;

        PlayerPrefs.SetFloat("master vol", masterVolPercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolPercent);
        PlayerPrefs.SetFloat("music vol", musicVolPercent);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolPercent * masterVolPercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolPercent * masterVolPercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolPercent * masterVolPercent, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp (musicVolPercent * masterVolPercent, 0, percent);
            yield return null;
        }
    }
}
