using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateHardLand : PlayerState
{
    private float elapsedTime = 0f;//how long this state has been active
    private float animLength;//the animation length of this state
    private const float ROLL_SPEED = 5f;//the speed how the roll horizontally, change into multiple stages depending on animation, might work to add root motion to animator instead
    private bool isRolling = false;// whether the player is rolling
    private const float BLEND_RATE = 6f;//how fast the animations blend

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
        anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, BLEND_RATE * Time.deltaTime));
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
        if (horizontalMotion.sqrMagnitude == 0)
        {
            horizontalMotion = playerBody.forward * ROLL_SPEED;
        }
        verticalMotion = previousStateVertMotion;
    }
}
