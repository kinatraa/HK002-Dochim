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
    
    protected Dictionary<TileBase, TileProperties> _tilesData = new Dictionary<TileBase, TileProperties>();
    protected List<(Vector3Int, Vector3Int)> _possibleMoves = new List<(Vector3Int, Vector3Int)>();
    protected List<Dictionary<TileColor, int>> colorCounter = new List<Dictionary<TileColor, int>>();
    
    private Queue<Tilemap> _tilemapPool = new Queue<Tilemap>();
    
    void Start()
    {
        _pretendDiamondManager = GamePlayManager.Instance.PretendDiamondManager;
        _bounds = GamePlayManager.Instance.BoardBounds;
        
        _tilesData = GamePlayManager.Instance.TilesData;
        _tilemapPool = GamePlayManager.Instance.TilemapPool;
    }
    
    public void SetDiamondClick(DiamondClick diamondClick)
    {
        _diamondClick = diamondClick;
    }

    public virtual IEnumerator SelectTile()
    {
        while (GamePlayManager.Instance.State != GameState.OpponentTurn)
        {
            yield return null;
        }

        _possibleMoves.Clear();
        _possibleMoves = Utils.GetAllPossibleMoves(GamePlayManager.Instance.Tilemap);
        
        yield return null;
    }

    protected virtual int GetMove()
    {
        return 0;
    }
}
