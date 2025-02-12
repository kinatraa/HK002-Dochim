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
    [SerializeField] private InitObjectPool _initObjectPool;
    
    private List<GameObject> _objectPool;

    private Camera _camera;
    private BoundsInt _bounds;

    private Vector3Int[] _directions;
    private Vector3Int _selectedTile = default;
    private bool _selected = false;

    void Start()
    {
        _camera = Camera.main;
        _bounds = _tilemap.cellBounds;

        _objectPool = _initObjectPool.GetObjectPool();

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
                    //Swap tile
                    TileBase saveTile = _tilemap.GetTile(gridPosition);
                    _tilemap.SetTile(gridPosition, _tilemap.GetTile(_selectedTile));
                    _tilemap.SetTile(_selectedTile, saveTile);

                    StartCoroutine(ClearDiamond());
                }
            }
            else
            {
                if (_tilemap.GetTile(gridPosition) != null)
                {
                    _selected = true;
                    _selectedTile = gridPosition;
                    _bordermap.SetTile(_selectedTile, _borderTile);
                }
            }
        }
    }

    private List<Vector3Int> CheckAdjacentTiles(HashSet<Vector3Int> visited, Vector3Int curPos)
    {
        //BFS
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
                if (nextTile == null || nextTile.name != _tilemap.GetTile(curPos).name ||
                    visited.Contains(next)) continue;

                queue.Enqueue(next);
                visited.Add(next);
                clearTiles.Add(next);
            }
        }

        return clearTiles;
    }

    private IEnumerator ClearDiamond()
    {
        int t = 1;
        while (t-- > 0)
        {
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            {
                for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                {
                    Vector3Int curPos = new Vector3Int(x, y, 0);
                    if (visited.Contains(curPos) || _tilemap.GetTile(curPos) == null) continue;

                    List<Vector3Int> clearTiles = CheckAdjacentTiles(visited, curPos);

                    if (clearTiles.Count <= 2) continue;

                    foreach (Vector3Int tilePos in clearTiles)
                    {
                        _tilemap.SetTile(tilePos, null);
                    }
                }
            }

            yield return StartCoroutine(DropTile());
        }

        yield return null;
    }

    private IEnumerator DropTile()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (_tilemap.GetTile(new Vector3Int(x, y, 0)) != null) continue;

                Queue<Vector3Int> aboveTiles = GetAboveTiles(x, y);
                if (aboveTiles.Count == 0) continue;

                StartCoroutine(DropAboveTiles(new Queue<Vector3Int>(aboveTiles), x, y));

                break;
            }
        }

        yield return null;
    }

    private Queue<Vector3Int> GetAboveTiles(int x, int y)
    {
        Queue<Vector3Int> aboveTiles = new Queue<Vector3Int>();

        for (int k = y + 1; k < _bounds.yMax; k++)
        {
            if (_tilemap.GetTile(new Vector3Int(x, k, 0)) == null) continue;

            aboveTiles.Enqueue(new Vector3Int(x, k, 0));
        }

        return aboveTiles;
    }

    private IEnumerator DropAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        while (aboveTiles.Count > 0)
        {
            Vector3Int aboveTilePos = aboveTiles.Dequeue();

            StartCoroutine(MoveTileCoroutine(aboveTilePos, new Vector3Int(x, y++, 0), _tilemap.GetTile(aboveTilePos)));
        }

        yield return null;
    }

    /*private IEnumerator MoveTileCoroutine(Vector3Int fromPos, Vector3Int toPos, TileBase tile)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        Vector3 fromWorldPos = _tilemap.CellToWorld(fromPos);
        Vector3 toWorldPos = _tilemap.CellToWorld(toPos);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 currentPos = Vector3.Lerp(fromWorldPos, toWorldPos, t);
            _tilemap.SetTile(_tilemap.WorldToCell(currentPos), tile);
            yield return null;
            elapsedTime += Time.deltaTime;
            _tilemap.SetTile(_tilemap.WorldToCell(currentPos), null);
        }

        _tilemap.SetTile(toPos, tile);
    }*/
    
    private IEnumerator MoveTileCoroutine(Vector3Int fromPos, Vector3Int toPos, TileBase tile)
    {
        float elapsedTime = 0f;
        float duration = 0.25f;
        Vector3 fromWorldPos = _tilemap.GetCellCenterWorld(fromPos);
        Vector3 toWorldPos = _tilemap.GetCellCenterWorld(toPos);

        GameObject tempTile = null;
        
        foreach (var obj in _objectPool)
        {
            if (obj.activeInHierarchy == false)
            {
                tempTile = obj;
                tempTile.SetActive(true);
                break;
            }
        }
        
        tempTile.GetComponent<SpriteRenderer>().sprite = (tile as Tile)?.sprite;
        tempTile.transform.position = fromWorldPos;

        _tilemap.SetTile(fromPos, null);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);

            tempTile.transform.position = Vector3.Lerp(fromWorldPos, toWorldPos, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _tilemap.SetTile(toPos, tile);
        tempTile.SetActive(false);
    }
}