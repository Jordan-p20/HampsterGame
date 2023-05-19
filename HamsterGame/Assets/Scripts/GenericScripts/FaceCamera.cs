using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        //StartCoroutine(LookAtCamera());
    }

    private void Update()
    {
        transform.LookAt(mainCam);
    }

    private IEnumerator LookAtCamera()
    {
        while(true)
        {
            transform.LookAt(mainCam);
            yield return null;
        }
        
    }
}
