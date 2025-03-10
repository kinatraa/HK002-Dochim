using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButcherLoren : BaseCharacter, PassiveSkill
{
	private int tileCount = 0;
	private int overflow = 0;
	private bool isActive = false;
	public bool IsActive {  get { return isActive; } }

	public override void Active()
	{
		PassiveSkills();
	}

	public void PassiveSkills()
	{
		isActive = true;
		GamePlayManager.Instance.GameTurnController.AddExtraAction(1);
	}

	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		tileCount += amount;
		if(tileCount >= 10)
		{
			Active();
			overflow = (int)(tileCount - 20) / 2;
			tileCount = overflow;
		}
	}
}
