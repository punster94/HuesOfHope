using UnityEngine;
using System.Collections;

public class AdjustableColorBehaviour : MonoBehaviour
{
    public StateManagerBehaviour stateManager;
    private MeshRenderer meshRenderer;
    private Color[] originalColors;
    private bool fullColor = false;
    private bool fullColorOffOnStart = false;

    // Use this for initialization
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        originalColors = new Color[meshRenderer.materials.Length];

        for (int i = 0; i < originalColors.Length; i++)
            originalColors[i] = meshRenderer.materials[i].color;

        if (!fullColorOffOnStart)
            fullColor = gameObject.GetComponent<InteractionObject>() != null || gameObject.transform.parent.gameObject.GetComponent<InteractionObject>() != null;
    }

    // Update is called once per frame
    void Update()
    {
        if (fullColor)
            for (int i = 0; i < originalColors.Length; i++)
                meshRenderer.materials[i].color = originalColors[i];
        else
            for (int i = 0; i < originalColors.Length; i++)
                changeSaturationToPercentage(stateManager.currentDepressionPercentage(), i);
    }

    private void changeSaturationToPercentage(float percentage, int i) {
        float red = originalColors[i].r, green = originalColors[i].g, blue = originalColors[i].b;

        red *= percentage;
        green *= percentage;
        blue *= percentage;

        meshRenderer.materials[i].color = new Color(red, green, blue);
    }

    public bool isFullColor()
    {
        return fullColor;
    }

    public void setFullColor(bool fc)
    {
        fullColor = fc;
    }

    public void turnFullColorOffOnStart()
    {
        fullColorOffOnStart = true;
    }
}
