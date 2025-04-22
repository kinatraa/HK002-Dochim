using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HaKien;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DiamondManager : MonoBehaviour
{
    [SerializeField] private TileBase[] _normalTiles;
    [SerializeField] private TileBase[] _columnTiles;
    [SerializeField] private TileBase[] _rowTiles;
    [SerializeField] private TileBase _areaTile;
    
    [SerializeField] private TileBase licoriceTile;
    [SerializeField] private Tilemap licoriceTileMap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _terrainTile;
    
    private GameTurnController _gameTurnController;
    
    private Queue<SpriteRenderer> _tilePool;
    private Queue<Transform> _effectPool;

    private BoundsInt _bounds;
    private int _rows;
    private int _columns;

    private int _count = 0, _bonus = 0;

    private int _dropping = -1;
    private int _effectRunning = 0;
    private bool _swapping = false;
    
    private Vector3Int _firstMousePos, _secondMousePos;

    private List<Vector3Int> clearTiles = new List<Vector3Int>();
    private List<Vector3Int> lockTiles = new List<Vector3Int>();
    private List<(Vector3Int setPos, TileBase)> specialTiles = new List<(Vector3Int setPos, TileBase)>();

    private Dictionary<TileBase, TileProperties> _tilesData;

    private List<Vector3Int> _visited = new List<Vector3Int>();
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

	[SerializeField] private Transform _specialTileEffect;
	List<Vector3Int> _clearEffectTiles = new List<Vector3Int>();
	
	void Awake()
    {
        _tilemap = GamePlayManager.Instance.Tilemap;

        _rows = GamePlayManager.Instance._rows;
        _columns = GamePlayManager.Instance._columns;
        
        _gameTurnController = GamePlayManager.Instance.GameTurnController;
        _tilesData = GamePlayManager.Instance.TilesData;
	}
    
    void Start()
    {
        _bounds = GamePlayManager.Instance.BoardBounds;
        //Debug.Log($"{_bounds.xMin}, {_bounds.yMin}, {_bounds.xMax}, {_bounds.yMax}");
        _tilePool = GamePlayManager.Instance.TilePool;
        _effectPool = GamePlayManager.Instance.EffectPool;
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
					tile = _normalTiles[Random.Range(0, _normalTiles.Length)];
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
                _tilemap.SetTile(new Vector3Int(i, j, 0), _normalTiles[Random.Range(0, _normalTiles.Length)]);
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
                    _tilemap.SetTile(position, _normalTiles[Random.Range(0, _normalTiles.Length)]);
                }
            }
        }

        yield return null;
    }

	private List<Vector3Int> GetAdjacentTiles(Vector3Int curPos)
	{
		List<Vector3Int> adjacentTiles = new List<Vector3Int>();
		
		if (_tilesData[_tilemap.GetTile(curPos)].Type == TileType.Area)
		{
			_visited.Add(curPos);
			adjacentTiles.Add(curPos);
			return adjacentTiles;
		}
		
		adjacentTiles.AddRange(CheckAdjacentTiles(curPos, 0));
		adjacentTiles.AddRange(CheckAdjacentTiles(curPos, 1));
		
		return adjacentTiles;
	}

	//axis: 0 la ngang, 1 la doc
	//chac la DFS
    private List<Vector3Int> CheckAdjacentTiles(Vector3Int curPos, int axis)
    {
	    List<Vector3Int> adjacentTiles = new List<Vector3Int>();
	    
        int cnt; Vector3Int pos;

        //check theo hang ngang
        if (axis == 0)
        {
	        cnt = 1; pos = curPos;
	        --pos.x;
	        while (pos.x >= _bounds.xMin)
	        {
		        if (SameTileColor(pos, curPos)) ++cnt;
		        else break;
		        --pos.x;
	        }

	        pos = curPos;
	        ++pos.x;
	        while (pos.x < _bounds.xMax)
	        {
		        if (SameTileColor(pos, curPos)) ++cnt;
		        else break;
		        ++pos.x;
	        }

	        if (cnt >= 3)
	        {
		        --pos.x;
		        for (int i = 0; i < cnt; i++,  --pos.x)
		        {
			        if(_visited.Contains(pos)) continue;
			        _visited.Add(pos);
	            
			        if (!IsLocked(pos))
			        {
				        adjacentTiles.Add(pos);
				        adjacentTiles.AddRange(CheckAdjacentTiles(pos, 1 - axis));
			        }
			        else
			        {
				        lockTiles.Add(pos);
			        }
		        }
	        }
        }
        //check theo hang doc
        else
        {
	        cnt = 1; pos = curPos;
	        --pos.y;
	        while (pos.y >= _bounds.yMin)
	        {
		        if (SameTileColor(pos, curPos)) ++cnt;
		        else break;
		        --pos.y;
	        }

	        pos = curPos;
	        ++pos.y;
	        while (pos.y < _bounds.yMax)
	        {
		        if (SameTileColor(pos, curPos)) ++cnt;
		        else break;
		        ++pos.y;
	        }

	        if (cnt >= 3)
	        {
		        --pos.y;
		        for (int i = 0; i < cnt; i++, --pos.y)
		        {
			        if(_visited.Contains(pos)) continue;
			        _visited.Add(pos);
	            
			        if (!IsLocked(pos))
			        {
				        adjacentTiles.Add(pos);
				        adjacentTiles.AddRange(CheckAdjacentTiles(pos, 1 - axis));
			        }
			        else
			        {
				        lockTiles.Add(pos);
			        }
		        }
	        }
        }
        
        return adjacentTiles;
    }

    private void CheckForSpawnSpecialTile(List<Vector3Int> poss)
    {
	    if(poss.Count < 4) return;

	    Vector3Int setPos = poss[Random.Range(0, poss.Count)];
	    if (poss.Contains(_firstMousePos) || poss.Contains(_secondMousePos))
	    {
		    if (poss.Contains(_firstMousePos)) setPos = _firstMousePos;
		    else setPos = _secondMousePos;
		    
		    _firstMousePos = _secondMousePos = new Vector3Int(-1000, -1000, 0);
	    }
	    
	    if (poss.Count >= 5)
	    {
		    specialTiles.Add((setPos, _areaTile));
	    }
	    else
	    {
		    if (poss.All(p => p.y == poss[0].y))
		    {
			    for (int i = 0; i < _rowTiles.Length; i++)
			    {
				    if (_tilesData[_tilemap.GetTile(setPos)].Color == _tilesData[_rowTiles[i]].Color)
				    {
					    specialTiles.Add((setPos, _rowTiles[i]));
					    break;
				    }
			    }
		    }
		    else
		    {
			    for (int i = 0; i < _columnTiles.Length; i++)
			    {
				    if (_tilesData[_tilemap.GetTile(setPos)].Color == _tilesData[_columnTiles[i]].Color)
				    {
					    specialTiles.Add((setPos, _columnTiles[i]));
					    break;
				    }
			    }
		    }
	    }
    }

    private void SetSpecialTile()
    {
	    for (int i = 0; i < specialTiles.Count; i++)
	    {
		    _tilemap.SetTile(specialTiles[i].Item1, specialTiles[i].Item2);
	    }
    }

    private int ActiveSpecialTile(Vector3Int pos)
    {
	    List<Vector3Int> clearEffectTile = new List<Vector3Int>();
	    int count = 0;
	    
	    TileBase tile = _tilemap.GetTile(pos);
	    if (_tilesData[tile].Type == TileType.Column)
	    {
		    for (int y = _bounds.yMin; y < _bounds.yMax; y++)
		    {
			    pos.y = y;
			    if(_visited.Contains(pos)) continue;
			    _visited.Add(pos);
			    if (!IsLocked(pos))
			    {
				    if (_tilesData[tile].Type != TileType.Normal) ActiveSpecialTile(pos);
				    /*_tilemap.SetTile(pos, null);*/
				    clearEffectTile.Add(pos);
				    ++count;
			    }
			    else
			    {
				    lockTiles.Add(pos);
			    }
		    }
		    RunSpectialTileEffect(_tilemap.GetCellCenterWorld(pos), 0, clearEffectTile);
	    }
	    else if (_tilesData[tile].Type == TileType.Row)
	    {
		    for (int x = _bounds.xMin; x < _bounds.xMax; x++)
		    {
			    pos.x = x;
			    if(_visited.Contains(pos)) continue;
			    _visited.Add(pos);
			    if (!IsLocked(pos))
			    {
				    if (_tilesData[tile].Type != TileType.Normal) ActiveSpecialTile(pos);
				    /*_tilemap.SetTile(pos, null);*/
				    clearEffectTile.Add(pos);
				    ++count;
			    }
			    else
			    {
				    lockTiles.Add(pos);
			    }
		    }
		    RunSpectialTileEffect(_tilemap.GetCellCenterWorld(pos), 1, clearEffectTile);
	    }
	    else if (_tilesData[tile].Type == TileType.Area)
	    {
		    for (int i = 0; i < dir.Length; i++)
		    {
			    if(!GamePlayManager.Instance.IsInBound(pos + dir[i])) continue;
			    if(_visited.Contains(pos + dir[i])) continue;
			    _visited.Add(pos + dir[i]);
			    if (!IsLocked(pos + dir[i]))
			    {
				    if (_tilesData[tile].Type != TileType.Normal) ActiveSpecialTile(pos + dir[i]);
				    _tilemap.SetTile(pos + dir[i], null);
				    ++count;
			    }
			    else
			    {
				    lockTiles.Add(pos + dir[i]);
			    }
		    }
	    }
	    
	    return count;
    }

    private IEnumerator CalculateScore(Dictionary<TileBase, int> destroyedTiles)
    {
	    _clearEffectTiles.Clear();
	    
	    foreach (Vector3Int tilePos in clearTiles)
	    {
		    TileBase tile = _tilemap.GetTile(tilePos);
		    if (tile != null)
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
			    _bonus += 1;
		    }

		    if (_tilesData[tile].Type != TileType.Normal)
		    {
			    _count += ActiveSpecialTile(tilePos);
		    }
		    
		    _tilemap.SetTile(tilePos, null);
		    ++_count;
	    }
	    
	    while (_effectRunning > 0)
	    {
		    yield return null;
	    }

	    yield return null;
    }

    private void UnlockTiles()
    {
	    for(int i = 0; i < lockTiles.Count; i++)
	    {
		    licoriceTileMap.SetTile(lockTiles[i], null);
	    }
    }

    public IEnumerator ClearDiamond(Vector3Int firstPos, Vector3Int secondPos)
    {
        _dropping = 0;
        _firstMousePos = firstPos;
        _secondMousePos = secondPos;	
        
        while (true)
        {
            clearTiles.Clear();
            lockTiles.Clear();
            specialTiles.Clear();
            _visited.Clear();
            
            for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            {
                for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                {
                    Vector3Int curPos = new Vector3Int(x, y, 0);
                        
                    if (_tilemap.GetTile(curPos) == null || _visited.Contains(curPos)) continue;
			
                    //cung toi uu hon mot ti thi phai
                    List<Vector3Int> adjacentTiles = GetAdjacentTiles(curPos);
                    
					CheckForSpawnSpecialTile(adjacentTiles);
                    clearTiles.AddRange(adjacentTiles);
                }
            }
            
            if (clearTiles.Count == 0)
            {
	            _dropping = -1;
                break;
            }
			BaseCharacter characterToCheck = null;
			int currentTurn = GamePlayManager.Instance.GameTurnController.GetTurn();
			if (currentTurn == 0)
			{
				characterToCheck = GamePlayManager.Instance.PlayerCharacter;
			}
			else if (currentTurn == 1)
			{
				characterToCheck = GamePlayManager.Instance.OpponentCharacter;
			}

			if (characterToCheck != null)
			{
				characterToCheck.Trigger(clearTiles, clearTiles.Count);

				if (characterToCheck.IsActive && GamePlayManager.Instance.State != GameState.SkillAnimation)
				{
					GamePlayManager.Instance.SkillJustActivated = true;
					MessageManager.Instance.SendMessage(new Message(MessageType.OnSkillActive));

					characterToCheck.IsActive = false;
				}
				else if (characterToCheck.IsActive && GamePlayManager.Instance.State == GameState.SkillAnimation)
				{
					characterToCheck.IsActive = false;
				}
			}


			Dictionary<TileBase,int> destroyedTiles = new Dictionary<TileBase, int>();
			_bonus = 0; _count = 0;
            yield return StartCoroutine(CalculateScore(destroyedTiles));
            
            UnlockTiles();
            
            SetSpecialTile();

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

			UpdatePlayersScore(_count, (int)Mathf.Ceil(_bonus / 2.0f));
			
            yield return StartCoroutine(DropTile());
            
            while (_dropping > 0)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

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

        if (_tilePool.Count > 0)
        {
            tempTile = _tilePool.Dequeue();
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
        _tilePool.Enqueue(tempTile);
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

        while (_dropping > 0)
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
                //MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
                break;
            case 1:
                DataManager.Instance.OpponentScore += count;
                //MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
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
        return _dropping != -1;
    }
    
    private bool SameTileColor(Vector3Int a, Vector3Int b)
    {
	    TileBase aTile = _tilemap.GetTile(a);
	    TileBase bTile = _tilemap.GetTile(b);
	    bool res = (_tilesData[aTile].Color == _tilesData[bTile].Color) ||
	               _tilesData[aTile].Color == TileColor.All ||
	               _tilesData[bTile].Color == TileColor.All;
	    
	    return res;
    }

    //axis: 0 - column, 1 - row
    private void RunSpectialTileEffect(Vector3 pos, int axis, List<Vector3Int> clearEffectTiles)
    {
	    if (axis == 0)
	    {
		    Vector3 fromPos = new Vector3(pos.x, _bounds.yMin, 0);
		    Vector3 toPos = new Vector3(pos.x, _bounds.yMax, 0);
		    clearEffectTiles.Sort((a, b) => a.y.CompareTo(b.y));
		    StartCoroutine(MoveEffectCoroutine(fromPos, toPos, axis, clearEffectTiles));
	    }
	    else
	    {
		    Vector3 fromPos = new Vector3(_bounds.xMin, pos.y, 0);
		    Vector3 toPos = new Vector3(_bounds.xMax, pos.y, 0);
		    clearEffectTiles.Sort((a, b) => a.x.CompareTo(b.x));
		    StartCoroutine(MoveEffectCoroutine(fromPos, toPos, axis, clearEffectTiles));
	    }
    }
    
    private IEnumerator MoveEffectCoroutine(Vector3 fromPos, Vector3 toPos, int axis, List<Vector3Int> clearEffectTiles)
    {

	    ++_effectRunning;
        
	    float elapsedTime = 0f;
	    float duration = 1f;

	    Transform tempEffect = null;

	    if (_effectPool.Count > 0)
	    {
		    tempEffect = _effectPool.Dequeue();
		    tempEffect.gameObject.SetActive(true);
	    }
	    /*Transform newEffect = Instantiate(_specialTileEffect, GameObject.Find("---GAMEPLAY").transform);*/
	    if (axis == 0)
	    {
		    tempEffect.rotation = Quaternion.Euler(0, 0, 90);
	    }
	    
	    tempEffect.position = fromPos;

	    while (elapsedTime < duration)
	    {
		    float t = elapsedTime / duration;
		    t = t * t * (3f - 2f * t);

		    tempEffect.position = Vector3.Lerp(fromPos, toPos, t);

		    while (clearEffectTiles.Count > 0)
		    {
			    if (axis == 0)
			    {
				    if (clearEffectTiles[0].y <= tempEffect.position.y)
				    {
					    _tilemap.SetTile(clearEffectTiles[0], null);
					    clearEffectTiles.RemoveAt(0);
				    }
				    else
				    {
					    break;
				    }
			    }
			    else
			    {
				    if (clearEffectTiles[0].x <= tempEffect.position.x)
				    {
					    _tilemap.SetTile(clearEffectTiles[0], null);
					    clearEffectTiles.RemoveAt(0);
				    }
				    else
				    {
					    break;
				    }
			    }
		    }

		    elapsedTime += Time.deltaTime;
		    yield return null;
	    }

	    while (clearEffectTiles.Count > 0)
	    {
		    _tilemap.SetTile(clearEffectTiles[0], null);
		    clearEffectTiles.RemoveAt(0);
	    }
	    
	    tempEffect.gameObject.SetActive(false);
	    _effectPool.Enqueue(tempEffect);
	    --_effectRunning;
    }
}