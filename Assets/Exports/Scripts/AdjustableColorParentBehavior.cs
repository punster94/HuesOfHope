using UnityEngine;
using System.Collections;

public class AdjustableColorParentBehavior : MonoBehaviour
{
    public StateManagerBehaviour stateManager;

	// Use this for initialization
	void Start ()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<MeshRenderer>() != null)
                child.gameObject.AddComponent<AdjustableColorBehaviour>().stateManager = stateManager;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
