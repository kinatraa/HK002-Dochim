using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class InitObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _tilePrefab;
    private Queue<SpriteRenderer> _tilePool = new Queue<SpriteRenderer>();
    [SerializeField] private GameObject _tilemapPrefab;
    private Queue<Tilemap> _tilemapPool = new Queue<Tilemap>();
    [SerializeField] private List<Transform> _specialEffectPool;

    void Awake()
    {
        for (int i = 0; i < 128; i++)
        {
            GameObject obj = Instantiate(_tilePrefab, transform);
            _tilePool.Enqueue(obj.GetComponent<SpriteRenderer>());
            obj.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(_tilemapPrefab, transform);
            _tilemapPool.Enqueue(obj.GetComponent<Tilemap>());
            obj.SetActive(false);
        }
    }

    public Queue<SpriteRenderer> GetTilePool()
    {
        return _tilePool;
    }

    public Queue<Tilemap> GetTilemapPool()
    {
        return _tilemapPool;
    }

    public Queue<Transform> GetSpecialEffectPool()
    {
        return new Queue<Transform>(_specialEffectPool);
    }
}
