using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerControllerStateMachine : MonoBehaviour
{
    /*
     * this class controls what state the player is in and how states are controlled and transitioned
     */

    private PlayerControllerCameraMovement camMovement;//camera movement reference

    private PlayerControllerInput playerInput;// input reference

    [SerializeField] private CharacterController SMCharaController;//serialized for debug drawing doesnt need to be for actual play
    
    private Animator SMCharacterAnimator;//animator

    private Transform playerBody;//transform of the players body

    private Dictionary<string, float> animTableLengths = new Dictionary<string, float>();//animation lengths

    //current state
    [SerializeField] private PlayerStates curPlayerStateFlag = PlayerStates.NONE;//current state the player is in flag (mainly for check what state the player is in so you dont have to use gettype)
    private PlayerState curPlayerState;// the state the player is currently in

    [Tooltip("Whether to draw a player hitbox gizmo")]
    [SerializeField] private bool DrawGizmos = true;


    private void Start()
    {
        camMovement = PlayerManager.playerCameraMovement;
        playerInput = PlayerManager.playerControllerInput;

        SMCharaController = GetComponent<CharacterController>();
        SMCharacterAnimator = PlayerManager.playerAnimator;
        playerBody = PlayerManager.playerTransform.GetChild(2);

        curPlayerState = new PlayerStateWalk();
        curPlayerStateFlag = PlayerStates.WALK;
        curPlayerState.Initialize(GetSMData());
        curPlayerState.OnStateEnter();

        foreach (AnimationClip clip in SMCharacterAnimator.runtimeAnimatorController.animationClips)
        {
            if (!animTableLengths.ContainsKey(clip.name))
            {
                animTableLengths.Add(clip.name, clip.length);
            }
            
        }

    }

    private void Update()
    {

        //teleport to top of tower
        if (Input.GetKeyDown("r"))
        {
            SMCharaController.gameObject.SetActive(false);
            transform.position = new Vector3(-23.9209995f, 33.9749985f, -2.69000006f);
            SMCharaController.gameObject.SetActive(true);
        }

        //teleport to ground
        if (Input.GetKeyDown("e"))
        {
            SMCharaController.gameObject.SetActive(false);
            transform.position = new Vector3(-18.3999996f, 0, -7.21000004f);
            SMCharaController.gameObject.SetActive(true);
        }

        curPlayerState.StateUpdate();
        curPlayerState.TransitionCheck();
    }


    //transitions from one state to another
    public void TransitionState(PlayerStates newState)
    {
        if (curPlayerStateFlag == newState) { return;  }//return if new state is same as old state

        Debug.Log("TRANSITIONING TO " + newState);

        Vector3 oldHorMotion = curPlayerState.horizontalMotion;
        Vector3 oldVertMotion = curPlayerState.verticalMotion;

        curPlayerState.OnStateExit();//call exit on old state for clean up

        switch (newState)//select new state
        {
            case PlayerStates.WALK:
                curPlayerState = new PlayerStateWalk();
                curPlayerState.Initialize(GetSMData());
                break;
            case PlayerStates.JUMP:
                curPlayerState = new PlayerStateJump();
                curPlayerState.Initialize(GetSMData(), oldHorMotion);
                break;
            case PlayerStates.FALL:
                curPlayerState = new PlayerStateFall();
                curPlayerState.Initialize(GetSMData(), oldHorMotion, oldVertMotion);
                break;
            case PlayerStates.HARD_LAND:
                curPlayerState = new PlayerStateHardLand();
                curPlayerState.Initialize(GetSMData(), oldHorMotion, oldVertMotion);
                break;
            case PlayerStates.ATTACK:
                curPlayerState = new PlayerStateAttack();
                curPlayerState.Initialize(GetSMData());
                break;
            case PlayerStates.ROLL:
                curPlayerState = new PlayerStateRoll();
                curPlayerState.Initialize(GetSMData(), oldHorMotion);
                break;
            case PlayerStates.AIM_GRAPPLE:
                curPlayerState = new PlayerStateAimGrapple();
                curPlayerState.Initialize(GetSMData());
                break;
            case PlayerStates.GRAPPLE_MOVE:
                curPlayerState = new PlayerStateGrappleMove();
                curPlayerState.Initialize(GetSMData());
                break;
        }
        
        curPlayerStateFlag = newState;//set flag of new state

        curPlayerState.OnStateEnter();//call enter on new state
    }

    //returns a struct of references states might need to be initialized with
    public PlayerStateMachineData GetSMData()
    {
        return new PlayerStateMachineData(SMCharaController, SMCharacterAnimator, playerBody, this, animTableLengths);
    }

    public PlayerStates GetCurrentStateFlag()
    {
        return curPlayerStateFlag;
    }

    public PlayerState GetCurrentPlayerState()
    {
        return curPlayerState;
    }

    public void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            Gizmos.DrawWireCube(SMCharaController.center + transform.position, new Vector3(SMCharaController.radius * 2, SMCharaController.height, SMCharaController.radius * 2));
        }
        
    }
}

public enum PlayerStates
{
    NONE,
    WALK,
    JUMP,
    FALL,
    HARD_LAND,
    ROLL,
    ATTACK,
    AIM_GRAPPLE,
    GRAPPLE_MOVE
}

public struct PlayerStateMachineData
{
    public PlayerStateMachineData(CharacterController SMController, Animator SMAnimator, Transform body, PlayerControllerStateMachine sm, Dictionary<string, float> animLengths)
    {
        dataCharaController = SMController;
        dataAnimator = SMAnimator;
        dataBody = body;
        stateMachine = sm;
        dataAnimLengths = animLengths;
    }

    public CharacterController dataCharaController;
    public Animator dataAnimator;
    public Transform dataBody;
    public PlayerControllerStateMachine stateMachine;
    public Dictionary<string, float> dataAnimLengths;
}
