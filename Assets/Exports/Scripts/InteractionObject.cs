using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class MeshEmissionColor
{
    public MeshRenderer renderer;
    public Color initialEmissionColor;
    public bool emissionEnabled;

    public MeshEmissionColor(MeshRenderer r, Color c, bool e)
    {
        renderer = r;
        initialEmissionColor = c;
        emissionEnabled = e;
    }
}
public class InteractionObject : MonoBehaviour
{
    public AudioSource interactionSound;
    public StateManagerBehaviour stateManager;
    public Camera mainCamera;
    public UIBarManager uiBarManager;
    public string[] hudEnableObject;
    public string[] hudDisableObject;

    public string[] interactionText;
    public AudioSource[] interactionMonologue;
    public DialogueManager dialogueManager;

    public bool interactableAtStart = true;
    public bool disappearsOnInteraction = false;
    public float happinessOnInteraction = 5f;

    protected ArrayList nextItemsInChain;
    protected bool canBeInteracted;
    protected bool isFinished;
    protected bool isGlowing;
    protected ArrayList renderers;
    protected ArrayList emissionPairs;

    private float distanceToInteract = 8;
    private AudioSource notReadyAudio;
    private string notReadyText = "I don't... <b><color=COLOR_ORANGE>I'm not ready.</color></b>";

    private AudioSource[] alreadyDoneAudio;
    private string[] alreadyDoneText;

    private AudioSource interactScore;

    protected int interactCount = 0;

    void Start()
    {
        MeshRenderer tempRenderer;

        renderers = new ArrayList();
        emissionPairs = new ArrayList();

        if ((tempRenderer = gameObject.GetComponent<MeshRenderer>()) != null)
        {
            renderers.Add(tempRenderer);
            emissionPairs.Add(new MeshEmissionColor(tempRenderer, tempRenderer.material.GetColor("_EmissionColor"), tempRenderer.material.HasProperty("_Emission")));
        }
        else
        {
            foreach (Transform child in transform)
            {
                if ((tempRenderer = child.gameObject.GetComponent<MeshRenderer>()) != null)
                {
                    renderers.Add(tempRenderer);
                    emissionPairs.Add(new MeshEmissionColor(tempRenderer, tempRenderer.material.GetColor("_EmissionColor"), tempRenderer.material.HasProperty("_Emission")));
                }
            }
        }

        notReadyAudio = GameObject.Find("IDontImNotReady").GetComponent<AudioSource>();
        interactCount = 0;

        alreadyDoneAudio = new AudioSource[2];
        alreadyDoneText = new string[2];

        alreadyDoneAudio[0] = GameObject.Find("IveAlreadyDoneThat").GetComponent<AudioSource>();
        alreadyDoneText[0] = "I've <b><color=COLOR_ORANGE>already done</color></b> that.";

        alreadyDoneAudio[1] = GameObject.Find("ImDoneWithThat").GetComponent<AudioSource>();
        alreadyDoneText[1] = "I'm <b><color=COLOR_ORANGE>done</color></b> with that.";

        interactScore = GameObject.Find("PositiveVibe").GetComponent<AudioSource>();

        initialize();
    }

    void Update()
    {
        if (isBeingLookedAt() && !isFinished)
        {
            changeGlowEffect(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (canBeInteracted)
                    interact();
                else if (!disappearsOnInteraction)
                    PresentDialogue(false);
            }
        }
        else
            changeGlowEffect(false);
    }

    virtual protected void interact()
    {
        isFinished = true;

        updateMeshRenderers();
        changeDepression();

        PresentDialogue();

        if (interactionSound)
        {
            interactionSound.Play();
        }

        if (interactScore && happinessOnInteraction > 0)
        {
            interactScore.Play();
        }

        if (uiBarManager && hudEnableObject != null)
        {
            foreach (string str in hudEnableObject)
            {
                if (str != "")
                {
                    uiBarManager.Enable(str);
                }
            }
        }

        if (uiBarManager && hudDisableObject != null)
        {
            foreach (string str in hudDisableObject)
            {
                if (str != "")
                {
                    uiBarManager.Disable(str);
                }
            }
        }

        ++interactCount;
        if (interactCount >= interactionMonologue.Length || interactCount >= interactionText.Length)
        {
            interactCount = 0;
        }

        canBeInteracted = false;

        if (nextItemsInChain != null)
            foreach (InteractionObject item in nextItemsInChain)
                if (!item.interactable())
                    item.setToInteractable();
    }

