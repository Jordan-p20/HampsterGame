using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRoll : PlayerState
{


    // <.36667 <- slow
    // .36667 <- fast
    //1.06667 <- stop

    private float rollSpeed;

    private const float FAST_ROLL_SPEED = 6.5f;
    private const float SLOW_ROLL_SPEED = 2.5f;
    private Vector3 directionVector;
    private float animLength;
    private float elapsedTime = 0f;
    public override void OnStateEnter()
    {
        anim.SetBool("Roll", true);
        animLength = animLengths["DodgeRoll"];
        playerBody.rotation = Quaternion.LookRotation(directionVector, Vector3.up);
    }

    public override void OnStateExit()
    {
        anim.SetBool("Roll", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        //if (elapsedTime < 0.3667f)
        //{
        //    rollSpeed = SLOW_ROLL_SPEED;
        //}
        if (elapsedTime <= 0.8667f)
        {
            rollSpeed = FAST_ROLL_SPEED;
        }
        else if (elapsedTime <= 1.1f)
        {
            rollSpeed = SLOW_ROLL_SPEED;
        }
        else
        {
            rollSpeed = 0;
        }

        controller.Move( ( (directionVector * rollSpeed) + verticalMotion ) * Time.deltaTime );
    }

    public override void TransitionCheck()
    {
        if (elapsedTime >= animLength * 0.78f)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
    }

    public override void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion)
    {
        base.Initialize(SMData, previousStateHorMotion);
        directionVector = previousStateHorMotion.normalized;
        if (directionVector.sqrMagnitude <= 0)
        {
            directionVector = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);
        }

        verticalMotion = Vector3.up * GRAVITY * 0.15f;
    }
}