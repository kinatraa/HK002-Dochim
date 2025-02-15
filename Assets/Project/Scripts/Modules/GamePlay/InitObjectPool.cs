using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public List<GameObject> objectPool;

    void Awake()
    {
        for (int i = 0; i < 64; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            objectPool.Add(obj);
            obj.SetActive(false);
        }
    }

    public List<GameObject> GetObjectPool()
    {
        return objectPool;
    }
}
