using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    public static MainUIManager instance;
    public Transform aimReticle { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this; 
        aimReticle = transform.GetChild(0);
    }

    public void ShowAimReticle()
    {
        aimReticle.gameObject.SetActive(true);
    }

    public void HideAimReticle()
    {
        aimReticle.gameObject.SetActive(false);
    }
}
