using UnityEngine;
using System.Collections;

public class DoorBehavior : InteractionObject
{
    public AnimationClip openAnimation;
    public AnimationClip closeAnimation;
    private Animation animations;

    private bool opened;

    protected override void initialize()
    {
        base.initialize();

        animations = gameObject.GetComponent<Animation>();
        animations.wrapMode = WrapMode.Once;
        opened = false;
    }

    protected override void interact()
    {
        if (!opened)
            open();

        base.interact();
    }

    private void open()
    {
        animations.Play(openAnimation.name);
        opened = true;
    }

    private void close()
    {
        animations.Play(closeAnimation.name);
        opened = false;
    }

    public override void reset()
    {
        if (opened)
        {
            close();
            updateMeshRenderers();
        }

        base.reset();
    }
}
