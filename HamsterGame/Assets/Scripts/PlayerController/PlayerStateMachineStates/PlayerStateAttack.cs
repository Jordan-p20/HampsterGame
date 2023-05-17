using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAttack : PlayerState
{
    private float animLength;
    private float elapsedTime = 0f;
    private int cambo = 0;


    public override void OnStateEnter()
    {
        anim.SetBool("Attack", true);
        animLength = animLengths["Attack"] * 0.5f;
    }

    public override void OnStateExit()
    {
        anim.SetBool("Attack", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        TransitionCheck();
    }

    public override void TransitionCheck()
    {
        if (elapsedTime >= animLength)
        {
            SM.TransitionState(PlayerStates.WALK);
        }
    }
}
