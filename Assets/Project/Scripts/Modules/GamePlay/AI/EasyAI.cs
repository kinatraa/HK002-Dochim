using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EasyAI : MonoBehaviour, IAIBehavior
{
    private DiamondClick _diamondClick;
    private BoundsInt _bounds;

    private List<Tuple<Vector3Int, Vector3Int>> _swapableTiles = new List<Tuple<Vector3Int, Vector3Int>>();
    
    private Vector3Int[] directions = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
    };
    
    void Start()
    {
        _bounds = GamePlayManager.Instance.BoardBounds;
    }

    public void SetDiamondClick(DiamondClick diamondClick)
    {
        _diamondClick = diamondClick;
    }

    public IEnumerator SelectTile()
    {
        _swapableTiles.Clear();
        _swapableTiles = GetSwapableTiles();
        Debug.Log(_swapableTiles.Count);
        
        int randomIdx = Random.Range(0, _swapableTiles.Count);

        yield return _diamondClick.SelectTile(_swapableTiles[randomIdx].Item1);
        yield return new WaitForSeconds(1f);
        yield return _diamondClick.SelectTile(_swapableTiles[randomIdx].Item2);
        
        /*
        GamePlayManager.Instance.GameTurnController.ChangeTurn();
        */
        
        yield return null;
    }

    public List<Tuple<Vector3Int, Vector3Int>> GetSwapableTiles()
    {
        List<Tuple<Vector3Int, Vector3Int>> list = new List<Tuple<Vector3Int, Vector3Int>>();
        Vector3Int curTile = Vector3Int.zero;
        
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                curTile.x = x;
                curTile.y = y;
                
                foreach (Vector3Int dir in directions)
                {
                    if(!_diamondClick.ValidClick(curTile + dir)) continue;
                    if(!_diamondClick.CanSwap(curTile, curTile + dir, 1)) continue;
                    
                    list.Add(Tuple.Create(curTile, curTile + dir));
                }
            }
        }
        
        return list;
    }
}
