using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class PretendDiamondManager : MonoBehaviour
{
	[SerializeField] private TileBase[] _normalTiles;
	[SerializeField] private TileBase[] _columnTiles;
	[SerializeField] private TileBase[] _rowTiles;
	[SerializeField] private TileBase _areaTile;
	
	private BoundsInt _bounds;
	
	private Tilemap _tilemap;
	private Tilemap _licoriceTilemap;
	[SerializeField] private Tilemap _copyTilemap;
	[SerializeField] private Tilemap _copyLicoriceTilemap;
	
	private Dictionary<TileBase, TileProperties> _tilesData = new Dictionary<TileBase, TileProperties>();
	
	private List<Vector3Int> _visited = new List<Vector3Int>();
	private List<Vector3Int> _lockTiles = new List<Vector3Int>();
	private List<Vector3Int> _clearTiles = new List<Vector3Int>();
	private List<(Vector3Int, TileBase)> _specialTiles = new List<(Vector3Int, TileBase)>();
	
	private Vector3Int _firstMousePos, _secondMousePos;
	private int _curId = -1;
	
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
	
	private List<(Vector3Int, Vector3Int)> _possibleMoves = new List<(Vector3Int, Vector3Int)>();
	private List<Dictionary<TileColor, int>> colorCounter = new List<Dictionary<TileColor, int>>();
	public List<Dictionary<TileColor, int>> ColorCounter
	{
		get => colorCounter;
		set => colorCounter = value;
	}
	private List<int> scoreCounter = new List<int>();
	public List<int> ScoreCounter
	{
		get => scoreCounter;
		set => scoreCounter = value;
	}

	private void Start()
	{
		_bounds = GamePlayManager.Instance.BoardBounds;
		_tilemap = GamePlayManager.Instance.Tilemap;
		_licoriceTilemap = GamePlayManager.Instance.LicoriceTileMap;
        
		_tilesData = GamePlayManager.Instance.TilesData;
	}

	public void CalculateAllCasesScore()
    {
	    ResetCounter();
        for (int i = 0; i < _possibleMoves.Count; i++)
        {
            CopyTilemap(_tilemap, _copyTilemap);
            CopyTilemap(_licoriceTilemap, _copyLicoriceTilemap);
            
            _curId = i;
            SwapTiles(_possibleMoves[i].Item1, _possibleMoves[i].Item2);
            PretendClearTile();
        }
        
        /*for (int i = 0; i < scoreCounter.Count; i++)
        {
	        Debug.Log($"{i} : {scoreCounter[i]}");
        }*/
    }

    private void PretendClearTile()
    {
        _firstMousePos = _possibleMoves[_curId].Item2;
        _secondMousePos = _possibleMoves[_curId].Item1;
        
        while (true)
        {
	        _visited.Clear();
	        _lockTiles.Clear();
	        _clearTiles.Clear();
	        _specialTiles.Clear();
	        
            for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            {
                for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                {
                    Vector3Int curPos = new Vector3Int(x, y, 0);
                    if(_copyTilemap.GetTile(curPos) == null || _visited.Contains(curPos)) continue;
                    
	                List<Vector3Int> adjacentTiles = GetAdjacentTiles(curPos);
	                
	                _clearTiles.AddRange(adjacentTiles);
	                CheckForSpawnSpecialTile(adjacentTiles);
                }
            }
            
            if (_clearTiles.Count == 0)
            {
	            break;
            }
            
            int bonus = 0, count = 0;
            CalculateScore(ref count, ref bonus);
            scoreCounter[_curId] += count;
            /*Debug.Log($"{_curId} : {scoreCounter[_curId]}");*/
            
            UnlockTiles();
            
            SetSpecialTile();

            DropTile();

            SpawnBoard();
        }
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
				        _lockTiles.Add(pos);
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
				        /*colorCounter[id][_tilesData[_copyTilemap.GetTile(pos)].Color]++;*/
				        adjacentTiles.Add(pos);
				        adjacentTiles.AddRange(CheckAdjacentTiles(pos, 1 - axis));
			        }
			        else
			        {
				        _lockTiles.Add(pos);
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
		    _specialTiles.Add((setPos, _areaTile));
	    }
	    else
	    {
		    if (poss.All(p => p.y == poss[0].y))
		    {
			    for (int i = 0; i < _rowTiles.Length; i++)
			    {
				    if (_tilesData[_copyTilemap.GetTile(setPos)].Color == _tilesData[_rowTiles[i]].Color)
				    {
					    _specialTiles.Add((setPos, _rowTiles[i]));
					    break;
				    }
			    }
		    }
		    else
		    {
			    for (int i = 0; i < _columnTiles.Length; i++)
			    {
				    if (_tilesData[_copyTilemap.GetTile(setPos)].Color == _tilesData[_columnTiles[i]].Color)
				    {
					    _specialTiles.Add((setPos, _columnTiles[i]));
					    break;
				    }
			    }
		    }
	    }
    }

    private void CalculateScore(ref int count, ref int bonus)
    {
	    foreach (Vector3Int tilePos in _clearTiles)
	    {
		    TileBase tile = _copyTilemap.GetTile(tilePos);
		    if (tile != null)
		    {
			    colorCounter[_curId][_tilesData[tile].Color]++;
		    }
		    /*if (CheckTerrainEffect(tilePos))
		    {
			    bonus += 1;
		    }*/

		    if (_tilesData[tile].Type != TileType.Normal)
		    {
			    count += ActiveSpecialTile(tilePos);
		    }
		    
		    _copyTilemap.SetTile(tilePos, null);
		    ++count;
	    }
    }
    
    private int ActiveSpecialTile(Vector3Int pos)
    {
	    int count = 0;
	    
	    TileBase tile = _copyTilemap.GetTile(pos);
	    
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
				    colorCounter[_curId][_tilesData[_copyTilemap.GetTile(pos)].Color]++;
				    _copyTilemap.SetTile(pos, null);
				    ++count;
			    }
			    else
			    {
				    _lockTiles.Add(pos);
			    }
		    }
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
				    colorCounter[_curId][_tilesData[_copyTilemap.GetTile(pos)].Color]++;
				    _copyTilemap.SetTile(pos, null);
				    ++count;
			    }
			    else
			    {
				    _lockTiles.Add(pos);
			    }
		    }
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
				    colorCounter[_curId][_tilesData[_copyTilemap.GetTile(pos + dir[i])].Color]++;
				    _copyTilemap.SetTile(pos + dir[i], null);
				    ++count;
			    }
			    else
			    {
				    _lockTiles.Add(pos + dir[i]);
			    }
		    }
	    }
	    
	    return count;
    }
    
    private void UnlockTiles()
    {
	    for(int i = 0; i < _lockTiles.Count; i++)
	    {
		    _copyLicoriceTilemap.SetTile(_lockTiles[i], null);
	    }
    }
    
    private void SetSpecialTile()
    {
	    for (int i = 0; i < _specialTiles.Count; i++)
	    {
		    _copyTilemap.SetTile(_specialTiles[i].Item1, _specialTiles[i].Item2);
	    }
    }
    
    private void DropTile()
    {
	    for (int x = _bounds.xMin; x < _bounds.xMax; x++)
	    {
		    for (int y = _bounds.yMin; y < _bounds.yMax; y++)
		    {
			    if (_copyTilemap.GetTile(new Vector3Int(x, y, 0)) != null) continue;

			    Queue<Vector3Int> aboveTiles = GetAboveTiles(x, y);

			    DropAboveTiles(new Queue<Vector3Int>(aboveTiles), x, y);

			    break;
		    }
	    }
    }
    
    private Queue<Vector3Int> GetAboveTiles(int x, int y)
    {
	    Queue<Vector3Int> aboveTiles = new Queue<Vector3Int>();

	    for (int k = y + 1; k < _bounds.yMax + GamePlayManager.Instance._rows; k++)
	    {
		    Vector3Int curPos = new Vector3Int(x, k, 0);
		    if (_copyTilemap.GetTile(curPos) == null) continue;

		    aboveTiles.Enqueue(curPos);
	    }

	    return aboveTiles;
    }
    
    private void DropAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
	    Vector3Int pos = new Vector3Int(x, y, 0);

	    while (aboveTiles.Count > 0)
	    {
		    Vector3Int aboveTilePos = aboveTiles.Dequeue();

		    _copyTilemap.SetTile(pos, _copyTilemap.GetTile(aboveTilePos));
		    _copyTilemap.SetTile(aboveTilePos, null);

		    ++pos.y;
	    }
    }
    
    private void SpawnBoard()
    {
	    for (int i = _bounds.xMin; i < _bounds.xMax; i++)
	    {
		    for (int j = _bounds.yMax; j < _bounds.yMax + GamePlayManager.Instance._rows; j++)
		    {
			    Vector3Int position = new Vector3Int(i, j, 0);
			    if (_copyTilemap.GetTile(position) == null)
			    {
				    _copyTilemap.SetTile(position, _normalTiles[Random.Range(0, _normalTiles.Length)]);
			    }
		    }
	    }
    }

    private void CopyTilemap(Tilemap originalTilemap, Tilemap copyTilemap)
    {
        for (int i = _bounds.xMin; i < _bounds.xMax; i++)
        {
            for (int j = _bounds.yMin; j < _bounds.yMax + GamePlayManager.Instance._rows; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                copyTilemap.SetTile(pos, originalTilemap.GetTile(pos));
            }
        }
    }

    private void ResetCounter()
    {
	    Dictionary<TileColor, int> tmpDict = new Dictionary<TileColor, int>();
	    foreach (TileColor tileColor in Enum.GetValues(typeof(TileColor)))
	    {
		    tmpDict.Add(tileColor, 0);
	    }
	    
	    colorCounter.Clear();
	    scoreCounter.Clear();
	    for (int i = 0; i < _possibleMoves.Count; i++)
	    {
		    colorCounter.Add(tmpDict);
		    scoreCounter.Add(0);
	    }
    }
    
    private bool SameTileColor(Vector3Int a, Vector3Int b)
    {
	    TileBase aTile = _copyTilemap.GetTile(a);
	    TileBase bTile = _copyTilemap.GetTile(b);
	    bool res = (_tilesData[aTile].Color == _tilesData[bTile].Color) ||
	               _tilesData[aTile].Color == TileColor.All ||
	               _tilesData[bTile].Color == TileColor.All;
	    
	    return res;
    }
    
    private bool IsLocked(Vector3Int position)
    {
	    return _copyLicoriceTilemap.GetTile(position) != null;
    }

    private void SwapTiles(Vector3Int a, Vector3Int b)
    {
	    TileBase tile = _copyTilemap.GetTile(a);
	    _copyTilemap.SetTile(a, _copyTilemap.GetTile(b));
	    _copyTilemap.SetTile(b, tile);
    }

    public void SetPossibleMoves(List<(Vector3Int, Vector3Int)> possibleMoves)
    {
	    _possibleMoves = possibleMoves;
    }
}
