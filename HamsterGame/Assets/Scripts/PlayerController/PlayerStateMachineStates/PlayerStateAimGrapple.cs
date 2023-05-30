using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAimGrapple : PlayerState
{
    private const float MAX_GRAPPLE_DISTANCE = 25f;
    private const float AIM_WALK_SPEED = 3.5f;

    public override void OnStateEnter()
    {
        PlayerManager.playerCameraMovement.ChangeCameraSetting(CamPositionPreset.Close);
        anim.SetBool("inLocomotionState", true);
        MainUIManager.instance.ShowAimReticle();
        PlayerManager.playerCameraMovement.SetClampCamera(false);
    }

    public override void OnStateExit()
    {
        PlayerManager.playerCameraMovement.ChangeCameraSetting(CamPositionPreset.Medium);
        anim.SetBool("inLocomotionState", false);
        MainUIManager.instance.HideAimReticle();
        PlayerManager.playerCameraMovement.SetClampCamera(true);//might change this to change the clamp settings rather than turning it off but this works for now even tho its a little jank
    }

    public override void StateUpdate()
    {
        horizontalMotion = GetHorizontalMotion() * AIM_WALK_SPEED;
        verticalMotion = Vector3.up * GRAVITY * 0.15f;

        controller.Move((verticalMotion + horizontalMotion) * Time.deltaTime);

        RaycastHit hitInfo;
        if (PlayerManager.playerControllerInput.aimPressed)
        {
            bool hit = Physics.Raycast(PlayerManager.playerCameraMovement.actualCamera.position, PlayerManager.playerCameraMovement.actualCamera.forward, out hitInfo, MAX_GRAPPLE_DISTANCE);// place a mask onto this when it becomes necessary
            Debug.Log(hit);
            if (hit)
            {

            }
        }
        
    }

    public override void TransitionCheck()
    {
        if (!controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }

        if (PlayerManager.playerControllerInput.attackPressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.ATTACK);
            return;
        }

        if (PlayerManager.playerControllerInput.jumpPressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.JUMP);
            return;
        }

        if (!PlayerManager.playerControllerInput.aimPressed || !controller.isGrounded || PlayerManager.playerCameraMovement.lockedOn)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }
    }
}
