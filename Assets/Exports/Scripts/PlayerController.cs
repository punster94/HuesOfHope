using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public StateManagerBehaviour stateManager;
    public float minumumSpeed;
    public float maximumSpeed;
    public Texture2D crosshair;
    public float bobDistance = 0.04f;
    public int numberOfBobPositions = 14;
    public int ticksPerBobPosition = 2;

    public DialogueManager dialogueManager;
    public AudioSource wakeUpMonologue;
    public string wakeUpSubtitleText;

    private Transform playerCameraTransform;
    private Rigidbody rigidbodyComponent;
    private Animation getOutOfBedAnimation;

    private int bobIndex;
    private float[] bobPatern;

    private Vector3 lastTranslation;

    private Quaternion startingRotation;
    private Vector3 startingPosition;
    private Quaternion initialCameraRotation;
    
    // Use this for initialization
    void Start()
    {
        startingRotation = transform.rotation;
        startingPosition = transform.position;

        lastTranslation = new Vector3(0, 0, 0);
        playerCameraTransform = transform.FindChild("Main Camera");

        initialCameraRotation = playerCameraTransform.rotation;

        getOutOfBedAnimation = gameObject.GetComponent<Animation>();
        getOutOfBedAnimation.wrapMode = WrapMode.Once;

        rigidbodyComponent = gameObject.GetComponent<Rigidbody>();
        bobIndex = 0;
        generateBobPatern();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (stateManager.inGameState())
            drawCrosshair();
    }

    void FixedUpdate()
    {
        if (stateManager.inGameState() && stateManager.takingInput() && !getOutOfBedAnimation.isPlaying)
            updatePosition();
    }

    private void updatePosition()
    {
        rigidbodyComponent.velocity = new Vector3();
        lastTranslation = (Input.GetAxis("Vertical") * playerCameraTransform.forward + Input.GetAxis("Horizontal") * playerCameraTransform.right) * calculateDynamicSpeed();
        lastTranslation.y = 0;

        if (lastTranslation.x != 0 && lastTranslation.z != 0)
        {
            lastTranslation.y = calculateCameraBob();
            lastTranslation.z += lastTranslation.y / 2;
        }

        transform.position += lastTranslation;
    }

    private float calculateDynamicSpeed()
    {
        float dynamicPortion = maximumSpeed - minumumSpeed;

        dynamicPortion *= stateManager.currentDepressionPercentage();

        return dynamicPortion + minumumSpeed;
    }

    private void drawCrosshair()
    {
        float xMin = (Screen.width / 2) - (crosshair.width / 2);
        float yMin = (Screen.height / 2) - (crosshair.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshair.width, crosshair.height), crosshair);
    }

    private float calculateCameraBob()
    {
        int previousIndex = bobIndex;
        bobIndex += 1;
        bobIndex %= bobPatern.Length;

        return bobPatern[bobIndex] - bobPatern[previousIndex];
    }

    private void generateBobPatern()
    {
        bobPatern = new float[numberOfBobPositions * ticksPerBobPosition * 2];

        for(int i = 0; i < numberOfBobPositions; i++)
        {
            for (int j = 0; j < ticksPerBobPosition; j++)
            {
                bobPatern[i * ticksPerBobPosition + j] = bobDistance * i;
                bobPatern[ticksPerBobPosition * numberOfBobPositions * 2 - (i * ticksPerBobPosition + j + 1)] = bobDistance * i;
            }
        }
    }

    public Vector3 getLastTranslation()
    {
        return lastTranslation;
    }

    public void getOutOfBed()
    {
        getOutOfBedAnimation.Play();

        if (wakeUpMonologue != null && wakeUpSubtitleText != null && dialogueManager != null)
        {
            dialogueManager.PlayDialogue(wakeUpMonologue, wakeUpSubtitleText);
        }
    }

    public void reset()
    {
        transform.rotation = startingRotation;
        transform.position = startingPosition;

        playerCameraTransform.rotation = initialCameraRotation;

        Start();
    }
}
