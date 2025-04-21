using HaKien;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class BaseCharacter : MonoBehaviour
{
	public string characterName;
	[SerializeField]private int maxHP;
	[SerializeField]private int currentHP;
	[SerializeField] private AnimationClip skillAnimation;
	public string skillActiveQuote;
	public Sprite characterPortrait;
	public Sprite SkillIcon;
	protected bool isReady = false;
	protected bool isActive = false;
	public int activeConditionAmount = 0;
	public int currentConditionAmount = 0;
	protected List<StatusData> activeStatus = new List<StatusData>();
	private Dictionary<StatusType,int> tileDestroyedPreEffect = new Dictionary<StatusType,int>();
	public int activeSkillExistenceTurn = 0;
	[SerializeField] public List<TileBase> conditionTile;
	
	protected virtual void Awake()
	{
		currentHP = maxHP;
	}

	//status
	public bool IsReady {  get { return isReady; } }
	public bool IsActive
	{
		get { return isActive; }
		set { isActive = value; }
	}
	public int ActiveConditionAmount {  get { return activeConditionAmount; } }

	public AnimationClip SkillAnimation {  get { return skillAnimation; } }

	public int CurrentActiveSkillExistenceTurn
	{
		get { return activeSkillExistenceTurn; }
	}

	public abstract void RemoveActiveSkill();

	public event System.Action<List<StatusData>> OnStatusChanged;
	public event System.Action<int> OnHPChangedDuringTurn;
	public void ApplyStatus(StatusData status)
	{
		var existingStatus = activeStatus.Find(e => e.Type == status.Type);
		if (existingStatus != null) 
		{
			existingStatus.AddStack(status.Stack);
		}
		else
		{
			activeStatus.Add(status);
			if (!tileDestroyedPreEffect.ContainsKey(status.Type))
			{
				tileDestroyedPreEffect[status.Type] = 0;
				Debug.Log($"{characterName} started tracking tiles for {status.Type}.");
			}
		}
		OnStatusChanged?.Invoke(activeStatus);
	}

	public void ApplyBloodLoss(BaseCharacter source)
	{
		var bloodLoss = activeStatus.Find(e => e.Type == StatusType.BloodLoss);
		if (bloodLoss != null) 
		{
			int damage = bloodLoss.Stack * bloodLoss.EffectValue;
			TakeDamamage(damage);
			Debug.Log($"{characterName} takes {damage} damage from Bloodloss!");
			Debug.Log(currentHP);
			if (source != null)
			{
				source.Heal(damage);
				Debug.Log($"{source.characterName} recovers {damage} HP from Bloodloss!");
				Debug.Log($"{source.currentHP}");
			}
			while (damage > 0)
			{
				RemoveEffectStack(bloodLoss.Type);
				--damage;
			}
		}
	}

	public void AddTilesDestroyed(Dictionary<TileBase, int> destroyedTiles)
	{
		int tilesCounted = 0;
		foreach (var effect in activeStatus.ToArray())
		{
			if (effect.RequiredTiles == null || effect.RequiredTiles.Count == 0 )
			{
				foreach(var tileCount in destroyedTiles.Values)
				{
					tilesCounted += tileCount;
				}
			}
			else
			{
				foreach(var tile in destroyedTiles)
				{
					if (effect.RequiredTiles.Contains(tile.Key))
					{
						tilesCounted += tile.Value;
					}
				}
			}
			if (tilesCounted > 0)
			{
				Debug.Log(tilesCounted);
				tileDestroyedPreEffect[effect.Type] += tilesCounted;

				int cnt = tileDestroyedPreEffect[effect.Type] / effect.AmountOfTileRequired;
				while(cnt-- > 0)
				{
					RemoveEffectStack(effect.Type);
					tileDestroyedPreEffect[effect.Type] -= effect.AmountOfTileRequired;
				}
/*
				if (tileDestroyedPreEffect[effect.Type] >= effect.AmountOfTileRequired)
				{
					Debug.Log("...");
					RemoveEffectStack(effect.Type);
					tileDestroyedPreEffect[effect.Type] -= tilesCounted;

					
				}*/
 			}
		}
	}
	//public void OnEndOfRound(BaseCharacter source = null)
	//{
	//	foreach (var effect in activeStatus.ToArray()) 
	//	{
	//		if(effect.Type == StatusType.BloodLoss)
	//		{
	//			int damage = effect.Stack * effect.EffectValue;
	//			currentHP -= damage;
	//			currentHP = Mathf.Max(currentHP, 0);
	//			Debug.Log($"{characterName} takes {damage} damage from Bloodloss!");
	//			if (source != null)
	//			{
	//				int healValue = damage;
	//				source.currentHP += healValue;
	//				source.currentHP = Mathf.Min(source.maxHP, source.currentHP);
	//				Debug.Log($"{source.characterName} recovers {damage} HP from Bloodloss!");
	//			}
	//		}
	//		else if(effect.Type == StatusType.Charm)
	//		{
	//			//comingsoon
	//		}
	//	}
	//}
	public void RemoveEffectStack(StatusType status)
	{
		var effect = activeStatus.Find(e => e.Type == status);
		if(effect != null)
		{
			effect.AddStack(-1);
			if (effect.Stack == 0)
			{
				activeStatus.Remove(effect);
				Debug.Log("No longer in status effect");
			}
			OnStatusChanged?.Invoke(activeStatus);
		}
	}
	public abstract void Trigger(List<Vector3Int> triggerPosition, int amount);

	public abstract void Active();
	public void TakeDamamage(int amount)
	{
		currentHP = Mathf.Max(currentHP - amount,0);
	}
	public void TakeDamageDuringTurn(int amount)
	{
		currentHP = Mathf.Max(currentHP - amount, 0);
		OnHPChangedDuringTurn?.Invoke(currentHP);
	}
	public virtual void Heal(int amount)
	{
		currentHP = Mathf.Min(currentHP + amount, maxHP);
	}
	public int GetCurrentHP()
	{
		return currentHP;
	}
}
