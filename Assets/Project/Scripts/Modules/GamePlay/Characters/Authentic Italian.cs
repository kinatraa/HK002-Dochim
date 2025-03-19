using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AuthenticItalian : BaseCharacter, IActiveSkill
{
	[SerializeField] private TileBase conditionTile;
	[SerializeField] private TileBase fireTile;
	//private int yellowTileCount = 0;
	private Tilemap tileMap;
	private BoundsInt bounds;
	private Tilemap effectTileMap;
	private List<Vector3Int> fireArea = new List<Vector3Int> ();
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
			if (tile == conditionTile)
			{
				//yellowTileCount++;
				activeConditionAmount++;
			}
		}
		if(activeConditionAmount >= 2)
		{
			isReady = true;
			activeConditionAmount = 0;
			return;
		}
	}

	public void ActiveSkills()
	{
		if (!isReady)
			return;
		Vector3Int randomPos = new Vector3Int(Random.Range(bounds.xMin + 2, bounds.xMax - 2), Random.Range(bounds.yMin + 2, bounds.yMax - 2), 0);
		/*for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				Vector3Int tilePos = new Vector3Int(randomPos.x + x, randomPos.y + y, 0);
				effectTileMap.SetTile(tilePos, fireTile);
			}
		}*/

		Vector3Int tilePos = new Vector3Int(randomPos.x, randomPos.y, 0);
		effectTileMap.SetTile(tilePos, fireTile);
		isReady = false;

	}

	public override void Active()
	{
		ActiveSkills();
	}
}
