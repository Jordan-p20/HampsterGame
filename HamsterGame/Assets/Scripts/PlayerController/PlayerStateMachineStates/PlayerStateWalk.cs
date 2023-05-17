//player state that deals with walking, running, and idling
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

    private Vector3 GetMotion()
    {
        UpdateVerticalMotion();
        UpdateHorizontalMotion();
        return (horizontalMotion + verticalMotion) * Time.deltaTime;
    }

    private void UpdateHorizontalMotion()
    {
        Vector2 moveInput = PlayerManager.playerControllerInput.moveInput;
        Vector3 forward = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.FORWARD);

        Vector3 right = PlayerManager.playerCameraMovement.GetDirectionVector(CamDirection.RIGHT);

        horizontalMotion = (((forward * moveInput.y) + (right * moveInput.x)).normalized * playerSpeed);

        SetAnimatorMotionParameters(horizontalMotion);

        SetBodyDirection(moveInput);

    }

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

    private void UpdateVerticalMotion()
    {
        verticalMotion += Vector3.up * GRAVITY * Time.deltaTime;
        

        if (controller.isGrounded)
        {
            verticalMotion = Vector3.up * GRAVITY * 0.05f;
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
