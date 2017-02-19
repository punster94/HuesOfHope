using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraController : MonoBehaviour
{ 
    private float yawSpeed;
    private float pitchSpeed;

    private float currentYaw = 0f;
    private float currentPitch = 0f;

    public StateManagerBehaviour stateManager;
    public float maximumBlurIntensity;

    private Animation getOutOfBedAnimation;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;

        getOutOfBedAnimation = gameObject.transform.parent.gameObject.GetComponent<Animation>();

        yawSpeed = 10f;
        pitchSpeed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        updateBlur();
    }

    void FixedUpdate()
    {
        if (stateManager.inGameState() && stateManager.takingInput() && !getOutOfBedAnimation.isPlaying)
            updateCameraAngle();
    }

    private void updateCameraAngle()
    {
        currentYaw += yawSpeed * Input.GetAxis("Mouse X");

        float pitchAdjustment = pitchSpeed * Input.GetAxis("Mouse Y");

        if (currentPitch - pitchAdjustment > 90f)
            currentPitch = 90f;
        else if (currentPitch - pitchAdjustment < -90f)
            currentPitch = -90f;
        else
            currentPitch -= pitchAdjustment;

        transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0f);
    }

    private void updateBlur()
    {
        gameObject.GetComponent<BlurOptimized>().blurSize = maximumBlurIntensity * (1f - stateManager.currentDepressionPercentage());
    }

    public void reset()
    {
        Cursor.visible = false;

        yawSpeed = 10f;
        pitchSpeed = 2f;

        currentYaw = currentPitch = 0f;
    }
}
