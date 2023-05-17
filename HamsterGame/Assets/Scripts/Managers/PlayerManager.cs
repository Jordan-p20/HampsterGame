using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public static Transform playerTransform { private set; get; }
    public static PlayerControllerInput playerControllerInput { private set; get; } 
    public static PlayerControllerCameraMovement playerCameraMovement { private set; get; }

    public static Animator playerAnimator { private set; get; }

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
