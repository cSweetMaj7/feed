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

    private BeatMap currentBeatmap;
    private Priority_Queue.SimplePriorityQueue<int> beatQueue;
    private float beatDeltatime; // I am seconds, don't use me, use
    private int nextBeat;
    private bool musicPlaying;
    private string musicClip;

    private GameManager gameManager;

    private void Update()
    {
        if(musicPlaying)
        {
            if (!MusicSource.isPlaying)
            {
                musicLooper();
            }

            // check if we've reached the next beat in the queue
            beatDeltatime += Time.deltaTime;

            if(getBeatDeltaTime() > nextBeat)
            {
                // hit beat, fire and setup next one
                beat();
                if(beatQueue.Count > 0)
                {
                    nextBeat = beatQueue.Dequeue();
                } else
                {
                    // if we reached the end, just reset the beat delta time and start over
                    beatDeltatime = 0;
                    nextBeat = 0;
                    // don't forget to re-write the queue
                    string jsonString = LoadResourceTextfile("beatmap_stage_music_slow");
                    currentBeatmap = JsonUtility.FromJson<BeatMap>(jsonString);
                    for (int i = 0; i < currentBeatmap.beatmap.Length; i++)
                    {
                        beatQueue.Enqueue(currentBeatmap.beatmap[i], currentBeatmap.beatmap[i]);
                    }
                }
            }
        }
    }

    public void beat() // called on downbeats based on beat map
    {
        if(!gameManager)
        {
            gameManager = GetComponentInParent<GameManager>();
        }

        if(nextBeat != 0) //don't count the 0 beat
        {
            //Debug.Log("BEAT! " + nextBeat.ToString());
            gameManager.bounceBackground();
        }
    }

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
        MusicSource.loop = false;
        musicClip = "beatmap_stage_music_slow";
        Debug.Log("Starting beat");
        startBeat();
        Debug.Log("Starting music");
        MusicSource.Play();
    }

    public void startMusicFast()
    {
        MusicSource.Stop();
        MusicSource.clip = fastStageMusic;
        MusicSource.loop = false;
        musicClip = "beatmap_stage_music_fast";
        Debug.Log("Starting beat");
        startBeat();
        Debug.Log("Starting music");
        MusicSource.Play();
    }

    // event that fires when a music clip ends
    // important to have this so we can re-sync the beat track
    private void musicLooper()
    {
        if(musicClip == "beatmap_stage_music_slow")
        {
            startMusicSlow();
        } else if (musicClip == "beatmap_stage_music_fast")
        {
            startMusicFast();
        }
    }

    private void startBeat()
    {
        // write the beatmap
        string jsonString = LoadResourceTextfile(musicClip);
        currentBeatmap = JsonUtility.FromJson<BeatMap>(jsonString);

        // use the beatmap to generate the beat queue, apply offset if there is one
        beatQueue = new Priority_Queue.SimplePriorityQueue<int>(); // make an empty queue
        // enqueue all the beats
        for(int i = 0; i < currentBeatmap.beatmap.Length; i++)
        {
            beatQueue.Enqueue(currentBeatmap.beatmap[i] + currentBeatmap.offset, currentBeatmap.beatmap[i] + currentBeatmap.offset);
        }

        // reset beat delta time
        beatDeltatime = 0;
        nextBeat = 0;

        musicPlaying = true; // this starts the update incrementing delta time
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

    public static string LoadResourceTextfile(string name)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(name);
        return targetFile.text;
    }

    private int getBeatDeltaTime() // use me to get MS
    {
        return Mathf.FloorToInt(beatDeltatime * 1000);
    }
}

[System.Serializable]
public class BeatMap
{
    public string title;
    public int offset;
    public int[] beatmap;
}
