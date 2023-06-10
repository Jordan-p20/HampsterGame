using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateJump : PlayerState
{
    private const float JUMP_SPEED = 7f;//how fast the player jumps

    private float elapsedJumpTime = 0f;//how long the jump state has been active
    private float jumpAnimLength;// how long the animation is
    private const float BLEND_RATE = 6f;//how fast the animations blend

    public override void OnStateEnter()
    {
        anim.SetBool("Jump", true);
        verticalMotion = Vector3.up * JUMP_SPEED;
        jumpAnimLength = (float)animLengths["RunningJump"] * 0.85f;
    }

    public override void OnStateExit()
    {
        anim.SetBool("Jump", false);
    }

    public override void StateUpdate()
    {
        anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, BLEND_RATE * Time.deltaTime));
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;


        controller.Move((verticalMotion + horizontalMotion) * Time.deltaTime);

        elapsedJumpTime += Time.deltaTime;
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
