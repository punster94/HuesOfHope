using UnityEngine;
using System.Collections;

public class CurtainBehavior : InteractionObject
{
    protected override void interact()
    {
        changeDepression();
        PresentDialogue();

        foreach (Transform child in transform.parent.parent)
        {
            CurtainBehavior closedPart = child.gameObject.GetComponentInChildren<CurtainBehavior>();
            OpenCurtainBehavior openPart = child.gameObject.GetComponentInChildren<OpenCurtainBehavior>();

            closedPart.updateMeshRenderers();
            openPart.updateMeshRenderers();
            openPart.setToEnvironment();
            openPart.updateMeshRenderers();
            closedPart.setToUninteractable();
            openPart.setToUninteractable();
        }
    }
}
