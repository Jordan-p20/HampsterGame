using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerCameraMovement : MonoBehaviour
{
    public bool playerControlled { private set; get; } = true;//if the player has control over the camera

    [Tooltip("Whether or not the player is currently locked on to a target")]
    public bool lockedOn = false;
    [SerializeField] private Transform lockedOnTarget;
    [SerializeField] private Vector3 lockOnTargetOriginalPosition;

    [Tooltip("The amount in degrees that the camera is allowed to rotate towards the ground starting from 0."), Range(0, 50)]
    [SerializeField] private float upperBound = 40f;
    [Tooltip("The amount in degrees that the camera is allowed to rotate upwards starting from 0."), Range(0, 60)]
    [SerializeField] private float lowerBound = 30f;

    [Tooltip("Transform that mimics what direction the camera is facing without the unnecessary rotations (only takes from cameras y rotations)")]
    public Transform camLocomotion { get; private set; }

    [Tooltip("how fast the camera moves to the locked on target")]
    public float lockOnSpeed = 4f;
    [Tooltip("how far away a locked on target can be before you cant lock on/ lock on disengages")]
    public float lockOnRange = 35f;

    public Transform actualCamera { get; private set; }// the transform of the actual camera object 

    [SerializeField] private LockOnTarget lockedOnComponent;//the lockOnTarget component of the locked on target

    private Coroutine currentCoroutine;//hold the currently acting coroutine, null if not using one
    [Tooltip("the position that the camera should be from the camera holder when in the close setting")]
    [SerializeField] private Vector3 closeCameraPos;

    [Tooltip("the position that the camera should be from the camera holder when in the medium setting")]
    [SerializeField] private Vector3 mediumCameraPos;

    [Tooltip("the position that the camera should be from the camera holder when in the far setting")]
    [SerializeField] private Vector3 farCameraPos;

    [SerializeField] private float cameraZoomSpeed;

    private bool clampCamera = true;//whether or not to clamp the cameras rotation

    private void Start()
    {
        camLocomotion = PlayerManager.playerTransform.GetChild(1);
        if (lockedOnTarget != null)
        {
            lockOnTargetOriginalPosition = lockedOnTarget.localPosition;
        }
        actualCamera = transform.GetChild(0);

        Debug.Log(Vector3.Distance(transform.position, actualCamera.position));
    }

    public void Update()
    {
        if (!playerControlled) return;

        if (Input.GetKeyDown("t")) ChangeCameraSetting(CamPositionPreset.Close);

        LockOnTargetCheck();
        UpdateTargetTransform();
        MoveCamera(PlayerManager.playerControllerInput.mouseMovement);
    }

    public void ChangeCameraSetting(CamPositionPreset setting)
    {
        if (currentCoroutine != null) StopAllCoroutines();

        switch (setting)
        {
            case CamPositionPreset.Close:
                currentCoroutine =  StartCoroutine(MoveCameraToDistance(closeCameraPos));
                break;

            case CamPositionPreset.Medium:
                currentCoroutine = StartCoroutine(MoveCameraToDistance(mediumCameraPos));
                break;

            case CamPositionPreset.Far:
                currentCoroutine = StartCoroutine(MoveCameraToDistance(farCameraPos));
                break;
        }
    }

    private IEnumerator MoveCameraToDistance(Vector3 newPos)
    {
        Debug.Log("inside coro");
        
        while (actualCamera.localPosition != newPos)
        {
            actualCamera.localPosition = Vector3.MoveTowards(actualCamera.localPosition, newPos, cameraZoomSpeed * Time.deltaTime);
            yield return null;
        }
        currentCoroutine = null;
    }

    //updates the locked on targets lockon transform based on distance to the player
    private void UpdateTargetTransform()
    {
        if (!lockedOn) { return; }

        float distance = Vector3.Distance(lockedOnTarget.position, transform.position);
        lockedOnTarget.localPosition = lockOnTargetOriginalPosition - (Vector3.up * lockOnTargetOriginalPosition.y * (1 - (distance / lockOnRange)));
    }

    //calculates if the locked on target is farther than lockOnRange 
    private bool TooFarFromTarget()
    {
        return Vector3.Distance(transform.position, lockedOnTarget.position) > lockOnRange;
    }

    //checks if the player is trying to unlock/lock on to a target, and resolves that input
    private void LockOnTargetCheck()
    {
        //if target is too far
        if (lockedOn && TooFarFromTarget())
        {
            ResetLockOnValues();
            return;
        }

        //if player has not pressed middle return
        if (!PlayerManager.playerControllerInput.tabPressed) { return; }

        if (!lockedOn)//if player is not locked on 
        {
            SetLockOnTarget(FindLockOnTarget());//find and set new target
            if (lockedOnTarget != null)// if target found
            {
                lockedOn = true;
                lockOnTargetOriginalPosition = lockedOnTarget.localPosition;
                lockedOnComponent = lockedOnTarget.parent.GetComponent<LockOnTarget>();
                lockedOnComponent.LockOn();
                PlayerManager.playerAnimator.SetBool("isLockedOn", true);
                
            }
            else
            {
                ResetLockOnValues();
            }
        }
        else
        {
            ResetLockOnValues();
        }
    }

    //resets lock on varaibles
    private void ResetLockOnValues()
    {
        SetLockOnTarget(null);
        if (lockedOnComponent)
        {
            lockedOnComponent.UnLockOn();
            lockedOnComponent = null;
        }
        lockedOn = false;
        PlayerManager.playerAnimator.SetBool("isLockedOn", false);
    }

    //finds the target that is the closest to where the player is looking
    private Transform FindLockOnTarget()
    {
        List<ColliderValueStorage> colliders = GetEligibleTargets();

        if (colliders.Count < 1) { return null; }
        
        return FindBestEligibleTarget(colliders);
    }

    //selects a target from a list of possible targets based on how close they are to where the player is looking
    private Transform FindBestEligibleTarget(List<ColliderValueStorage> colliders)
    {
        Transform returnTransform = null;
        float bestValue = 0;
        foreach (ColliderValueStorage storage in colliders)
        {
            if (storage.value > bestValue)
            {
                bestValue = storage.value;
                returnTransform = storage.trans;
            }
        }

        return returnTransform.GetComponent<LockOnTarget>().GetLockOnTransform();
    }

    //creates a list of all objects that could be the intended lock on target
    private List<ColliderValueStorage> GetEligibleTargets()
    {
        Collider[] castHits = Physics.OverlapSphere(transform.position, lockOnRange);
        List<ColliderValueStorage> colliderValues = new List<ColliderValueStorage>();

        foreach (Collider hit in castHits)
        {
            if (!hit.gameObject.CompareTag("Enemy")) { continue; }

            Vector3 directionToHit = (hit.transform.position - transform.position).normalized;
            float angleToHit = Vector3.Dot(actualCamera.forward, directionToHit);
            if (angleToHit < 0) { continue; }

            RaycastHit rayHitInfo;
            if (!Physics.Raycast(transform.position + (Vector3.up * 1.4f), directionToHit, out rayHitInfo, lockOnRange))
            {
                continue;
            }

            //Debug.DrawRay(transform.position, directionToHit * 15, Color.red, 10f);

            if (!rayHitInfo.collider.gameObject.CompareTag("Enemy")) { continue; }

            colliderValues.Add(new ColliderValueStorage(hit.transform, angleToHit));
        }

        return colliderValues;
    }


    //returns a Vector of what the camLocation direction is
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

    //rotates the camera by movement
    public void MoveCamera(Vector2 movement)
    {
        if (lockedOn)
        {
            Quaternion targetQuat = Quaternion.LookRotation(lockedOnTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetQuat, Time.deltaTime * lockOnSpeed);
        }
        else
        {
            Vector3 vecRot = transform.localEulerAngles + new Vector3(movement.y, movement.x, 0);
            if (clampCamera)
            {
                vecRot = ClampCameraXRot(vecRot, upperBound, lowerBound);
            }
            transform.rotation = Quaternion.Euler(vecRot);
        }

        UpdateCamDirLocomotion();
        
    }

    //updates the camlocomotion transform to be up to date with the camera
    private void UpdateCamDirLocomotion()
    {
        camLocomotion.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    //setter for playerControlled
    public void SetPlayerControl(bool control)
    {
        playerControlled = control;
    }

    //clamps the rotation of the camera so the player cannot look from beneath the camera or from too high up
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

    //sets what target the player is locking on to
    public void SetLockOnTarget(Transform newTarget)
    {
        if (lockedOnTarget != null)
        {
            lockedOnTarget.localPosition = lockOnTargetOriginalPosition;
        }
        lockedOnTarget = newTarget;
    }

    //returns what target the player is locking on to
    public Transform GetLockOnTarget()
    {
        return lockedOnTarget;
    }

    public void SetClampCamera(bool setting)
    {
        clampCamera = setting;
    }
}

public enum CamPositionPreset {
    Close,
    Medium,
    Far
}

public enum CamDirection
{
    UP, RIGHT, FORWARD
}

struct ColliderValueStorage
{
    public ColliderValueStorage(Transform data1, float data2)
    {
        trans = data1;
        value = data2;  
    }

    public Transform trans;
    public float value;

}

