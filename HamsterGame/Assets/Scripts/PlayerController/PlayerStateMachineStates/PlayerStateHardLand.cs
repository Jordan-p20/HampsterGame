using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHardLand : PlayerState
{
    private float elapsedTime = 0f;
    private float animLength = 2f;

    public override void OnStateEnter()
    {
        anim.SetBool("Land", true);
        animLength = animLengths["HardLand"];
    }

    public override void OnStateExit()
    {
        anim.SetBool("Land", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;
        //Debug.Log(elapsedTime);
        TransitionCheck();
    }

    public override void TransitionCheck()
    {
        if (elapsedTime >= animLength)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
    }
}
