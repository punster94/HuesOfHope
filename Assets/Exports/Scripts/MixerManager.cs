using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour {

    public AudioMixer footstepMixer;
    public AudioMixer guitarMixer;
    public AudioMixer masterMixer;
    public int fadeDuration;
    public AudioSource[] guitarScoreSequences;
    public AudioSource[] otherScoreSequences;

    private int totalTrackCount;
    private int happinessBucketSize;
    private int currentHappinessBucket;
    private int guitarUnlockCount;

    private StateManagerBehaviour stateManagerBehaviour;
    private PlayerController playerController;

    enum PoseState
    {
        STATIONARY,
        WALKING,
        RUNNING
    };
    PoseState currentPoseState;

    const int RUN_THRESHOLD = 5;



	// Use this for initialization
	void Start ()
    {
        stateManagerBehaviour = GameObject.Find("StateManager").GetComponent<StateManagerBehaviour>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        totalTrackCount = guitarScoreSequences.Length + otherScoreSequences.Length;
        happinessBucketSize = 100 / totalTrackCount;
        currentHappinessBucket = 0;

        guitarUnlockCount = 0;

        foreach (AudioSource source in guitarScoreSequences)
        {
            source.mute = false;
            source.volume = 0;
        }

        foreach (AudioSource source in otherScoreSequences)
        {
            source.mute = false;
            source.volume = 0;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        float newHappiness = stateManagerBehaviour.currentDepressionPercentage() * 100;

        UpdateScore(newHappiness);
        UpdateFootsteps();
        UpdateMixer(newHappiness);
    }

    private void UpdateScore(float newHappiness)
    {
        int newBucket = (int) newHappiness / happinessBucketSize;

        if (newBucket == currentHappinessBucket)    return;

        currentHappinessBucket = newBucket;

        int tracksLeftToProcess = currentHappinessBucket + 1;

        for (int i = 0; i < guitarScoreSequences.Length; ++i)
        {
            if (i < guitarUnlockCount && tracksLeftToProcess > 0)
            {
                RequestPlayScore(guitarScoreSequences[i]);
                tracksLeftToProcess--;
            }
            else
            {
                RequestStopScore(guitarScoreSequences[i]);
            }
        }

        for (int i = 0; i < otherScoreSequences.Length; ++i)
        {
            if (tracksLeftToProcess > 0)
            {
                RequestPlayScore(otherScoreSequences[i]);
                tracksLeftToProcess--;
            }
            else
            {
                RequestStopScore(otherScoreSequences[i]);
            }
        }
    }

    private void UpdateFootsteps()
    {
        Vector3 translation = playerController.getLastTranslation();
        PoseState newPoseState = PoseState.STATIONARY;

        if (translation.magnitude > RUN_THRESHOLD)
        {
            newPoseState = PoseState.RUNNING;
        }
        else if (translation.magnitude > 0)
        {
            newPoseState = PoseState.WALKING;
        }

        if (newPoseState != currentPoseState)
        {
            currentPoseState = newPoseState;
            switch (currentPoseState)
            {
                case PoseState.STATIONARY: footstepMixer.FindSnapshot("Stationary").TransitionTo(0.2f); break;
                case PoseState.WALKING: footstepMixer.FindSnapshot("Walking").TransitionTo(0.2f); break;
                case PoseState.RUNNING: footstepMixer.FindSnapshot("Running").TransitionTo(0.2f); break;
            }
        }
    }

    void UpdateMixer(float newHappiness)
    {
        // Dry mix varies from -1,000mb to 0
        // Do we want to do it logarithmically? For now let's just do linear
        float dryMix = -7500 + ((newHappiness * 7500) / 100);
        guitarMixer.SetFloat("MainBusReverbDry", dryMix);

        // We want to vary the wet mix from 0 to 15 and wet from 85 to 100
        float delta = ((newHappiness * 15) / 100) / 100;
        masterMixer.SetFloat("DialogEchoDryMix", 0.85f + delta);
        masterMixer.SetFloat("DialogEchoWetMix", 0.15f - delta);
    }

    public void OnGuitarInteract()
    {
        guitarUnlockCount++;
        if (guitarUnlockCount > guitarScoreSequences.Length)
        {
            guitarUnlockCount = guitarScoreSequences.Length;
        }
    }

    // Will fade in slowly, instead of
    // horribly breaking in
    private void RequestPlayScore(AudioSource audiosource)
    {
        StartCoroutine(FadeIn(audiosource, fadeDuration));
    }

    // Will fade out slowly
    private void RequestStopScore(AudioSource audiosource)
    {
        StartCoroutine(FadeOut(audiosource, fadeDuration));
    }

    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        while (source.volume < 1)
        {
            source.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
}
