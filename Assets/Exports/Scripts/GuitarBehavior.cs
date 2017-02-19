using UnityEngine;
using System.Collections;

public class GuitarBehavior : InteractionObject
{
    public MixerManager mixerManager;
    private ArrayList guitarHelpers;

    override protected void interact()
    {
        base.interact();
    }

    protected override void initialize()
    {
        base.initialize();

        guitarHelpers = new ArrayList(GameObject.Find("DormRoom").GetComponentsInChildren<PickBehavior>());
    }
    protected override void changeDepression()
    {
        foreach (PickBehavior behavior in GameObject.Find("DormRoom").GetComponentsInChildren<PickBehavior>())
        {
            if (!behavior.interactable() && guitarHelpers.Contains(behavior))
            {
                mixerManager.OnGuitarInteract();
                guitarHelpers.Remove(behavior);
                base.changeDepression();
            }
        }

        if (guitarHelpers.Count > 0)
            isFinished = false;
    }
}
