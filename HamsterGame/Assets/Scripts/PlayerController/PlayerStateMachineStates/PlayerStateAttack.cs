using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAttack : PlayerState
{
    private float animLength;
    private float elapsedTime = 0f;
    private int combo = 1;
    private const int maxCombo = 3;

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
    }

    public override void TransitionCheck()
    {
        if (PlayerManager.playerControllerInput.middleMousePressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.ROLL);
            return;
        }

        if (elapsedTime >= animLength)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }

        if (elapsedTime >= animLength * 0.75f && combo < maxCombo && PlayerManager.playerControllerInput.attackPressed)
        {
            Debug.Log(combo);
            elapsedTime= 0f;
            combo++;
            anim.CrossFade("Attack", 0.1f, 0, 0);
            return;
        }
    }
}
