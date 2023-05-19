using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtAndRotateAroundAxis : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float degreesPerSecond;
    private Transform mainCam;
    private Quaternion rotationOffset = Quaternion.identity;

    private void Start()
    {
        mainCam = Camera.main.transform;
    }

    private void Update()
    {
        transform.LookAt(mainCam);
        rotationOffset *= Quaternion.Euler(rotationAxis * Time.deltaTime * degreesPerSecond);
        transform.localRotation *= rotationOffset;
    }
}
