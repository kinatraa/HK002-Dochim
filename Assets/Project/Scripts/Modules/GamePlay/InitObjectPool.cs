using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public List<SpriteRenderer> objectPool;

    void Awake()
    {
        for (int i = 0; i < 128; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            objectPool.Add(obj.GetComponent<SpriteRenderer>());
            obj.SetActive(false);
        }
    }

    public List<SpriteRenderer> GetObjectPool()
    {
        return objectPool;
    }
}
