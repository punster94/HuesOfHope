using UnityEngine;
using System.Collections;

public class MovieBehavior : InteractionObject
{
    protected override void initialize()
    {
        base.initialize();

        nextItemsInChain = new ArrayList();
        nextItemsInChain.Add(GameObject.Find("DormRoom").GetComponentInChildren<TVBehavior>());
    }
}
