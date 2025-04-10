using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ImpalerHilda : BaseCharacter,IActiveSkill,IPassiveSkill
{
	private BaseCharacter target;			
	[SerializeField] private List<TileBase> conditionTile;
	private Tilemap tilemap;
	private int threshHold = 0;
	protected override void Awake()
	{
		base.Awake();
		tilemap = GamePlayManager.Instance.Tilemap;
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
		if (!isReady)
			return;
		ActiveSkills();
	}

	public void ActiveSkills()
	{
			Debug.Log("UseSkill");
			isReady = true;
			TakeDamageDuringTurn(10);
			GamePlayManager.Instance.GameTurnController.AddExtraAction(1);
	}

	public void PassiveSkills()
	{
		if (threshHold <= 20)
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 1, -1, true, 1, 100, null);
			target.ApplyStatus(bloodLoss);
			//Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			// Reset indicator
			currentConditionAmount = 0;
			//Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
		else
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 2, -1, true, 1, 100, null);
			target.ApplyStatus(bloodLoss);
			//Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			// Reset indicator
			currentConditionAmount = 0;
			//Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
	}
	 
	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		isActive = false;
		foreach (var position in triggerPosition)
		{
			TileBase tile = tilemap.GetTile(position);
			if(conditionTile.Contains(tile))
			{
				currentConditionAmount++;
			}
			if(currentConditionAmount >= activeConditionAmount)
			{
				isActive = true;
				PassiveSkills();
			}
		}
	}
	public override void Heal(int amount)
	{
		base.Heal(amount);
		threshHold += amount;
		Debug.Log(threshHold);
		if(threshHold >= 2)
		{
			isReady = true;
		}
	}
	public override void RemoveActiveSkill()
	{
		
	}

	public void ModifyExistenceSkillTurn()
	{
		
	}

	public bool IsReady { get { return isReady; } }
}
