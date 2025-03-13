using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HaKien;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DiamondManager : MonoBehaviour
{
    [SerializeField] private TileBase[] _diamondTiles;
    [SerializeField] private TileBase licoriceTile;
    [SerializeField] private Tilemap licoriceTileMap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _terrainTile;
    [SerializeField] private InitObjectPool _initObjectPool;

    private GameTurnController _gameTurnController;
    
    private Queue<SpriteRenderer> _objectPool;

    private BoundsInt _bounds;
    private int _rows;
    private int _columns;

    private int _dropping = 0;
    private bool _swapping = false;

    private Vector3Int[] dir =
		{
			new Vector3Int(-1, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(0, -1, 0),
			new Vector3Int(0, 1, 0),
			new Vector3Int(-1, -1, 0),
			new Vector3Int(1, -1, 0),
			new Vector3Int(1, 1, 0),
			new Vector3Int(-1, 1, 0),
		};

	void Awake()
    {
        _tilemap = GamePlayManager.Instance.Tilemap;

        _rows = GamePlayManager.Instance._rows;
        _columns = GamePlayManager.Instance._columns;
        
        _gameTurnController = GamePlayManager.Instance.GameTurnController;
	}
    
    void Start()
    {
        _bounds = GamePlayManager.Instance.BoardBounds;
		//Debug.Log($"{_bounds.xMin}, {_bounds.yMin}, {_bounds.xMax}, {_bounds.yMax}");
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
				//_tilemap.SetTile(position, _diamondTiles[Random.Range(0, _diamondTiles.Length)]);
				TileBase tile;
				do
				{
					tile = _diamondTiles[Random.Range(0, _diamondTiles.Length)];
				}
				while (CheckMatch(i, j, tile));
				_tilemap.SetTile(new Vector3Int(i, j, 0), tile);

				if (Random.value < 0.1f)
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

        //StartCoroutine(ClearDiamond(0f));
    }
	private bool CheckMatch(int i, int j, TileBase newTile)
	{

		if (i >= _bounds.xMin + 2 &&
			_tilemap.GetTile(new Vector3Int(i - 1, j, 0)) == newTile &&
			_tilemap.GetTile(new Vector3Int(i - 2, j, 0)) == newTile)
			return true;
		if (j >= _bounds.yMin + 2 &&
			_tilemap.GetTile(new Vector3Int(i, j - 1, 0)) == newTile &&
			_tilemap.GetTile(new Vector3Int(i, j - 2, 0)) == newTile)
			return true;
		return false;
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

    //private List<Vector3Int> CheckAdjacentTiles(Vector3Int curPos)
    //{
    //    List<>
    //}


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
            if (clearTiles.Count > 0) 
            {
                  if(GamePlayManager.Instance.GameTurnController.GetTurn() == 0)
				  {
                    var playercharacter = GamePlayManager.Instance.PlayerCharacter;
                    playercharacter.Trigger(clearTiles,clearTiles.Count);
                    
				  }
			}
            Dictionary<TileBase,int> destroyedTiles = new Dictionary<TileBase, int>();
            int bonus = 0, count = 0;
            foreach (Vector3Int tilePos in clearTiles)
            {
                if (IsLocked(tilePos))
                {
                    licoriceTileMap.SetTile(tilePos, null);
                }
                else
                {
                    TileBase tile = _tilemap.GetTile(tilePos);
                    if(tile != null)
                    {
						if (destroyedTiles.ContainsKey(tile))
						{
							destroyedTiles[tile]++;
						}
						else
						{
							destroyedTiles[tile] = 1;
						}
					}
					if (CheckTerrainEffect(tilePos))
					{
						bonus += 1;
					}
					_tilemap.SetTile(tilePos, null);
                    ++count;
				}
            }
			if (_gameTurnController.GetTurn() == 0)
			{
				var playerCharacter = GamePlayManager.Instance.PlayerCharacter;
				if (playerCharacter != null)
				{
					playerCharacter.AddTilesDestroyed(destroyedTiles);
				}
			}
			else
			{
				var opponentCharacter = GamePlayManager.Instance.OpponentCharacter;
				if (opponentCharacter != null)
				{
					opponentCharacter.AddTilesDestroyed(destroyedTiles);
				}
			}

			UpdatePlayersScore(count, (int)Mathf.Ceil(bonus / 2.0f));

            yield return StartCoroutine(DropTile());
            
            while (_dropping != 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(SpawnBoard());
        }
        
        //_gameTurnController.ChangeTurn();
        
        yield return null;
    }

    private bool CheckTerrainEffect(Vector3Int pos)
    {
        if(_terrainTile.GetTile(pos) != null)
        {
            return true;
        }
        for (int i = 0; i < dir.Length; i++)
        {
			if (_terrainTile.GetTile(pos + dir[i]) != null)
			{
				return true;
			}
		}
        return false;
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

    private void UpdatePlayersScore(int count, int bonus)
    {
		if (GamePlayManager.Instance.PlayerCharacter is AuthenticItalian)
        {
			DataManager.Instance.PlayerScore += bonus;
		}
        else if(GamePlayManager.Instance.OpponentCharacter is AuthenticItalian)
        {
			DataManager.Instance.OpponentScore += bonus;
		}
        switch (_gameTurnController.GetTurn())
        {
            case 0:
                DataManager.Instance.PlayerScore += count;
                MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
                break;
            case 1:
                DataManager.Instance.OpponentScore += count;
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