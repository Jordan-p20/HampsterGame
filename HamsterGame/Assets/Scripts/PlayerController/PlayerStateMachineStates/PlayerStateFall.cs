using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFall : PlayerState
{
    private float fallLength = 0f;
    private const float HIGH_FALL_THRESHOLD = 0.85f;

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

        fallLength += Time.deltaTime;

        TransitionCheck();
    }

    public override void TransitionCheck()
    {
        if (controller.isGrounded)
        {
            if (fallLength >= HIGH_FALL_THRESHOLD)
            {
                //Debug.Log("Fall length " + fallLength);
                SM.TransitionState(PlayerStates.HARD_LAND);
                return;
            }
            else
            {
                //Debug.Log("Fall length " + fallLength);
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
