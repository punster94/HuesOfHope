using UnityEngine;
using System.Collections.Specialized;
using System.Linq;

public class DialogueManager : MonoBehaviour
{

    public InteractionMessageController messageController;
    public StateManagerBehaviour stateManager;

    private OrderedDictionary pendingDialogues;
    private AudioSource currentAudioSource;


    // Use this for initialization
    void Start()
    {
        pendingDialogues = new OrderedDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAudioSource)
        {
            if (currentAudioSource.isPlaying)
            {
                return;
            }
            else    // It just finished playing this frame.
            {
                StopCurrentDialogue();
            }
        }

        if (pendingDialogues.Count > 0)
        {
            RunCurrentDialogue();
        }
    }

    /// <summary>
    /// Adds the requested dialogue audio and text into the queue.
    /// </summary>
    /// <param name="audio">This is the AudioSource that will play when the queue reaches this item's turn.</param>
    /// <param name="subtitleText">This is the text that will appear on the canvas and will remain on-screen for the duration of the audio.</param>
    /// <param name="forcePlay">This will erase the queue and force this audio to play immediately.</param>
    public void PlayDialogue(AudioSource audio, string subtitleText, bool forcePlay = false)
    {

        if (forcePlay)
        {
            StopCurrentDialogue();
            pendingDialogues.Clear();
        }
        else if (pendingDialogues.Contains(audio))
        {
            return;
        }

        pendingDialogues.Add(audio, subtitleText);

        stateManager.setEndingOver(false);
    }

    private void RunCurrentDialogue()
    {
        currentAudioSource = (AudioSource)pendingDialogues.Cast<System.Collections.DictionaryEntry>().ElementAt(0).Key;
        string text = (string)pendingDialogues[0];

        if (currentAudioSource != null)
        {
            currentAudioSource.Play();
        }

        if (text != null)
        {
            messageController.DisplayText(text);
        }

        pendingDialogues.RemoveAt(0);
    }

    private void StopCurrentDialogue()
    {
        if (currentAudioSource)
        {
            currentAudioSource.Stop();
            currentAudioSource = null;
        }

        messageController.EraseText();

        stateManager.setEndingOver(true);
    }
    
}
