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

    private Camera _camera;

    private Vector3Int[] _directions;
    private Vector3Int _selectedTile = default;
    private bool _selected = false;
    
    void Start()
    {
        _camera = Camera.main;
        
        _directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, -1, 0)
        };
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = _tilemap.WorldToCell(worldPoint);
            if (_selected)
            {
                _selected = false;
                _bordermap.SetTile(_selectedTile, null);
                if (_tilemap.GetTile(gridPosition) != null)
                {
                    TileBase saveTile = _tilemap.GetTile(gridPosition);
                    _tilemap.SetTile(gridPosition, _tilemap.GetTile(_selectedTile));
                    _tilemap.SetTile(_selectedTile, saveTile);
                    
                    ClearDiamond();
                }
            }
            else
            {
                _selected = true;
                _selectedTile = gridPosition;
                _bordermap.SetTile(_selectedTile, _borderTile);
            }
        }
    }

    private void ClearDiamond()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        int t = 1;
        while (t-- > 0)
        {
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int curPos = new Vector3Int(x, y, 0);
                    if (visited.Contains(curPos) || _tilemap.GetTile(curPos) == null) continue;
                
                    List<Vector3Int> clearTiles = BFS(visited, curPos);
                    
                    if(clearTiles.Count <= 2) continue;
                    foreach (Vector3Int tilePos in clearTiles)
                    {
                        _tilemap.SetTile(tilePos, null);
                    }
                }
            }
            
        }
        
        DropTile();
    }

    private List<Vector3Int> BFS(HashSet<Vector3Int> visited, Vector3Int curPos)
    {
        List<Vector3Int> clearTiles = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        
        queue.Enqueue(curPos);
        visited.Add(curPos);
        clearTiles.Add(curPos);
                    
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            foreach (Vector3Int dir in _directions)
            {
                Vector3Int next = current + dir;
                TileBase nextTile = _tilemap.GetTile(next);
                if (nextTile == null || nextTile.name != _tilemap.GetTile(curPos).name || visited.Contains(next)) continue;

                queue.Enqueue(next);
                visited.Add(next);
                clearTiles.Add(next);
            }
        }
        
        return clearTiles;
    }

    private void DropTile()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                if (_tilemap.GetTile(new Vector3Int(x, y, 0)) != null) continue;
                Queue<Vector3Int> aboveTiles = new Queue<Vector3Int>();

                GetAboveTiles(aboveTiles, x, y);
                DropAboveTiles(aboveTiles, x, y);
            }
        }
    }

    private void GetAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        BoundsInt bounds = _tilemap.cellBounds;

        for (int k = y + 1; k < bounds.yMax; k++)
        {
            if (_tilemap.GetTile(new Vector3Int(x, k, 0)) == null) continue;

            aboveTiles.Enqueue(new Vector3Int(x, k, 0));
        }
    }

    private void DropAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        while (aboveTiles.Count > 0)
        {
            Vector3Int tile = aboveTiles.Dequeue();
            _tilemap.SetTile(new Vector3Int(x, y++, 0), _tilemap.GetTile(tile));
            _tilemap.SetTile(tile, null);
        }
    }
}