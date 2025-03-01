using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    private Queue<SpriteRenderer> _objectPool = new Queue<SpriteRenderer>();

    void Awake()
    {
        for (int i = 0; i < 128; i++)
        {
            GameObject obj = Instantiate(_prefab, transform);
            _objectPool.Enqueue(obj.GetComponent<SpriteRenderer>());
            obj.SetActive(false);
        }
    }

    public Queue<SpriteRenderer> GetObjectPool()
    {
        return _objectPool;
    }
}
