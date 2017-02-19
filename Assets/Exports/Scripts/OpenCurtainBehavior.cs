using UnityEngine;
using System.Collections;

public class OpenCurtainBehavior : InteractionObject
{
    protected override void initialize()
    {
        base.initialize();

        isFinished = true;
    }

    public override void reset()
    {
        if (disappearsOnInteraction)
            updateMeshRenderers();

        setToDisappear();

        base.reset();
    }
}
