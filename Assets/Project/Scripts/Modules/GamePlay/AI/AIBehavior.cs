using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AIBehavior : MonoBehaviour
{
    protected PretendDiamondManager _pretendDiamondManager;
    protected DiamondClick _diamondClick;
    protected BoundsInt _bounds;
    
    protected Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
    };
    
    private Dictionary<TileBase, TileProperties> _tilesData = new Dictionary<TileBase, TileProperties>();
    protected List<(Vector3Int, Vector3Int)> _swappableTiles = new List<(Vector3Int, Vector3Int)>();
    protected List<Dictionary<TileColor, int>> colorCounter = new List<Dictionary<TileColor, int>>();
    
    void Start()
    {
        _pretendDiamondManager = GamePlayManager.Instance.PretendDiamondManager;
        _bounds = GamePlayManager.Instance.BoardBounds;
        
        _tilesData = GamePlayManager.Instance.TilesData;
    }
    
    public void SetDiamondClick(DiamondClick diamondClick)
    {
        _diamondClick = diamondClick;
    }

    public virtual IEnumerator SelectTile()
    {
        _swappableTiles.Clear();
        _swappableTiles = GetSwappableTiles();
        
        yield return null;
    }
    
    private List<(Vector3Int, Vector3Int)> GetSwappableTiles()
    {
        List<(Vector3Int, Vector3Int)> list = new List<(Vector3Int, Vector3Int)>();
        Vector3Int curTile = Vector3Int.zero;
        
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                curTile.x = x;
                curTile.y = y;
                
                foreach (Vector3Int dir in directions)
                {
                    if(!GamePlayManager.Instance.IsInBound(curTile + dir)) continue;
                    if(!_diamondClick.CanSwap(curTile, curTile + dir, 1)) continue;
                    
                    list.Add((curTile, curTile + dir));
                }
            }
        }
        
        return list;
    }
    
    
}
