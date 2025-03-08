using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButcherLoren : BaseCharacter
{
	private int tileCount = 0;
	private int overflow = 0;
	private bool isActive = false;
	public bool IsActive {  get { return isActive; } }

	public override void Trigger(TileBase conditionTile, int amount)
	{
		tileCount += amount;
		if(tileCount >= 10)
		{
			ActiveSkill();
			overflow = (int)(tileCount - 20) / 2;
			tileCount = overflow;
		}
	}
	private void ActiveSkill()
	{
		isActive = true;
		GamePlayManager.Instance.GameTurnController.AddExtraAction(1);
	}
}
