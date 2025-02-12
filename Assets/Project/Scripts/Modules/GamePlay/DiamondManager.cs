using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DiamondManager : MonoBehaviour
{
    [SerializeField] private TileBase[] _diamondTiles;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private InitObjectPool _initObjectPool;
    
    [SerializeField] private int _rows = 8;
    [SerializeField] private int _columns = 8;

    private List<GameObject> _objectPool;
    
    private Camera _camera;
    private BoundsInt _bounds;
    
    private Vector3Int[] _directions;
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
        
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int i = -_rows / 2; i < _rows / 2; i++)
        {
            for (int j = -_columns / 2; j < _columns / 2; j++)
            {
                _tilemap.SetTile(new Vector3Int(i, j, 0), _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
            }

            for (int j = _columns / 2; j < _columns + _columns / 2; j++)
            {
                _tilemap.SetTile(new Vector3Int(i, j, 0), _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
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
