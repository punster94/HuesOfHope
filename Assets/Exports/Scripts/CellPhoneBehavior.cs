using UnityEngine;

public class CellPhoneBehavior : InteractionObject
{
    public int timesToInteract;
    public AudioSource ringSound;
    public AudioSource winDialog;
    public string winText;
    public AudioSource loseDialog;
    public string loseText;

    public Material textScreen;
    public Material phoneCallScreen;
    public Material winScreen;

    private Animation animations;

    private bool winningCall;
    private bool losingCall;

    private MeshRenderer mesh;

    protected override void initialize()
    {
        base.initialize();

        animations = gameObject.GetComponent<Animation>();
        animations.wrapMode = WrapMode.Once;

        mesh = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    public void moveCellPhoneUp(bool win = false, bool loss = false)
    {
        winningCall = win;
        losingCall = loss;

        mesh.material = phoneCallScreen;

        if (win || loss || timesToInteract == interactCount + 1)
            mesh.material = textScreen;

        animations.Play("cellPhoneUp");
        playRingSound();
    }

    public void moveCellPhoneDown()
    {
        animations.Play("cellPhoneDown");
    }

    private void playRingSound()
    {
        ringSound.Play();
    }

    protected override void interact()
    {
        ringSound.Stop();

        if (winningCall || losingCall || timesToInteract == interactCount + 1)
        {
            if (winningCall)
            {
                dialogueManager.PlayDialogue(winDialog, winText, true);
                mesh.material = winScreen;
            }
            else
            {
                dialogueManager.PlayDialogue(loseDialog, loseText, true);
            }

            stateManager.executeEndState(winningCall);
        }
        else
        {
            PresentDialogue();
            moveCellPhoneDown();

            stateManager.setTakingInput();
        }

        ++interactCount;

    }

    public override void reset()
    {
        moveCellPhoneDown();
        base.reset();
    }

    protected override void changeGlowEffect(bool on)
    {
        
    }
}
