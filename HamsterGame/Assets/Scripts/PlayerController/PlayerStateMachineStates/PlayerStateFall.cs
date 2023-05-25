using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFall : PlayerState
{
    private const float HIGH_FALL_THRESHOLD = -12f;//how high the player must be falling to be considered a "high fall"

    public override void OnStateEnter()
    {
        anim.SetBool("Fall", true);
    }

    public override void OnStateExit()
    {
        anim.SetBool("Fall", false);
    }

    public override void StateUpdate()
    {
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;

        controller.Move((horizontalMotion + verticalMotion) * Time.deltaTime);

    }

    public void UpdateHorizontalMotion()
    {
        Vector3 input = GetHorizontalMotion();

        if (input.sqrMagnitude != 0)
        {
            horizontalMotion = input;
        }
    }

    public override void TransitionCheck()
    {
        if (controller.isGrounded)
        {
            if (verticalMotion.y <= HIGH_FALL_THRESHOLD)
            {
                SM.TransitionState(PlayerStates.HARD_LAND);
                return;
            }
            else
            {
                SM.TransitionState(PlayerStates.WALK);
                return;
            }
            
        }
    }

    public override void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion, Vector3 previousStateVertMotion)
    {
        base.Initialize(SMData, previousStateHorMotion, previousStateVertMotion);
        horizontalMotion = previousStateHorMotion;
        verticalMotion = previousStateVertMotion;

    }
}
