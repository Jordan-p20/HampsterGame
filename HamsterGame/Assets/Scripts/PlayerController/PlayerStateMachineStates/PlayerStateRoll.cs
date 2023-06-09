using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRoll : PlayerState
{


    // <.36667 <- slow
    // .36667 <- fast
    //1.06667 <- stop

    private float rollSpeed;//the current roll speed

    private const float FAST_ROLL_SPEED = 7f;//the speed when the roll is fast
    private const float SLOW_ROLL_SPEED = 2.5f;//the speed when the roll is slow
    private const float BLEND_TIME = 5f;//how fast to blend the body towards the roll direcrion

    private float animLength;// how long the animation is
    private float elapsedTime = 0f;//how long the state has been active for
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

        UpdateHorizontalMotion();
        UpdateVerticalMotion();

        RotateBody();

        controller.Move( ( (horizontalMotion * rollSpeed) + verticalMotion ) * Time.deltaTime );
    }

    //rotates the players body towards the horizontal motion input by a blend time amount
    private void RotateBody()
    {
        playerBody.rotation = Quaternion.Lerp(playerBody.rotation, Quaternion.LookRotation(horizontalMotion, Vector3.up), Time.deltaTime * BLEND_TIME);
    }

    //updates the vertical (Y) motion
    private void UpdateVerticalMotion()
    {
        if (!controller.isGrounded)
        {
            verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;
        }
        else
        {
            verticalMotion = Vector3.up * GRAVITY * 0.15f;
        }
    }

    //gets an input direction vector from input
    private void UpdateHorizontalMotion()
    {
        Vector3 input = GetHorizontalMotion();

        if (input.sqrMagnitude != 0)
        {
            horizontalMotion = input;
        }
        
    }

    //selects what speed the player should move based on elapsed time
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
        if (!controller.isGrounded && elapsedTime >= animLength * 0.78f)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }

        if (elapsedTime >= animLength * 0.78f && PlayerManager.playerControllerInput.attackPressed)
        {
            SM.TransitionState(PlayerStates.ATTACK);
            return;
        }

        if (elapsedTime >= animLength * 0.78f && controller.isGrounded)
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
