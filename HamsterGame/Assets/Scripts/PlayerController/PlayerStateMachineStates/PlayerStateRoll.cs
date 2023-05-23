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
    private const float BLEND_TIME = 5f;

    private float animLength;
    private float elapsedTime = 0f;
    public override void OnStateEnter()
    {
        anim.SetBool("Roll", true);
        animLength = animLengths["DodgeRoll"];
    }

    public override void OnStateExit()
    {
        anim.SetBool("Roll", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        SelectMoveSpeed();

        GetHorizontalMotion();

        RotateBody();

        controller.Move( ( (horizontalMotion * rollSpeed) + verticalMotion ) * Time.deltaTime );
    }

    private void RotateBody()
    {
        playerBody.rotation = Quaternion.Lerp(playerBody.rotation, Quaternion.LookRotation(horizontalMotion, Vector3.up), Time.deltaTime * BLEND_TIME);
    }

    private void GetHorizontalMotion()
    {
        Vector2 moveInput = PlayerManager.playerControllerInput.moveInput;
        Vector3 forward = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);

        Vector3 right = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.RIGHT);

        Vector3 input = (((forward * moveInput.y) + (right * moveInput.x)).normalized);

        if (input.sqrMagnitude != 0)
        {
            horizontalMotion = input;
        }
        
    }

    private void SelectMoveSpeed()
    {
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
    }

    public override void TransitionCheck()
    {
        if (!controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }

        if (elapsedTime >= animLength * 0.78f && PlayerManager.playerControllerInput.attackPressed)
        {
            SM.TransitionState(PlayerStates.ATTACK);
            return;
        }

        if (elapsedTime >= animLength * 0.78f)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
    }

    public override void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion)
    {
        base.Initialize(SMData, previousStateHorMotion);
        horizontalMotion = previousStateHorMotion.normalized;
        if (horizontalMotion.sqrMagnitude <= 0)
        {
            horizontalMotion = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);
        }

        verticalMotion = Vector3.up * GRAVITY * 0.15f;
    }
}
