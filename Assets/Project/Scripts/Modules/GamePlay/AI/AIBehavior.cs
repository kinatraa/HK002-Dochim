using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AIBehavior : MonoBehaviour
{
    protected Tilemap _tilemap;
    protected BoundsInt _bounds;
    protected Vector3Int[] _directions;
    
    protected virtual void Awake()
    {
        _tilemap = GamePlayManager.Instance.Tilemap;
        _bounds = GamePlayManager.Instance.BoardBounds;
        
        _directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, -1, 0)
        };
    }
    
    public abstract List<Vector3Int> SelectTile();
}
