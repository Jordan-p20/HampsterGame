using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAttack : PlayerState
{
    private float animLength;//the animation length
    private float elapsedTime = 0f;//how long the state has been active fr
    private int combo = 1;// how many times the player has attacked in a row
    private const int maxCombo = 3;// the max amount of times the player can attack in a row

    public override void OnStateEnter()
    {
        anim.SetBool("Attack", true);
        animLength = 1.0f;//animLengths["Attack"] * 0.5f;
    }

    public override void OnStateExit()
    {
        anim.SetBool("Attack", false);
    }

    public override void StateUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (PlayerManager.playerCameraMovement.lockedOn)//moves the player towards their locked target when attacking, will also need a raycast to determine if its required to move towards them
        {
            Vector3 target = PlayerManager.playerCameraMovement.GetLockOnTarget().position;
            target.y = PlayerManager.playerTransform.position.y;
            PlayerManager.playerTransform.position = Vector3.MoveTowards(PlayerManager.playerTransform.position, target, Time.deltaTime * 2);
        }
    }

    public override void TransitionCheck()
    {
        if (PlayerManager.playerControllerInput.middleMousePressed && controller.isGrounded)
        {
            SM.TransitionState(PlayerStates.ROLL);
            return;
        }

        if (elapsedTime >= animLength)
        {
            SM.TransitionState(PlayerStates.WALK);
            return;
        }

        if (elapsedTime >= animLength * 0.5f && combo < maxCombo && PlayerManager.playerControllerInput.attackPressed)
        {
            Debug.Log(combo);
            elapsedTime= 0f;
            combo++;
            if(combo == 2)
            {
                anim.CrossFade("AttackTwo", 0.1f, 0, 0);
            }
            else
            {
                anim.CrossFade("AttackOne", 0.1f, 0, 0);
            }

            //anim.CrossFade("AttackTwo", 0.1f, 0, 0);
            return;
        }
    }
}
