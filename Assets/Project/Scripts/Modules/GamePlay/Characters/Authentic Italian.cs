using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AuthenticItalian : BaseCharacter, IActiveSkill
{
	[SerializeField] private TileBase fireTile;
	private Tilemap tileMap;
	private BoundsInt bounds;
	private Tilemap effectTileMap;
	public Dictionary<Vector3Int, int> fireTileInField = new Dictionary<Vector3Int, int>();
	public List<Vector3Int> fireTilePosition = new List<Vector3Int>();
	protected override void Awake()
	{
		base.Awake();
		tileMap = GamePlayManager.Instance.Tilemap;
		effectTileMap = GamePlayManager.Instance.EffectTileMap;
		bounds = GamePlayManager.Instance.BoardBounds;
	}
	public override void Trigger(List<Vector3Int> listTile, int amount)
	{
		foreach (Vector3Int pos in listTile)
		{
			TileBase tile = tileMap.GetTile(pos);
			if (conditionTile.Contains(tile))
			{
				currentConditionAmount++;
			}
		}
		if(currentConditionAmount >= activeConditionAmount)
		{
			isReady = true;
			currentConditionAmount = activeConditionAmount;
			return;
		}
	}

	public void ActiveSkills()
	{
		if (!isReady)
			return;
		isActive = true;
		Vector3Int tilePos;
		do
		{
			Vector3Int randomPos = new Vector3Int(Random.Range(bounds.xMin + 2, bounds.xMax - 2), Random.Range(bounds.yMin + 2, bounds.yMax - 2), 0);
			tilePos = new Vector3Int(randomPos.x, randomPos.y, 0);
		}
		while (fireTilePosition.Contains(tilePos));
		fireTileInField.Add(tilePos, activeSkillExistenceTurn);
		fireTilePosition.Add(tilePos);
		effectTileMap.SetTile(tilePos,fireTile);
		isReady = false;
		isActive = false;
		currentConditionAmount = 0;
	}

	public override void Active()
	{
		ActiveSkills();
	}

	public override void RemoveActiveSkill()
	{
		ModifyExistenceSkillTurn();
	}

	public void ModifyExistenceSkillTurn()
	{
		List<Vector3Int> key = new List<Vector3Int>();
		foreach (var item in fireTilePosition)
		{
			fireTileInField[item] -= 1;
			if (fireTileInField[item] <= 0)
			{
				key.Add(item);
			}
		}
		foreach(var item in key)
		{
			fireTilePosition.Remove(item);
			fireTileInField.Remove(item);
			effectTileMap.SetTile(item, null);
		}
	}
}
