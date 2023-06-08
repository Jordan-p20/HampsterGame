using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerAnimationEventHelper : MonoBehaviour
{
    public void SpawnGrappleProjectile()
    {
        PlayerStateAimGrapple state = (PlayerStateAimGrapple)PlayerManager.playerTransform.GetComponent<PlayerControllerStateMachine>().GetCurrentPlayerState();
        state.ThrowGrapple();
    }
}
