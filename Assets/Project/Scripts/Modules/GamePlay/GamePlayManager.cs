using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HaKien;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GamePlayManager : Singleton<GamePlayManager>, IMessageHandle
{
    [SerializeField] public int _rows = 8;
    [SerializeField] public int _columns = 8;
    
    [SerializeField] private Tilemap _tilemap;
    public Tilemap Tilemap
    {
        get => _tilemap;
        set => _tilemap = value;
    }
    
    [SerializeField] private Tilemap _borderTilemap;
    public Tilemap BorderTilemap
    {
        get => _borderTilemap;
        set => _borderTilemap = value;
    }

    public BoundsInt _bounds;
    public BoundsInt BoardBounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    private void Awake()
    {
        _bounds.xMin = -_columns / 2;
        _bounds.xMax = _columns / 2;
        _bounds.yMin = -_rows / 2;
        _bounds.yMax = _rows / 2;
    }

    private void OnDestroy()
    {
        
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void Handle(Message message)
    {
        throw new NotImplementedException();
    }
    
    public bool ValidClick(Vector3Int v)
    {
        return v.x >= _bounds.xMin && v.x < _bounds.xMax && v.y >= _bounds.yMin && v.y < _bounds.yMax;
    }
}