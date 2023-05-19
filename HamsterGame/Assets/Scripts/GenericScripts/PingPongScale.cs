using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongScale : MonoBehaviour
{
    [SerializeField] private Vector3 maxScale;
    [SerializeField] private Vector3 minScale;

    private void Update()
    {
        transform.localScale = Vector3.Lerp(maxScale, minScale, Mathf.PingPong(Time.time, 1));

    }

    
}
