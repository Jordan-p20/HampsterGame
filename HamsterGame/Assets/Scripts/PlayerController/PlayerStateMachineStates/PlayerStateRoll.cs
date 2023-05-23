using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRoll : PlayerState
{
    private const float ROLL_SPEED = 5f;
    private Vector3 directionVector;
    private float animLength;
    private float elapsedTime = 0f;
    public override void OnStateEnter()
    {
        anim.SetBool("Roll", true);
        animLength = animLengths["DodgeRoll"];
        playerBody.rotation = Quaternion.LookRotation(directionVector, Vector3.up);
        Debug.Log(animLength);
    }

    public override void OnStateExit()
    {
        anim.SetBool("Roll", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        controller.Move( ( (directionVector * ROLL_SPEED) + verticalMotion ) * Time.deltaTime );
    }

    public override void TransitionCheck()
    {
        if (elapsedTime >= animLength)
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
