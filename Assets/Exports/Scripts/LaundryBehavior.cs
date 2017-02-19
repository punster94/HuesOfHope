using UnityEngine;
using System.Collections;

public class LaundryBehavior : InteractionObject
{
    private ArrayList baskets;

    protected override void initialize()
    {
        base.initialize();

        baskets = new ArrayList(GameObject.Find("DormRoom").GetComponentsInChildren<BasketBehavior>());
    }
    protected override void changeDepression()
    {
        foreach (BasketBehavior behavior in GameObject.Find("DormRoom").GetComponentsInChildren<BasketBehavior>())
        {
            if (!behavior.interactable() && baskets.Contains(behavior))
            {
                baskets.Remove(behavior);
                base.changeDepression();
            }
        }

        if (baskets.Count > 0)
            isFinished = false;
    }
}
