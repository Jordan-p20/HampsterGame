using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract class for player states
public abstract class PlayerState
{
    //constants
    protected const float GRAVITY = -9.8f * 2.25f;

    protected CharacterController controller;
    protected Animator anim;
    protected Transform playerBody;
    protected PlayerControllerStateMachine SM;
    protected Dictionary<string, float> animLengths;
    public Vector3 horizontalMotion { get; protected set; }
    public Vector3 verticalMotion { get; protected set; }

    //called when creating a new state to initialize references
    public virtual void Initialize(PlayerStateMachineData SMData)
    {
        controller = SMData.dataCharaController;
        anim = SMData.dataAnimator;
        playerBody = SMData.dataBody;
        SM = SMData.stateMachine;
        animLengths = SMData.dataAnimLengths;
    }

    //called when creating a new state to initialize references, overload with the previous states horizontal movement
    public virtual void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion)
    {
        controller = SMData.dataCharaController;
        anim = SMData.dataAnimator;
        playerBody = SMData.dataBody;
        SM = SMData.stateMachine;
        animLengths = SMData.dataAnimLengths;
    }

    //called when creating a new state to initialize references, overload with the previous states horiontal and vertical movement
    public virtual void Initialize(PlayerStateMachineData SMData, Vector3 previousStateHorMotion, Vector3 previousStateVertMotion)
    {
        controller = SMData.dataCharaController;
        anim = SMData.dataAnimator;
        playerBody = SMData.dataBody;
        SM = SMData.stateMachine;
        animLengths = SMData.dataAnimLengths;
    }

    //called when transitioning into this state
    public abstract void OnStateEnter();

    //called when transitioning out of this state
    public abstract void OnStateExit();

    //called every frame that this state is the current active state
    public abstract void StateUpdate();

    //called every frame to see if this state should transition into another state
    public abstract void TransitionCheck();


}
