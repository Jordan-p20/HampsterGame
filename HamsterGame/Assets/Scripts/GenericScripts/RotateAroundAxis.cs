using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour
{
    [SerializeField] private Vector3 rotationAxis;
    [SerializeField] private float degreesPerSecond;



    // Update is called once per frame
    void Update()
    {
        transform.localRotation *= Quaternion.Euler(rotationAxis * Time.time * degreesPerSecond * Time.deltaTime);
    }
}