    protected void PresentDialogue(bool ready = true)
    {
        if (ready)
        {
            if (interactionMonologue != null && interactionMonologue.Length > 0)
            {
                dialogueManager.PlayDialogue(interactionMonologue[interactCount], interactionText.Length > 0 ? interactionText[interactCount] : null);
            }
        }
        else
        {
            if (isFinished && alreadyDoneAudio != null && alreadyDoneAudio.Length > 0)
            {
                int randomIndex = Random.Range(0, alreadyDoneAudio.Length);
                dialogueManager.PlayDialogue(alreadyDoneAudio[randomIndex], alreadyDoneText[randomIndex]);
            }
            else if (notReadyAudio != null)
            {
                dialogueManager.PlayDialogue(notReadyAudio, notReadyText);
            }
        }
    }

    virtual protected void initialize()
    {
        canBeInteracted = interactableAtStart;
        isFinished = false;

        if (!interactableAtStart)
            updateMeshRenderers(true);
    }

    virtual protected void changeDepression()
    {
        stateManager.changeDepression(happinessOnInteraction);
    }

    public void updateMeshRenderers(bool onStart = false)
    {
        foreach (MeshRenderer mesh in renderers)
            updateMesh(mesh, onStart);
    }

    public void setToUninteractable()
    {
        canBeInteracted = false;
    }

    public void setToEnvironment()
    {
        disappearsOnInteraction = false;
    }

    public void setToDisappear()
    {
        disappearsOnInteraction = true;
    }

    virtual public void reset()
    {
        if (disappearsOnInteraction && !canBeInteracted && interactableAtStart)
            updateMeshRenderers();

        Start();
    }

    protected void setToInteractable()
    {
        updateMeshRenderers();

        canBeInteracted = true;
    }

    virtual protected void changeGlowEffect(bool on)
    {
        if (isGlowing == on)
            return;

        foreach (MeshRenderer renderer in renderers)
        {
            MeshEmissionColor pairing = (MeshEmissionColor)emissionPairs[0];

            foreach (MeshEmissionColor pair in emissionPairs)
            {
                if (pair.renderer == renderer)
                {
                    pairing = pair;
                    break;
                }
            }

            if (on)
            {
                renderer.material.SetColor("_EmissionColor", new Color(pairing.initialEmissionColor.r, pairing.initialEmissionColor.g, pairing.initialEmissionColor.b + 0.5f));
                renderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
                renderer.material.SetColor("_EmissionColor", pairing.initialEmissionColor);
                renderer.material.DisableKeyword("_EMISSION");
            }
        }

        isGlowing = on;
    }

    private void updateMesh(MeshRenderer mesh, bool onStart)
    {
        if (disappearsOnInteraction)
            toggleMeshVisibility(mesh);
        else
            toggleMeshColor(mesh, onStart);
    }

    private void toggleMeshColor(MeshRenderer mesh, bool onStart)
    {
        AdjustableColorBehaviour behavior = mesh.gameObject.GetComponent<AdjustableColorBehaviour>();

        if (behavior != null)
            toggleFullColor(behavior, onStart);
    }

    private void toggleMeshVisibility(MeshRenderer mesh)
    {
        mesh.enabled = !mesh.enabled;
    }

    private bool isBeingLookedAt()
    {
        foreach (MeshRenderer renderer in renderers)
        {
            if (Physics.Raycast(renderer.transform.position, -mainCamera.transform.forward, distanceToInteract, LayerMask.GetMask("Player")))
                return true;
        }

        return false;
    }

    private void toggleFullColor(AdjustableColorBehaviour behavior, bool onStart)
    {
        if (onStart)
            behavior.turnFullColorOffOnStart();
        else
            behavior.setFullColor(!behavior.isFullColor());
    }

    public bool interactable()
    {
        return canBeInteracted;
    }
}
