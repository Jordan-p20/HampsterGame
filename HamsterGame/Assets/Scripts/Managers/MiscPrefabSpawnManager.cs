using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscPrefabSpawnManager : MonoBehaviour
{

    [SerializeField] private GameObject[] m_Prefab;

    public static MiscPrefabSpawnManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;

    }

    public GameObject GetNewPrefabGO(MiscPrefab prefabType)
    {
        return GameObject.Instantiate(m_Prefab[(int)prefabType], Vector3.zero, Quaternion.identity);
    }

    
}

public enum MiscPrefab
{
    GrappleProjectile
}
