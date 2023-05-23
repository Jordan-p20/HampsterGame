using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerInput : MonoBehaviour
{
    public Vector2 mouseMovement { get; private set; } = Vector3.zero;
    public Vector2 cameraSense;

    public Vector2 moveInput { get; private set; }
    public bool runHeld {
        get { return shiftHeld; }
        private set { runHeld = value; }
    }

    private bool shiftHeld = false;

    private bool spacePressed = false;

    public bool jumpPressed
    {
        get { return spacePressed; } 
        set { spacePressed = value; }
    }

    public bool attackPressed
    {
        get { return leftMousePressed; }
        set { attackPressed = value; }
    }

    private bool leftMousePressed = false;

    public bool tabPressed = false;

    public bool middleMousePressed { get; private set; } = false;

    public void Update()
    {
        GetMouseInput();
        GetMovementInput();
    }

    private void GetMouseInput()
    {
        mouseMovement = new Vector2((Input.GetAxis("Mouse X") * cameraSense.x), -Input.GetAxis("Mouse Y") * cameraSense.y);

        getMouseButtonInputs();
    }

    private void getMouseButtonInputs()
    {
        leftMousePressed = Input.GetMouseButton(0);
        tabPressed= Input.GetKeyDown(KeyCode.Tab);
        middleMousePressed = Input.GetMouseButtonDown(2);
    }

    private void GetMovementInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        shiftHeld = Input.GetKey(KeyCode.LeftShift);
        jumpPressed = Input.GetKey(KeyCode.Space);
    }
}
