using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private BoundsInt _bounds;

    private bool _dropping = false;
    
    /*private Vector3Int[] _directions;*/
    void Start()
    {
        _bounds.xMin = -_rows / 2;
        _bounds.xMax = _rows / 2;
        _bounds.yMin = -_columns / 2;
        _bounds.yMax = _columns / 2;
        
        _objectPool = _initObjectPool.GetObjectPool();
        
        /*_directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, -1, 0)
        };*/
        
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

        StartCoroutine(ClearDiamond(0f));
    }

    private IEnumerator SpawnBoard()
    {
        for (int i = -_rows / 2; i < _rows / 2; i++)
        {
            for (int j = _columns / 2; j < _columns + _columns / 2; j++)
            {
                Vector3Int position = new Vector3Int(i, j, 0);
                if (_tilemap.GetTile(position) == null)
                {
                    _tilemap.SetTile(position, _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
                }
            }
        }

        yield return null;
    }
    
    private List<Vector3Int> CheckAdjacentTiles(Vector3Int curPos)
    {
        //BFS
        /*List<Vector3Int> clearTiles = new List<Vector3Int>();
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

        return clearTiles;*/
        
        List<Vector3Int> adjTiles = new List<Vector3Int>();
        
        int cnt = 1;
        Vector3Int pos = curPos;
        --pos.x;
        while (pos.x >= _bounds.xMin)
        {
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(curPos)) ++cnt;
            else break;
            --pos.x;
        }
        pos = curPos;
        ++pos.x;
        while (pos.x < _bounds.xMax)
        {
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(curPos)) ++cnt;
            else break;
            ++pos.x;
        }

        if (cnt >= 3)
        {
            --pos.x;
            for (int i = 0; i < cnt; i++)
            {
                adjTiles.Add(pos);
                --pos.x;
            }
        }

        cnt = 1;
        pos = curPos;
        --pos.y;
        while (pos.y >= _bounds.yMin)
        {
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(curPos)) ++cnt;
            else break;
            --pos.y;
        }
        pos = curPos;
        ++pos.y;
        while (pos.y < _bounds.yMax / 3)
        {
            if (_tilemap.GetTile(pos) == _tilemap.GetTile(curPos)) ++cnt;
            else break;
            ++pos.y;
        }
        if (cnt >= 3)
        {
            --pos.y;
            for (int i = 0; i < cnt; i++)
            {
                adjTiles.Add(pos);
                --pos.y;
            }
        }
        
        adjTiles = new HashSet<Vector3Int>(adjTiles).ToList();
        return adjTiles;
    }
    
    public IEnumerator ClearDiamond(float waitTime)
    {
        _dropping = true;
        
        while (true)
        {
            List<Vector3Int> clearTiles = new List<Vector3Int>();
            for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            {
                for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                {
                    Vector3Int curPos = new Vector3Int(x, y, 0);

                    if (_tilemap.GetTile(curPos) == null) continue;

                    //khong toi uu (nhu c)
                    clearTiles.AddRange(CheckAdjacentTiles(curPos));
                }
            }
            
            clearTiles = new HashSet<Vector3Int>(clearTiles).ToList();
            if (clearTiles.Count == 0)
            {
                break;
            }
            foreach (Vector3Int tilePos in clearTiles)
            {
                _tilemap.SetTile(tilePos, null);
            }

            yield return StartCoroutine(DropTile());
            
            yield return new WaitForSeconds(waitTime);
            
            yield return StartCoroutine(SpawnBoard());
        }

        _dropping = false;
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

                StartCoroutine(DropAboveTiles(new Queue<Vector3Int>(aboveTiles), x, y));
                
                break;
            }
        }

        yield return null;
    }
    
    private Queue<Vector3Int> GetAboveTiles(int x, int y)
    {
        Queue<Vector3Int> aboveTiles = new Queue<Vector3Int>();

        for (int k = y + 1; k < _bounds.yMax + _columns; k++)
        {
            Vector3Int curPos = new Vector3Int(x, k, 0);
            if (_tilemap.GetTile(curPos) == null) continue;

            aboveTiles.Enqueue(curPos);
        }

        return aboveTiles;
    }
    
    private IEnumerator DropAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        
        while (aboveTiles.Count > 0)
        {
            Vector3Int aboveTilePos = aboveTiles.Dequeue();

            StartCoroutine(MoveTileCoroutine(aboveTilePos, pos, _tilemap.GetTile(aboveTilePos)));

            ++pos.y;
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

    public bool IsDropping()
    {
        return _dropping;
    }
}
