using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHardLand : PlayerState
{
    private float elapsedTime = 0f;//how long this state has been active
    private float animLength;//the animation length of this state
    private const float ROLL_SPEED = 5f;//the speed how the roll horizontally
    private bool isRolling = false;// whether the player is rolling

    public override void OnStateEnter()
    {
        anim.SetBool("Land", true);
        if (verticalMotion.y > -18)
        {
            animLength = animLengths["LandRoll"];
            isRolling = true;
        }
        else
        {
            animLength = animLengths["HardLand"];
        }
        anim.SetFloat("FallSpeed", verticalMotion.y);
    }

    public override void OnStateExit()
    {
        anim.SetBool("Land", false);
        anim.SetFloat("FallSpeed", 0);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (isRolling)
        {
            controller.Move(((Vector3.up * GRAVITY * 0.2f) + horizontalMotion).normalized * ROLL_SPEED * Time.deltaTime);
        }
        
    }

    public override void TransitionCheck()
    {
        if (elapsedTime >= animLength * 0.95f)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
    }

    public override void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion, Vector3 previousStateVertMotion)
    {
        base.Initialize(SMData, previousStateHorMotion, previousStateVertMotion);
        horizontalMotion = previousStateHorMotion;
        verticalMotion = previousStateVertMotion;
    }
}
