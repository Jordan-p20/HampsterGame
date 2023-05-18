using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;//singleton of player manager
    public static Transform playerTransform { private set; get; }//players transform
    public static PlayerControllerInput playerControllerInput { private set; get; } // players input component
    public static PlayerControllerCameraMovement playerCameraMovement { private set; get; }//players camera movement component

    public static Animator playerAnimator { private set; get; }//players animator

    private void Awake()
    {
        if (instance == null) instance = this;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerControllerInput = playerTransform.GetComponent<PlayerControllerInput>();
        playerCameraMovement = playerTransform.GetChild(0).GetComponent<PlayerControllerCameraMovement>();
        playerAnimator = GameObject.FindGameObjectWithTag("PlayerAnim").GetComponent<Animator>();
        
        //should be moved into game manager or something
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
