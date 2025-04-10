using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ButcherLoren : BaseCharacter, IPassiveSkill
{

	public override void Active()
	{
		PassiveSkills();
	}

	public void PassiveSkills()
	{
		GamePlayManager.Instance.GameTurnController.AddExtraAction(1);
	}

	public override void RemoveActiveSkill()
	{
		
	}

	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		isActive = false;
		currentConditionAmount += amount;
		if(currentConditionAmount >= activeConditionAmount)
		{
			isActive = true;
			Active();
			currentConditionAmount = 0;
		}
	}
}
