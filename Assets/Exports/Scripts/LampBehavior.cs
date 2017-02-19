using UnityEngine;
using System.Collections;

public class LampBehavior : InteractionObject
{
    private float[][] emissions;
    private bool emissive;

    protected override void initialize()
    {
        base.initialize();
        emissive = false;

        changeEmissionOfMeshes(false);

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name == "Lights")
                child.gameObject.SetActive(false);
        }
    }

    protected override void interact()
    {
        base.interact();
        emissive = true;

        foreach (Transform child in gameObject.transform)
        {
            if (child.gameObject.name == "Lights")
                child.gameObject.SetActive(true);
        }

        changeEmissionOfMeshes(true);
    }

    private void changeEmissionOfMeshes(bool enabled)
    {
        foreach (MeshRenderer mesh in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material material in mesh.materials)
            {
                if (enabled)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            }
        }
    }

    protected override void changeGlowEffect(bool on)
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
                if (!emissive)
                    renderer.material.DisableKeyword("_EMISSION");
            }
        }

        isGlowing = on;
    }
}
