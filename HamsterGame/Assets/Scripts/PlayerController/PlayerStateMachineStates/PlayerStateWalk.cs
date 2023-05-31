//player state that deals with walking, running, and idling
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStateWalk : PlayerState
{
    private float playerSpeed = 4.8f;//current player speed
    private float lastSpeed = 0f;//the last speed the player was


    private const float WALK_SPEED = 4.8f;//how fast the player walks
    private const float RUN_SPEED = 6.5f;//how fast the player runs
    private const float LOCK_ON_SPEED = 4f;//how fast the player moves while locked on
    private const float TURN_SPEED = 10f;//how fast the players body changes to the correct direction
    private const float BLEND_RATE = 6f;//how fast the animations blend


    public override void OnStateEnter()
    {
        anim.SetBool("inLocomotionState", true);
    }

    public override void OnStateExit()
    {
        anim.SetFloat("locomotionBlend", 0);
        anim.SetBool("inLocomotionState", false);
    }

    public override void StateUpdate()
    {
        //set correct speed
        if (controller.isGrounded)
        {
            if (!PlayerManager.playerCameraMovement.lockedOn)
            {
                if (PlayerManager.playerControllerInput.runHeld)
                {
                    playerSpeed = lastSpeed = RUN_SPEED;
                }
                else
                {
                    playerSpeed = lastSpeed = WALK_SPEED;
                }
            }
            else
            {
                playerSpeed = LOCK_ON_SPEED;
            }
           
        }
        else
        {
            playerSpeed = lastSpeed;
        }
        
        controller.Move(GetMotion());
    }

    //returns the motion of the character given inputs
    private Vector3 GetMotion()
    {
        UpdateVerticalMotion();
        UpdateHorizontalMotion();
        return (horizontalMotion + verticalMotion) * Time.deltaTime;
    }

    //updates the horizontal (x,z) axis movement 
    private void UpdateHorizontalMotion()
    {
        Vector3 resultMotion = GetHorizontalMotion();

        horizontalMotion = resultMotion * playerSpeed;

        //debugstuff
        //Debug.DrawRay(controller.transform.position + Vector3.up * 0.1f, Vector3.down * 0.15f, Color.blue, 0.1f);//downwards slope checking ray drawer
        //Debug.DrawRay(controller.transform.position + Vector3.up, horizontalMotion.normalized, Color.red, 0.1f);//movement vector ray drawer


        SetAnimatorMotionParameters(resultMotion, PlayerManager.playerControllerInput.moveInput);//set animation parameters for locomotion animations
        SetBodyDirection(PlayerManager.playerControllerInput.moveInput);//set what direction the body should be facing

    }

    //sets which direction the body faces
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

    //sets the animation parameters given the input
    private void SetAnimatorMotionParameters(Vector3 motion, Vector2 inputVector)
    {
        motion = motion.normalized;
        if (playerSpeed == RUN_SPEED && motion.magnitude > 0)
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 1, BLEND_RATE * Time.deltaTime));
        }
        else if ((playerSpeed == WALK_SPEED || playerSpeed == LOCK_ON_SPEED) && motion.magnitude > 0)
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 0.5f, BLEND_RATE * Time.deltaTime));
        }
        else
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 0, BLEND_RATE * Time.deltaTime));
        }
        
        anim.SetFloat("locomotionBlendX", Mathf.Lerp(anim.GetFloat("locomotionBlendX"), inputVector.x, BLEND_RATE * Time.deltaTime));
        anim.SetFloat("locomotionBlendY", Mathf.Lerp(anim.GetFloat("locomotionBlendY"), inputVector.y, BLEND_RATE * Time.deltaTime));
    }

    //updates the vertical (y) axis movement
    private void UpdateVerticalMotion()
    {
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;
        

        if (controller.isGrounded)
        {
            verticalMotion = Vector3.up * GRAVITY * 0.15f;
        }
    }

    public override void TransitionCheck()
    {
        if (PlayerManager.playerControllerInput.middleMousePressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.ROLL);
            return;
        }

        if (controller.isGrounded && PlayerManager.playerControllerInput.jumpPressed)
        {
            SM.TransitionState(PlayerStates.JUMP);
            return;
        }

        //attacking
        if (PlayerManager.playerControllerInput.attackPressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.ATTACK);
            return;
        }

        if (!controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.FALL);
            return;
        }

        if (PlayerManager.playerControllerInput.aimPressed && controller.isGrounded && !PlayerManager.playerCameraMovement.lockedOn)
        {
            SM.TransitionState(PlayerStates.AIM_GRAPPLE);
            return;
        }

    }
}
