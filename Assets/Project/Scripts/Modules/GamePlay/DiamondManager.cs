using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HaKien;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DiamondManager : MonoBehaviour
{
    [SerializeField] private TileBase[] _diamondTiles;
    [SerializeField] private TileBase licoriceTile;
    [SerializeField] private Tilemap licoriceTileMap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private InitObjectPool _initObjectPool;

    private GameTurnController _gameTurnController;
    
    private Queue<SpriteRenderer> _objectPool;

    private BoundsInt _bounds;
    private int _rows;
    private int _columns;

    private int _dropping = 0;
    private bool _swapping = false;
    
    void Awake()
    {
        _tilemap = GamePlayManager.Instance.Tilemap;
        /*Debug.Log($"{_bounds.xMin}, {_bounds.yMin}, {_bounds.xMax}, {_bounds.yMax}");*/

        _rows = GamePlayManager.Instance._rows;
        _columns = GamePlayManager.Instance._columns;
        
        _gameTurnController = GamePlayManager.Instance.GameTurnController;
    }
    
    void Start()
    {
        _bounds = GamePlayManager.Instance.BoardBounds;
        _objectPool = new Queue<SpriteRenderer>(_initObjectPool.GetObjectPool());
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int i = _bounds.xMin; i < _bounds.xMax; i++)
        {
            for (int j = _bounds.yMin; j < _bounds.yMax; j++)
            {
                Vector3Int position = new Vector3Int(i, j,0);          
                _tilemap.SetTile(position, _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
				if (Random.value < 0.2f)
				{
					licoriceTileMap.SetTile(position, licoriceTile);
				}
			}
        }

        for (int i = _bounds.xMin; i < _bounds.xMax; i++)
        {
            for (int j = _bounds.yMax; j < _bounds.yMax + _rows; j++)
            {
                _tilemap.SetTile(new Vector3Int(i, j, 0), _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
            }
        }

        StartCoroutine(ClearDiamond(0f));
    }

    private IEnumerator SpawnBoard()
    {
        for (int i = _bounds.xMin; i < _bounds.xMax; i++)
        {
            for (int j = _bounds.yMax; j < _bounds.yMax + _rows; j++)
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
        while (pos.y < _bounds.yMax)
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
        _dropping = 0;
        
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
                if (IsLocked(tilePos))
                {
                    licoriceTileMap.SetTile(tilePos, null);
                }
                else
                    _tilemap.SetTile(tilePos, null);
            }
            
            UpdatePlayersScore(clearTiles.Count);

            yield return StartCoroutine(DropTile());
            
            while (_dropping != 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(SpawnBoard());
        }
        
        _gameTurnController.ChangeTurn();
        
        yield return null;
    }

    private IEnumerator DropTile()
    {
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
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

        for (int k = y + 1; k < _bounds.yMax + _rows; k++)
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
        
        ++_dropping;
        
        float elapsedTime = 0f;
        float duration = 0.25f;
        Vector3 fromWorldPos = _tilemap.GetCellCenterWorld(fromPos);
        Vector3 toWorldPos = _tilemap.GetCellCenterWorld(toPos);

        SpriteRenderer tempTile = null;

        if (_objectPool.Count > 0)
        {
            tempTile = _objectPool.Dequeue();
            tempTile.gameObject.SetActive(true);
        }

        tempTile.sprite = (tile as Tile)?.sprite;
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
        tempTile.gameObject.SetActive(false);
        _objectPool.Enqueue(tempTile);
        --_dropping;
    }

    public IEnumerator SwapTile(Vector3Int a, Vector3Int b)
    {

        if (IsLocked(a) || IsLocked(b)) 
        {
            Debug.Log("Either one is blocked");
            yield break;
        }
        TileBase saveTile = _tilemap.GetTile(a);
        StartCoroutine(MoveTileCoroutine(b, a, _tilemap.GetTile(b)));
        StartCoroutine(MoveTileCoroutine(a, b, saveTile));

        while (_dropping != 0)
        {
            yield return null;
        }
        
        yield return null;
    }

    private void UpdatePlayersScore(int score)
    {
        switch (_gameTurnController.GetTurn())
        {
            case 0:
                DataManager.Instance.PlayerScore += score;
                MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
                break;
            case 1:
                DataManager.Instance.OpponentScore += score;
                MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
                break;
        }
    }
    //Licorice
    public bool IsLocked(Vector3Int position)
    {
        return licoriceTileMap.GetTile(position) == licoriceTile;
    }

    private void UnlockLicorice(Vector3Int position)
    {
        licoriceTileMap.SetTile(position, null);
    }
    public bool IsDropping()
    {
        return _dropping != 0;
    }
}