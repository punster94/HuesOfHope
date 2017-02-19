using UnityEngine;
using System.Collections;

public class BasketBehavior : InteractionObject
{
    protected override void initialize()
    {
        base.initialize();

        nextItemsInChain = new ArrayList();
        nextItemsInChain.Add(GameObject.Find("DormRoom").GetComponentInChildren<LaundryBehavior>());
    }
}
