using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerStateAimGrapple : PlayerState
{
    private const float MAX_GRAPPLE_DISTANCE = 25f;
    private const float AIM_WALK_SPEED = 4f;
    private const float BLEND_RATE = 6f;
    private const float TURN_SPEED = 10f;

    public override void OnStateEnter()
    {
        PlayerManager.playerCameraMovement.ChangeCameraSetting(CamPositionPreset.Close);
        anim.SetBool("inLocomotionState", true);
        anim.SetBool("isLockedOn", true);
        MainUIManager.instance.ShowAimReticle();
        PlayerManager.playerCameraMovement.SetClampCamera(false);
        
    }

    public override void OnStateExit()
    {
        PlayerManager.playerCameraMovement.ChangeCameraSetting(CamPositionPreset.Medium);
        anim.SetBool("inLocomotionState", false);
        if (!PlayerManager.playerCameraMovement.lockedOn)
        {
            anim.SetBool("isLockedOn", false);
        }
        MainUIManager.instance.HideAimReticle();
        PlayerManager.playerCameraMovement.SetClampCamera(true);//might change this to change the clamp settings rather than turning it off but this works for now even tho its a little jank
        anim.SetFloat("locomotionBlend", 0);
        anim.SetFloat("locomotionBlendX", 0);
        anim.SetFloat("locomotionBlendY", 0);
    }

    public override void StateUpdate()
    {
        horizontalMotion = GetHorizontalMotion();
        verticalMotion = Vector3.up * GRAVITY * 0.15f;

        controller.Move((verticalMotion + (horizontalMotion * AIM_WALK_SPEED)) * Time.deltaTime);
        SetAnimatorMotionParameters(horizontalMotion, PlayerManager.playerControllerInput.moveInput);
        SetBodyDirection(PlayerManager.playerControllerInput.moveInput);

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

    private void SetAnimatorMotionParameters(Vector3 motion, Vector2 inputVector)
    {
        motion = motion.normalized;
        if (motion.sqrMagnitude != 0)
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 0.5f, BLEND_RATE * Time.deltaTime));
        }

        anim.SetFloat("locomotionBlendX", Mathf.Lerp(anim.GetFloat("locomotionBlendX"), inputVector.x, BLEND_RATE * Time.deltaTime));
        anim.SetFloat("locomotionBlendY", Mathf.Lerp(anim.GetFloat("locomotionBlendY"), inputVector.y, BLEND_RATE * Time.deltaTime));
    }

    private void SetBodyDirection(Vector3 motion)
    {
        if (anim.GetBool("isLockedOn"))//strafing
        {
            //playerBody.forward = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);
            playerBody.rotation = Quaternion.Lerp(playerBody.rotation, Quaternion.LookRotation(PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD)), Time.deltaTime * TURN_SPEED);
            return;
        }

        if (motion.magnitude > 0)//moving horizontally, not strafing
        {

            Vector3 forwardRelativeInput = motion.y * PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);
            Vector3 rightRelativeInput = motion.x * PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.RIGHT);

            Vector3 cameraRelativeMove = (forwardRelativeInput + rightRelativeInput).normalized;
            Quaternion quat = Quaternion.LookRotation(cameraRelativeMove, Vector3.up);

            playerBody.rotation = Quaternion.Lerp(playerBody.rotation, quat, TURN_SPEED * Time.deltaTime);
        }

    }
}
