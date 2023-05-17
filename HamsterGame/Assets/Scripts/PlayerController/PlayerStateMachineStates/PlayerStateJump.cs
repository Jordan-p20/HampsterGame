using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateJump : PlayerState
{
    private const float JUMP_SPEED = 7f;

    private float elapsedJumpTime = 0f;
    private float jumpAnimLength = 0.95f;


    public override void OnStateEnter()
    {
        anim.SetBool("Jump", true);
        verticalMotion = Vector3.up * JUMP_SPEED;
        jumpAnimLength = (float)animLengths["RunningJump"];
    }

    public override void OnStateExit()
    {
        anim.SetBool("Jump", false);
    }

    public override void StateUpdate()
    {
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;

        controller.Move((verticalMotion + horizontalMotion) * Time.deltaTime);

        elapsedJumpTime += Time.deltaTime;

        TransitionCheck();
    }

    public override void TransitionCheck()
    {
        if (controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
        else if (elapsedJumpTime >= jumpAnimLength)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }


    }

    public override void Initialize(PlayerStateMachineData SMData, Vector3 previousHorMotion)
    {
        base.Initialize(SMData);
        horizontalMotion = previousHorMotion;
        
    }

}
