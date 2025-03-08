using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class DiamondClick : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _bordermap;
    [SerializeField] private TileBase _borderTile;
    
    [SerializeField] private DiamondManager _diamondManager;

    private Camera _camera;
    private BoundsInt _bounds;

    /*private Vector3Int[] _directions;*/
    private Vector3Int _selectedTile;
    private bool _selected = false;

    void Start()
    {
        _camera = Camera.main;
        _tilemap = GamePlayManager.Instance.Tilemap;
        _bordermap = GamePlayManager.Instance.BorderTilemap;
        
        _bounds = GamePlayManager.Instance.BoardBounds;
    }

    public IEnumerator SelectTile(Vector3Int selectedPos)
    {
        if (_selected)
        {
            _selected = false;
            _bordermap.SetTile(_selectedTile, null);
            if (ValidClick(selectedPos) && CheckAdjacentVector(_selectedTile, selectedPos))
            {
                yield return StartCoroutine(_diamondManager.SwapTile(_selectedTile, selectedPos));
                if (!CanSwap(_selectedTile, selectedPos, 0))
                {
                    /*Debug.Log("Can't swap");*/
                    yield return StartCoroutine(_diamondManager.SwapTile(_selectedTile, selectedPos));
                }
                else
                {
                    /*Debug.Log("Swap");*/
                    yield return StartCoroutine(_diamondManager.ClearDiamond(0.5f));
                    GamePlayManager.Instance.GameTurnController.UseAction();
                }
                    
                /*StartCoroutine(ClearDiamond());*/
            }
        }
        else
        {
            if (ValidClick(selectedPos))
            {
                _selected = true;
                _selectedTile = selectedPos;
                _bordermap.SetTile(_selectedTile, _borderTile);
            }
        }
        
        yield return null;
    }

    public bool CanSwap(Vector3Int a, Vector3Int b, int rev)
    {
        if(_diamondManager.IsLocked(a) ||  _diamondManager.IsLocked(b)) { return false; }
        TileBase[] checkTiles = {_tilemap.GetTile(a), _tilemap.GetTile(b)};
        
        int cnt = 1;
        Vector3Int pos = a;
        while (pos.x - 1 >= _bounds.xMin)
        {
            --pos.x;
            if (rev == 1 && pos == b) break;
            if (_tilemap.GetTile(pos) == checkTiles[rev]) ++cnt;
            else break;
        }
        pos = a;
        while (pos.x + 1 < _bounds.xMax)
        {
            ++pos.x;
            if (rev == 1 && pos == b) break;
            if (_tilemap.GetTile(pos) == checkTiles[rev]) ++cnt;
            else break;
        }

        if (cnt >= 3)
        {
            return true;
        }

        cnt = 1;
        pos = a;
        while (pos.y - 1 >= _bounds.yMin)
        {
            --pos.y;
            if (rev == 1 && pos == b) break;
            if (_tilemap.GetTile(pos) == checkTiles[rev]) ++cnt;
            else break;
        }
        pos = a;
        while (pos.y + 1 < _bounds.yMax)
        {
            ++pos.y;
            if (rev == 1 && pos == b) break;
            if (_tilemap.GetTile(pos) == checkTiles[rev]) ++cnt;
            else break;
        }
        if (cnt >= 3)
        {
            return true;
        }
        
        cnt = 1;
        pos = b;
        while (pos.x - 1 >= _bounds.xMin)
        {
            --pos.x;
            if (rev == 1 && pos == a) break;
            if (_tilemap.GetTile(pos) == checkTiles[1 - rev]) ++cnt;
            else break;
        }
        pos = b;
        while (pos.x + 1 < _bounds.xMax)
        {
            ++pos.x;
            if (rev == 1 && pos == a) break;
            if (_tilemap.GetTile(pos) == checkTiles[1 - rev]) ++cnt;
            else break;
        }
        if (cnt >= 3)
        {
            return true;
        }

        cnt = 1;
        pos = b;
        while (pos.y - 1 >= _bounds.yMin)
        {
            --pos.y;
            if (rev == 1 && pos == a) break;
            if (_tilemap.GetTile(pos) == checkTiles[1 - rev]) ++cnt;
            else break;
        }
        pos = b;
        while (pos.y + 1 < _bounds.yMax)
        {
            ++pos.y;
            if (rev == 1 && pos == a) break;
            if (_tilemap.GetTile(pos) == checkTiles[1 - rev]) ++cnt;
            else break;
        }
        if (cnt >= 3)
        {
            return true;
        }

        return false;
    }

    private bool CheckAdjacentVector(Vector3Int a, Vector3Int b)
    {
        int dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        return dist == 1;
    }

    public bool CanClick()
    {
        return !_diamondManager.IsDropping();
    }
    
    public bool ValidClick(Vector3Int v)
    {
        return v.x >= _bounds.xMin && v.x < _bounds.xMax && v.y >= _bounds.yMin && v.y < _bounds.yMax;
    }
}