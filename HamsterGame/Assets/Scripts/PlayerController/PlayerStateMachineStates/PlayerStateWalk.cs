//player state that deals with walking, running, and idling
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerStateWalk : PlayerState
{
    private float playerSpeed = 5.5f;
    private float lastSpeed = 0f;

    private const float WALK_SPEED = 4.8f;
    private const float RUN_SPEED = 6.5f;
    private const float TURN_SPEED = 10f;
    private const float BLEND_RATE = 6f;

    public override void OnStateEnter()
    {
        
    }

    public override void OnStateExit()
    {
        anim.SetFloat("locomotionBlend", 0);
    }

    public override void StateUpdate()
    {
        //set correct speed
        if (controller.isGrounded)
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
            playerSpeed = lastSpeed;
        }

        //Debug.Log(playerSpeed);
        
        controller.Move(GetMotion());

        TransitionCheck();
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
        Vector2 moveInput = PlayerManager.playerControllerInput.moveInput;
        Vector3 forward = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);

        Vector3 right = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.RIGHT);


        horizontalMotion = (((forward * moveInput.y) + (right * moveInput.x)).normalized * playerSpeed);

        RaycastHit floorHitInfo;
        
        if (Physics.Raycast(controller.transform.position + Vector3.up * 0.1f, Vector3.down, out floorHitInfo, 0.15f))//slope checking and movement alignment to slope
        {
            horizontalMotion = Vector3.ProjectOnPlane(horizontalMotion, floorHitInfo.normal).normalized * playerSpeed;
        }

        //debugstuff
        //Debug.DrawRay(controller.transform.position + Vector3.up * 0.1f, Vector3.down * 0.15f, Color.blue, 0.1f);//downwards slope checking ray drawer
        //Debug.DrawRay(controller.transform.position + Vector3.up, horizontalMotion.normalized, Color.red, 0.1f);//movement vector ray drawer


        SetAnimatorMotionParameters(horizontalMotion);//set animation parameters for locomotion animations
        SetBodyDirection(moveInput);//set what direction the body should be facing

    }

    //sets which direction the body faces
    private void SetBodyDirection(Vector3 motion)
    {
        if (motion.magnitude > 0)//moving horizontally
        {

            Vector3 forwardRelativeInput = motion.y * PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);
            Vector3 rightRelativeInput = motion.x * PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.RIGHT);

            Vector3 cameraRelativeMove = (forwardRelativeInput + rightRelativeInput).normalized;
            Quaternion quat = Quaternion.LookRotation(cameraRelativeMove, Vector3.up);

            playerBody.rotation = Quaternion.Lerp(playerBody.rotation, quat, TURN_SPEED * Time.deltaTime);
        }

    }

    //sets the animation parameters given the input
    private void SetAnimatorMotionParameters(Vector3 motion)
    {
        if (playerSpeed == RUN_SPEED && motion.magnitude > 0)
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 1, BLEND_RATE * Time.deltaTime));
        }
        else if (playerSpeed == WALK_SPEED && motion.magnitude > 0)
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 0.5f, BLEND_RATE * Time.deltaTime));
        }
        else
        {
            anim.SetFloat("locomotionBlend", Mathf.Lerp(anim.GetFloat("locomotionBlend"), 0, BLEND_RATE * Time.deltaTime));
        }
    }

    //updates the vertical (y) axis movement
    private void UpdateVerticalMotion()
    {
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;
        

        if (controller.isGrounded)
        {
            verticalMotion = Vector3.up * GRAVITY * 0.20f;
        }
    }

    public override void TransitionCheck()
    {
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


    }
}
