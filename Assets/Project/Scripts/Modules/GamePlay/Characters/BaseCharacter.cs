using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseCharacter : MonoBehaviour
{
	public string characterName;
	public int maxHP;
	public int currentHP;
	public Sprite characterSprite;
	public Sprite activeSkillIcon;

	protected List<StatusData> activeStatus = new List<StatusData>();
	private Dictionary<StatusType,int> tileDestroyedPreEffect = new Dictionary<StatusType,int>();
	protected virtual void Start()
	{
		currentHP = maxHP;
	}

	public void Initialize(string name, int health,Sprite sprite)
	{
		characterName = name;
		maxHP = health;
		health = maxHP;
		characterSprite = sprite;
	}
	//status

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
		Debug.Log($"{characterName} applied {status.Type} with {status.Stack} stacks. Tiles required to remove: {status.AmountOfTileRequired}");
	}

	public void ApplyBloodLoss(BaseCharacter source)
	{
		var bloodLoss = activeStatus.Find(e => e.Type == StatusType.BloodLoss);
		if (bloodLoss != null) 
		{
			int damage = bloodLoss.Stack * bloodLoss.EffectValue;
			currentHP -= damage;
			currentHP = Mathf.Max(currentHP, 0);
			Debug.Log($"{characterName} takes {damage} damage from Bloodloss!");
			Debug.Log(currentHP);
			if (source != null)
			{
				source.currentHP += damage;
				source.currentHP = Mathf.Min(source.currentHP, maxHP);
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
				tileDestroyedPreEffect[effect.Type] += tilesCounted;

				int cnt = tileDestroyedPreEffect[effect.Type] / effect.AmountOfTileRequired;
				while(cnt-- > 0)
				{
					Debug.Log("...");
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
			if(effect.Stack >= 1)
			{
				effect.AddStack(-1);
			}
			else
			{
				activeStatus.Remove(effect);
				Debug.Log("No longer in status effect");
			}
		}
	}
	public abstract void Trigger(List<Vector3Int> triggerPosition, int amount);

	public abstract void Active();
	public enum status
	{
		None = 0,
		BloodLost = 1,
		Charm = 2
	}
	
}
