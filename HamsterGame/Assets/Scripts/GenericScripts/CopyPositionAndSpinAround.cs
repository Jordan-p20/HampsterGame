using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPositionAndSpinAround : MonoBehaviour
{
    [SerializeField] private Transform copyFrom;
    [SerializeField] private float radius = 1;
   

    // Update is called once per frame
    void Update()
    {
        transform.position = copyFrom.position + new Vector3(0, Mathf.Sin(Time.time) * radius, Mathf.Cos(Time.time) * radius);
    }
}
