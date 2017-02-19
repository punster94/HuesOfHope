using UnityEngine;
using System.Collections;

public class FanController : MonoBehaviour
{
    public StateManagerBehaviour stateManager;
    public float maxRotationSpeed = 5f;

    private Vector3 currentRotation;

	// Use this for initialization
	void Start ()
    {
        currentRotation = new Vector3();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void FixedUpdate()
    {
        rotateGameObject();
    }

    private void rotateGameObject()
    {
        currentRotation.y = rotationThisTick();
        gameObject.transform.eulerAngles += currentRotation;
    }

    private float rotationThisTick()
    {
        return stateManager.currentDepressionPercentage() * maxRotationSpeed;
    }
}
