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
	//private int indicator;
	private Tilemap tilemap;
	private int threshHold = 0;
	protected override void Awake()
	{
		base.Awake();
		tilemap = GamePlayManager.Instance.Tilemap;
	}
	private void Start()
	{
		target = GamePlayManager.Instance.OpponentCharacter.GetComponent<BaseCharacter>();
	}
	public override void Active()
	{
		ActiveSkills();
	}

	public void ActiveSkills()
	{
		if (!isReady)
			return;
		if (threshHold >= 10)
		{
			isReady = true;
			TakeDamamage(10);
			GamePlayManager.Instance.GameTurnController.AddExtraAction(1);
		}
	}

	public void PassiveSkills()
	{
		if (threshHold <= 20)
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 1, -1, true, 1, 10, null);
			target.ApplyStatus(bloodLoss);
			Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			// Reset indicator
			activeConditionAmount = 0;
			Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
		else
		{
			StatusData bloodLoss = new StatusData(StatusType.BloodLoss, 2, -1, true, 1, 10, null);
			target.ApplyStatus(bloodLoss);
			Debug.Log($"ImpalerHilda: Applied Bloodloss to {target.characterName}");

			// Reset indicator
			activeConditionAmount = 0;
			Debug.Log("ImpalerHilda: Indicator reset to 0");
		}
	}
	 
	public override void Trigger(List<Vector3Int> triggerPosition, int amount)
	{
		foreach(var position in triggerPosition)
		{
			TileBase tile = tilemap.GetTile(position);
			if(conditionTile.Contains(tile))
			{
				activeConditionAmount++;
			}
			if(activeConditionAmount >= 5)
			{
				PassiveSkills();
			}
		}
	}
	public bool IsReady { get { return isReady; } }
}
