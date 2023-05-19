using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    [SerializeField] private Transform lockOnTransform;
    [SerializeField] private GameObject lockOnIcon;

    public Transform GetLockOnTransform()
    {
        return lockOnTransform;
    }

    public void UnLockOn()
    {
        lockOnIcon.SetActive(false);
        
    }

    public void LockOn()
    {
        lockOnIcon.SetActive(true);
        
    }
}
