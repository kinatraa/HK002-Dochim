using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButcherLoren : BaseCharacter, IPassiveSkill
{
	private int overflow = 0;
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

	public override void RemoveActiveSkill()
	{
		
	}

	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		currentConditionAmount += amount;
		if(currentConditionAmount >= activeConditionAmount)
		{
			Active();
			overflow = (int)(currentConditionAmount - activeConditionAmount) / 2;
			currentConditionAmount = overflow;
		}
	}
}
