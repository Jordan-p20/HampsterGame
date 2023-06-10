using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class PlayerStateAimGrapple : PlayerState
{
    private const float MAX_GRAPPLE_DISTANCE = 25f;//make sure to change in GrappleMove now
    private const float AIM_WALK_SPEED = 4f;
    private const float BLEND_RATE = 6f;
    private const float TURN_SPEED = 10f;

    private GameObject grappleProjectile = null;

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

        anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1, Time.deltaTime * BLEND_RATE));
        
        

        if (grappleProjectile != null)
        {
            horizontalMotion = Vector3.zero;
            SetAnimatorMotionParameters(horizontalMotion, Vector3.zero);
            SetBodyDirection(Vector3.zero);
            
        }
        else
        {
            horizontalMotion = GetHorizontalMotion();
            SetAnimatorMotionParameters(horizontalMotion, PlayerManager.playerControllerInput.moveInput);
            SetBodyDirection(PlayerManager.playerControllerInput.moveInput);
            
        }
        
        verticalMotion = Vector3.up * GRAVITY * 0.15f;

        controller.Move((verticalMotion + (horizontalMotion * AIM_WALK_SPEED)) * Time.deltaTime);
        
        if (PlayerManager.playerControllerInput.attackPressed && grappleProjectile == null)//if you right click while holding left click
        {
            anim.SetBool("GrappleThrow", true);
        }
        
    }

    public void ThrowGrapple()
    {
        RaycastHit hitInfo;
        grappleProjectile = MiscPrefabSpawnManager.instance.GetNewPrefabGO(MiscPrefab.GrappleProjectile);
        bool hit = Physics.Raycast(PlayerManager.playerCameraMovement.actualCamera.position, PlayerManager.playerCameraMovement.actualCamera.forward, out hitInfo, MAX_GRAPPLE_DISTANCE);
        if (hit)
        {
            Vector3 direction = (hitInfo.point - (rightHandTracker.position)).normalized;
            grappleProjectile.transform.SetPositionAndRotation(rightHandTracker.position, Quaternion.LookRotation(direction));

        }
        else
        {
            grappleProjectile.transform.SetPositionAndRotation(rightHandTracker.position, PlayerManager.playerCameraMovement.actualCamera.rotation);
        }
        grappleProjectile.GetComponent<GrappleProjectile>().Initialize(rightHandTracker, Vector3.zero, SM);
        
    }

    public void ResetGrappleAnimation()
    {
        anim.SetBool("GrappleThrow", false);
    }

    public override void TransitionCheck()
    {
        if (!controller.isGrounded)
        {
            ResetGrappleAnimation();
            SM.TransitionState(PlayerStates.FALL);
            return;
        }

        //might want this but kinda doesnt work cuz of pressing lMouse to throw grapple so ill take it out until we have a solution for control scheme
        //if (PlayerManager.playerControllerInput.attackPressed && controller.isGrounded)
        //{
        //    SM.TransitionState(PlayerStates.ATTACK);
        //    return;
        //}

        if (PlayerManager.playerControllerInput.jumpPressed && controller.isGrounded && grappleProjectile == null)
        {
            ResetGrappleAnimation();
            SM.TransitionState(PlayerStates.JUMP);
            return;
        }

        if ((!PlayerManager.playerControllerInput.aimPressed || PlayerManager.playerCameraMovement.lockedOn) && grappleProjectile == null)
        {
            ResetGrappleAnimation();
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
