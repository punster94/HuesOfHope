using UnityEngine;
using System.Collections;

public class LightBehavior : MonoBehaviour
{
    public StateManagerBehaviour stateManager;
    public float minimumIntensity = 1f;

    private Light[] lights;
    private float[] originalIntensities;

	// Use this for initialization
	void Start ()
    {
        lights = gameObject.GetComponentsInChildren<Light>();

        originalIntensities = new float[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            originalIntensities[i] = lights[i].intensity;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity = (originalIntensities[i] - minimumIntensity) * stateManager.currentDepressionPercentage() + minimumIntensity;
        }
	}
}
