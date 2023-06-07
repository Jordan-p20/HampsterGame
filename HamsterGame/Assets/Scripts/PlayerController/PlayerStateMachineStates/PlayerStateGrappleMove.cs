using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGrappleMove : PlayerState
{
    private const float MAX_GRAPPLE_DISTANCE = 25f;//make sure the change in aimGrapple as well
    private const float MAX_GRAPPLE_SPEED = 35f;
    private const float GRAPPLE_SPEED_RATE = 5f;
    private float currentMoveSpeed = 20f;

    private Vector3 travelDirection;

    private RaycastHit hitInfo;
    

    public override void OnStateEnter()
    {
        //Physics.Raycast(PlayerManager.playerCameraMovement.actualCamera.position, PlayerManager.playerCameraMovement.actualCamera.forward, out hitInfo, MAX_GRAPPLE_DISTANCE);
        //travelDirection = (hitInfo.point - (controller.transform.position + controller.center)).normalized;
    }

    public override void OnStateExit()
    {
        
    }

    public override void StateUpdate()
    {
        Debug.DrawRay(controller.transform.position + controller.center, travelDirection * hitInfo.distance, Color.red, 0.1f);
        currentMoveSpeed += GRAPPLE_SPEED_RATE * Time.deltaTime;
        controller.Move(travelDirection * currentMoveSpeed * Time.deltaTime);
    }

    public override void TransitionCheck()
    {
        if (controller.collisionFlags != CollisionFlags.None && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }

        if (controller.collisionFlags != CollisionFlags.None && !controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }


    }

    public void ResetGrappleAnimation()
    {
        anim.SetBool("GrappleThrow", false);
    }

    public void GiveGrapplePoint(Vector3 grapplePointLocation)
    {
        travelDirection = (grapplePointLocation - (controller.transform.position + controller.center)).normalized;
    }
}
