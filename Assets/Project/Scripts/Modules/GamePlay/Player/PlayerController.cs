﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private Tilemap _tilemap;

	private DiamondClick _diamondClick;

	private Camera _camera;
	[SerializeField] private BaseCharacter _character;

	public bool IsPlayerTurn = false;

	[SerializeField] private TextMeshProUGUI _scoreText;
	[SerializeField] private int _score = 0;
	[SerializeField] private List<GameObject> characterPool;
	private void Awake()
	{
		_tilemap = GamePlayManager.Instance.Tilemap;
		_camera = Camera.main;
		_diamondClick = GetComponent<DiamondClick>();
		int index = PlayerPrefs.GetInt("PlayerSelection");
		_character = characterPool[index].GetComponent<BaseCharacter>();
		//_character = characterPool.First().GetComponent<BaseCharacter>();
		GamePlayManager.Instance.PlayerCharacter = _character;
		//send message for init 
		DataManager.Instance.PlayerCharacter = _character;
		DataManager.Instance.PlayerPortrait = _character.characterPortrait;
		DataManager.Instance.PlayerSkillIcon = _character.SkillIcon;
		DataManager.Instance.PlayerQuote = _character.skillActiveQuote;
		DataManager.Instance.PlayerSkillRequirementAmount = _character.activeConditionAmount;
		DataManager.Instance.PlayerMaxHP = _character.GetCurrentHP();
		DataManager.Instance.PlayerHP = _character.GetCurrentHP();
		DataManager.Instance.PlayerConditionTilesSprite = _character.conditionTile
		 .OfType<Tile>()
		 .Select(tile => tile.sprite)
		 .Where(sprite => sprite != null) 
		 .ToList();
		SkillButtonClick skillButton = FindObjectOfType<SkillButtonClick>();
		if (skillButton != null)
		{
			skillButton.Init(GamePlayManager.Instance.GameTurnController);
		}
	}

	void Update()
	{
		if (_diamondClick.CanClick() && IsPlayerTurn)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
				Vector3Int gridPosition = _tilemap.WorldToCell(worldPoint);

				StartCoroutine(_diamondClick.SelectTile(gridPosition));
			}
		}
	}
}
