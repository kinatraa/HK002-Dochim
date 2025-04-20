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
        
        _diamondManager = GamePlayManager.Instance.DiamondManager;
    }

    public IEnumerator SelectTile(Vector3Int selectedPos)
    {
        if (_selected)
        {
            _selected = false;
            _bordermap.SetTile(_selectedTile, null);
            if (GamePlayManager.Instance.IsInBound(selectedPos) && CheckAdjacentVector(_selectedTile, selectedPos))
            {
                yield return StartCoroutine(_diamondManager.SwapTile(_selectedTile, selectedPos));
                if (!Utils.CanSwap(_selectedTile, selectedPos, 0, _tilemap))
                {
                    /*Debug.Log("Can't swap");*/
                    yield return StartCoroutine(_diamondManager.SwapTile(_selectedTile, selectedPos));
                }
                else
                {
					/*Debug.Log("Swap");*/
					GamePlayManager.Instance.SkillJustActivated = false;
					yield return StartCoroutine(_diamondManager.ClearDiamond(selectedPos, _selectedTile));
                    bool skillWasActivated = GamePlayManager.Instance.SkillJustActivated;
					//if (skillWasActivated)
					//{

					//}
					//else
					//{
					//    GamePlayManager.Instance.GameTurnController.UseAction();
					//}
					GamePlayManager.Instance.GameTurnController.UseAction();
				}
                    
                /*StartCoroutine(ClearDiamond());*/
            }
        }
        else
        {
            if(_diamondManager.IsLocked(selectedPos)) yield break;
            if (GamePlayManager.Instance.IsInBound(selectedPos))
            {
                _selected = true;
                _selectedTile = selectedPos;
                _bordermap.SetTile(_selectedTile, _borderTile);
            }
        }
        
        yield return null;
    }

    private bool CheckAdjacentVector(Vector3Int a, Vector3Int b)
    {
        int dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        return dist == 1;
    }

    public bool CanClick()
    {
        GameState currentState = GamePlayManager.Instance.State;
		bool isInTurn = currentState == GameState.PlayerTurn || currentState == GameState.OpponentTurn;
		return isInTurn && !_diamondManager.IsDropping();
    }
    
}