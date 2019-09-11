using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource[] EffectsSources;
    public AudioSource MusicSource;

    public AudioClip slowStageMusic;
    public AudioClip fastStageMusic;
    public AudioClip endStageMusic;
    public AudioClip aim;
    public AudioClip ballBounceBarrier;
    public AudioClip ballSplit;
    public AudioClip ballTap;
    public AudioClip ballDestroy;
    public AudioClip ballReturn;
    public AudioClip ballBounceTarget;
    public AudioClip targetDestroyed;
    public AudioClip gameOverSuccess;
    public AudioClip gameOverFailed;
    public AudioClip stageAdvance;

    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        //DontDestroyOnLoad(gameObject);
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip)
    {
        playEffectsSource(clip);
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    public void startMusicSlow()
    {
        MusicSource.Stop();
        MusicSource.clip = slowStageMusic;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void startMusicFast()
    {
        MusicSource.Stop();
        MusicSource.clip = fastStageMusic;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void startEndMusic()
    {
        MusicSource.Stop();
        MusicSource.clip = endStageMusic;
        MusicSource.loop = false;
        MusicSource.Play();
    }

    public void playTargetDestroyed()
    {
        playEffectsSource(targetDestroyed);
    }

    public void playStageAdvance()
    {
        playEffectsSource(stageAdvance);
    }

    public void playBallBounceBarrier()
    {
        playEffectsSource(ballBounceBarrier);
    }

    public void playBallTap()
    {
        playEffectsSource(ballTap);
    }

    private void playEffectsSource(AudioClip clip)
    {
        AudioSource targetSource = null;
        // find an available fx sorce
        for(int i = 0; i < EffectsSources.Length; i++)
        {
            if(!EffectsSources[i].isPlaying)
            {
                targetSource = EffectsSources[i];
            }
        }

        if(targetSource != null)
        {
            targetSource.clip = clip;
            targetSource.Play();
        } else
        {
            // audio got dropped because all effects sources are playing
            Debug.LogWarning("Dropped FX, all sources are playing");
        }
    }

}