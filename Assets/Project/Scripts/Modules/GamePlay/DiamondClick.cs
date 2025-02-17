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

    public void SelectTile(Vector3Int selectedPos)
    {
        if (_selected)
        {
            _selected = false;
            _bordermap.SetTile(_selectedTile, null);
            if (GamePlayManager.Instance.ValidClick(selectedPos) && CheckAdjacentVector(_selectedTile, selectedPos))
            {
                SwapTile(_selectedTile, selectedPos);
                    
                if (!CanSwap(_selectedTile, selectedPos))
                {
                    SwapTile(_selectedTile, selectedPos);
                }
                else
                {
                    StartCoroutine(_diamondManager.ClearDiamond(0.5f));
                }
                    
                /*StartCoroutine(ClearDiamond());*/
            }
        }
        else
        {
            if (GamePlayManager.Instance.ValidClick(selectedPos))
            {
                _selected = true;
                _selectedTile = selectedPos;
                _bordermap.SetTile(_selectedTile, _borderTile);
            }
        }
    }

    private bool CanSwap(Vector3Int a, Vector3Int b)
    {
        int cnt = 1;
        Vector3Int pos = a;
        while (pos.x - 1 >= _bounds.xMin)
        {
            --pos.x;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(a)) ++cnt;
            else break;
        }
        pos = a;
        while (pos.x + 1 < _bounds.xMax)
        {
            ++pos.x;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(a)) ++cnt;
            else break;
        }
        if (cnt >= 3) return true;

        cnt = 1;
        pos = a;
        while (pos.y - 1 >= _bounds.yMin)
        {
            --pos.y;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(a)) ++cnt;
            else break;
        }
        pos = a;
        while (pos.y + 1 < _bounds.yMax)
        {
            ++pos.y;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(a)) ++cnt;
            else break;
        }
        if (cnt >= 3) return true;
        
        cnt = 1;
        pos = b;
        while (pos.x - 1 >= _bounds.xMin)
        {
            --pos.x;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(b)) ++cnt;
            else break;
        }
        pos = b;
        while (pos.x + 1 < _bounds.xMax)
        {
            ++pos.x;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(b)) ++cnt;
            else break;
        }
        if (cnt >= 3) return true;

        cnt = 1;
        pos = b;
        while (pos.y - 1 >= _bounds.yMin)
        {
            --pos.y;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(b)) ++cnt;
            else break;
        }
        pos = b;
        while (pos.y + 1 < _bounds.yMax)
        {
            ++pos.y;
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(b)) ++cnt;
            else break;
        }
        if (cnt >= 3) return true;

        return false;
    }

    private void SwapTile(Vector3Int a, Vector3Int b)
    {
        TileBase saveTile = _tilemap.GetTile(a);
        _tilemap.SetTile(a, _tilemap.GetTile(b));
        _tilemap.SetTile(b, saveTile);
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
}