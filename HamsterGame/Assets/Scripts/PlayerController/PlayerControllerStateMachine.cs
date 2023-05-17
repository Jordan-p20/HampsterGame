using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerControllerStateMachine : MonoBehaviour
{
    private PlayerControllerCameraMovement camMovement;

    private PlayerControllerInput playerInput;

    [SerializeField] private CharacterController SMCharaController;//serialized for debug drawing doesnt need to be for actual play

    private Animator SMCharacterAnimator;

    private Transform playerBody;

    private Dictionary<string, float> animTableLengths = new Dictionary<string, float>();

    //current state
    [SerializeField] private PlayerStates curPlayerStateFlag = PlayerStates.WALK;
    private PlayerState curPlayerState;

    [SerializeField] private bool DrawGizmos = true;


    private void Start()
    {
        camMovement = PlayerManager.playerCameraMovement;
        playerInput = PlayerManager.playerControllerInput;

        SMCharaController = GetComponent<CharacterController>();
        SMCharacterAnimator = PlayerManager.playerAnimator;
        playerBody = PlayerManager.playerTransform.GetChild(2);

        curPlayerState = new PlayerStateWalk();
        curPlayerState.Initialize(GetSMData());


        foreach (AnimationClip clip in SMCharacterAnimator.runtimeAnimatorController.animationClips)
        {
            animTableLengths.Add(clip.name, clip.length);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown("t"))
        {
            Debug.Break();
        }

        if (Input.GetKeyDown("r"))
        {
            SMCharaController.gameObject.SetActive(false);
            transform.position = new Vector3(-23.9209995f, 33.9749985f, -2.69000006f);
            SMCharaController.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown("e"))
        {
            SMCharaController.gameObject.SetActive(false);
            transform.position = new Vector3(-18.3999996f, 0, -7.21000004f);
            SMCharaController.gameObject.SetActive(true);
        }

        curPlayerState.StateUpdate();
    }


    public void TransitionState(PlayerStates newState)
    {
        Debug.Log("TRANSITIONING TO " + newState);

        if (curPlayerStateFlag == newState) { return;  }//return if new state is same as old state

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
                curPlayerState.Initialize(GetSMData());
                break;
            case PlayerStates.ATTACK:
                curPlayerState = new PlayerStateAttack();
                curPlayerState.Initialize(GetSMData());
                break;
        }
        
        curPlayerStateFlag = newState;//set flag of new state

        curPlayerState.OnStateEnter();//call enter on new state
    }

    public PlayerStateMachineData GetSMData()
    {
        return new PlayerStateMachineData(SMCharaController, SMCharacterAnimator, playerBody, this, animTableLengths);
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
    WALK,
    JUMP,
    FALL,
    HARD_LAND,
    ROLL,
    ATTACK
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
