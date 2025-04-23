using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ImpalerHilda : BaseCharacter,IPassiveSkill
{
	private BaseCharacter target;			
	
	private Tilemap tilemap;
	private int threshHold = 0;
	protected override void Awake()
	{
		base.Awake();
		tilemap = GamePlayManager.Instance.Tilemap;
		canBeRevived = true;
	}
	private void Start()
	{
		if (GamePlayManager.Instance.PlayerCharacter is ImpalerHilda)
		{
			target = GamePlayManager.Instance.OpponentCharacter.GetComponent<BaseCharacter>();
		}
		else
		{
			target = GamePlayManager.Instance.PlayerCharacter.GetComponent<BaseCharacter>();
		}
	}
	public override void Active()
	{
		PassiveSkills();
	}
	public void PassiveSkills()
	{
		if (threshHold <= 15)
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 1, -1, true, 2, 15, null);
			target.ApplyStatus(bloodLoss);
			//Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			//Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
		else
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 2, -1, true, 2, 20, null);
			target.ApplyStatus(bloodLoss);
			//Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			//Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
		// Reset indicator
		currentConditionAmount = 0;
	}
	 
	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		isActive = false;
		bool conditionMetThisTrigger = false;
		foreach (var position in triggerPosition)
		{
			TileBase tile = tilemap.GetTile(position);
			if(conditionTile.Contains(tile))
			{
				currentConditionAmount++;
			}
			if(currentConditionAmount >= activeConditionAmount)
			{
				Active();
				conditionMetThisTrigger = true;
			}
		}
		if (conditionMetThisTrigger)
			isActive = true;
	}
	public override void Heal(int amount)
	{
		base.Heal(amount);
		threshHold += amount;
		Debug.Log(threshHold);
	}
	public override void RemoveActiveSkill()
	{
		
	}

	public void ModifyExistenceSkillTurn()
	{
		
	}
}
