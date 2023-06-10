using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerAnimationEventHelper : MonoBehaviour
{
    public void SpawnGrappleProjectile()
    {
        PlayerControllerStateMachine SM = PlayerManager.playerTransform.GetComponent<PlayerControllerStateMachine>();
        if (SM.GetCurrentStateFlag() == PlayerStates.AIM_GRAPPLE)
        {
            PlayerStateAimGrapple state = (PlayerStateAimGrapple)SM.GetCurrentPlayerState();
            state.ThrowGrapple();
        }
        
    }
}
