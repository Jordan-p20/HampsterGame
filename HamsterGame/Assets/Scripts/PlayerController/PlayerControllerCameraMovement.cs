using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerControllerCameraMovement : MonoBehaviour
{
    public bool playerControlled { private set; get; } = true;

    public bool lockedOn = false;
    [SerializeField] private Transform lockedOnTarget;
    [SerializeField] private Vector3 lockOnTargetOriginalPosition;

    [Tooltip("The amount in degrees that the camera is allowed to rotate towards the ground starting from 0."), Range(0, 50)]
    [SerializeField] private float upperBound = 40f;
    [Tooltip("The amount in degrees that the camera is allowed to rotate upwards starting from 0."), Range(0, 60)]
    [SerializeField] private float lowerBound = 30f;

    [Tooltip("Transform that mimics what direction the camera is facing without the unnecessary rotations (only takes from cameras y rotations)")]
    public Transform camLocomotion { get; private set; }

    public float lockOnSpeed = 4f;
    public float lockOnRange = 35f;

    private Transform actualCamera;

    private void Start()
    {
        camLocomotion = PlayerManager.playerTransform.GetChild(1);
        if (lockedOnTarget != null)
        {
            lockOnTargetOriginalPosition = lockedOnTarget.localPosition;
        }
        actualCamera = transform.GetChild(0);
        
    }

    public void Update()
    {
        LockOnTargetCheck();
        UpdateTargetTransform();
        MoveCamera(PlayerManager.playerControllerInput.mouseMovement);
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
        

        //if player has not pressed middle return
        if (!PlayerManager.playerControllerInput.middleMousePressed) { return; }

        if (!lockedOn)//if player is not locked on 
        {
            SetLockOnTarget(FindLockOnTarget());//find and set new target
            if (lockedOnTarget != null)// if target found
            {
                lockedOn = true;
                lockOnTargetOriginalPosition = lockedOnTarget.localPosition;
            }
            else
            {
                lockedOn = false;
                lockedOnTarget = null;
            }
        }
        else
        {
            lockedOn = false;
            lockedOnTarget = null;
        }


        //if target is too far
        if (lockedOn && TooFarFromTarget())
        {
            SetLockOnTarget(null);
            lockedOn = false;
            return;
        }
    }

    //finds the target that is the closest to where the player is looking
    private Transform FindLockOnTarget()
    {
        List<ColliderValueStorage> colliders = GetEligibleTargets();
        
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

        return returnTransform.GetComponent<EnemyTestScript>().lockOnTransform;
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
            Physics.Raycast(transform.position, directionToHit, out rayHitInfo, lockOnRange);
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
            vecRot = ClampCameraXRot(vecRot, upperBound, lowerBound);
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

    public void SetLockOnTarget(Transform newTarget)
    {
        if (lockedOnTarget != null)
        {
            lockedOnTarget.localPosition = lockOnTargetOriginalPosition;
        }
        lockedOnTarget = newTarget;
    }

    public Transform GetLockOnTarget()
    {
        return lockedOnTarget;
    }

   
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
