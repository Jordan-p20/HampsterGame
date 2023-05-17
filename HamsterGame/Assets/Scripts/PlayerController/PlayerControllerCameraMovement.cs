using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCameraMovement : MonoBehaviour
{
    public bool playerControlled { private set; get; } = true;
    [Tooltip("The amount in degrees that the camera is allowed to rotate towards the ground starting from 0."), Range(0, 50)]
    [SerializeField] private float upperBound = 40f;
    [Tooltip("The amount in degrees that the camera is allowed to rotate upwards starting from 0."), Range(0, 60)]
    [SerializeField] private float lowerBound = 30f;

    public Transform camLocomotion { get; private set; }

    private void Start()
    {
        camLocomotion = PlayerManager.playerTransform.GetChild(1);
    }

    public void Update()
    {
        if (playerControlled) MoveCamera(PlayerManager.playerControllerInput.mouseMovement);
    }

    public Vector3 GetDirectionVector(CamDirection direction)
    {
        switch (direction)
        {
            case CamDirection.UP:
                return camLocomotion.up;
            case CamDirection.RIGHT:
                return camLocomotion.right;
            case CamDirection.FORWARD:
                return camLocomotion.forward;
            default:
                return Vector3.zero;
        }

    }

    public void MoveCamera(Vector2 movement)
    {
        Vector3 vecRot = transform.localEulerAngles + new Vector3(movement.y, movement.x, 0);
        vecRot = ClampCameraXRot(vecRot, upperBound, lowerBound);
        transform.rotation = Quaternion.Euler(vecRot);

        UpdateCamDirLocomotion();
        
    }



    private void UpdateCamDirLocomotion()
    {
        camLocomotion.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    public void SetPlayerControl(bool control)
    {
        playerControlled = control;
    }

    private Vector3 ClampCameraXRot(Vector3 vec, float upperBounds, float lowerBounds)
    {
        if (vec.x > upperBounds && vec.x < 180)
        {
            vec.x = upperBounds;
        }
        else if (vec.x < 360 - lowerBounds && vec.x > 181)
        {
            vec.x = 360 - lowerBounds;
        }

        return vec;
    }

}

public enum CamDirection
{
    UP, RIGHT, FORWARD
}

