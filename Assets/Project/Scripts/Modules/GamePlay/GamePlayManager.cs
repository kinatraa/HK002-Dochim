﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HaKien;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum GameState{
    PlayerTurn,
    OpponentTurn,
    PlayerWin,
    PlayerLose,
    CoinFlip,
    BattleAnimation,
    SkillAnimation
}
public class GamePlayManager : Singleton<GamePlayManager>, IMessageHandle
{
    public bool SkillJustActivated { get; set; }
    public GameState State {  get; set; }

    [SerializeField] private DiamondManager diamondManager;

    public DiamondManager DiamondManager
    {
        get => diamondManager;
        set => diamondManager = value;
    }

    [SerializeField] private PretendDiamondManager pretendDiamondManager;

    public PretendDiamondManager PretendDiamondManager
    {
        get => pretendDiamondManager;
        set => pretendDiamondManager = value;
    }

    [SerializeField] private GameTurnController _gameTurnController;

    public GameTurnController GameTurnController
    {
        get => _gameTurnController;
        set => _gameTurnController = value;
    }

    [SerializeField] private BaseCharacter _playerCharacter;

    public BaseCharacter PlayerCharacter
    {
        get => _playerCharacter;
        set => _playerCharacter = value;
    }

    [SerializeField] private BaseCharacter _opponentCharacter;

    public BaseCharacter OpponentCharacter
    {
        get => _opponentCharacter;
        set => _opponentCharacter = value;
    }
    
    [SerializeField] private InitObjectPool _objectPool;

    public Queue<SpriteRenderer> TilePool
    {
        get => _objectPool.GetTilePool();
    }

    public Queue<Tilemap> TilemapPool
    {
        get => _objectPool.GetTilemapPool();
    }

    public Queue<Transform> EffectPool
    {
        get => _objectPool.GetSpecialEffectPool();
    }

    [SerializeField] private TilesData _tileData;

    public Dictionary<TileBase, TileProperties> TilesData
    {
        get => _tileData.TilesDictionary;
        set => _tileData.TilesDictionary = value;
    }

    [SerializeField] public int _rows = 8;
    [SerializeField] public int _columns = 8;

    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tilemap _licoriceTileMap;
    [SerializeField] private Tilemap _effectTileMap;
    private int _coinFlipOutcome = -1;
    public int CoinFlipOutCome
    {
        get => _coinFlipOutcome;
    }
    public Tilemap Tilemap
    {
        get => _tilemap;
        set => _tilemap = value;
    }

    public float Timer
    {
        get => _gameTurnController.timer;
    }

    [SerializeField] private Tilemap _borderTilemap;

    public Tilemap BorderTilemap
    {
        get => _borderTilemap;
        set => _borderTilemap = value;
    }

    public Tilemap LicoriceTileMap
    {
        get => _licoriceTileMap;
        set => _licoriceTileMap = value;
    }

    public Tilemap EffectTileMap
    {
        get => _effectTileMap;
        set => _effectTileMap = value;
    }

    public BoundsInt _bounds;

    public BoundsInt BoardBounds
    {
        get => _bounds;
        set => _bounds = value;
    }

    private void Awake()
    {
        _bounds.xMin = -_columns / 2;
        _bounds.xMax = _columns / 2;
        _bounds.yMin = -_rows / 2;
        _bounds.yMax = _rows / 2;
		State = GameState.CoinFlip;
	}
    public void SetCoinFlipOutcome(int outcome)
    {
        _coinFlipOutcome = outcome;
        _gameTurnController.InitTurn();
    }

	public bool IsInBound(Vector3Int v)
    {
        return v.x >= _bounds.xMin && v.x < _bounds.xMax && v.y >= _bounds.yMin && v.y < _bounds.yMax;
    }

    public bool SameTileColor(Vector3Int a, Vector3Int b, Tilemap tilemap)
    {
        TileBase aTile = tilemap.GetTile(a);
        TileBase bTile = tilemap.GetTile(b);
        bool res = (TilesData[aTile].Color == TilesData[bTile].Color) ||
                   TilesData[aTile].Color == TileColor.All ||
                   TilesData[bTile].Color == TileColor.All;
	    
        return res;
    }

    public bool SameTileType(Vector3Int a, Vector3Int b)
    {
        return TilesData[_tilemap.GetTile(a)].Type == TilesData[_tilemap.GetTile(b)].Type;
    }

    private void OnDestroy()
    {
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
    public void Handle(Message message)
    {
        throw new NotImplementedException();
    }
}