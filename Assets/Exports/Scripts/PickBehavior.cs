using UnityEngine;
using System.Collections;

public class PickBehavior : InteractionObject
{
    protected override void initialize()
    {
        base.initialize();

        nextItemsInChain = new ArrayList();
        nextItemsInChain.Add(GameObject.Find("DormRoom").GetComponentInChildren<GuitarBehavior>());
    }
}
