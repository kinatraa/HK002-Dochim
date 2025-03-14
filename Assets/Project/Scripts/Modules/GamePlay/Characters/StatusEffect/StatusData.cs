using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum StatusType
{
	None,
	BloodLoss,
	Charm
};
[System.Serializable]
public class StatusData
{
	public StatusType Type {  get; private set; }
	public int Stack {  get; private set; }
	public int RemainingTurns { get; private set; }
	public bool CanBeDispellActively {  get; private set; }
	public int EffectValue {  get; private set; }
	public int AmountOfTileRequired { get; private set; }
	public List<TileBase> RequiredTiles { get; private set; }

	public StatusData(StatusType type, int stack,int turns, bool canBeDispelled, int effectValue, int amountOfTileRequired,List<TileBase> requiredTiles = null)
	{
		Type = type;
		Stack = stack;
		RemainingTurns = turns;
		CanBeDispellActively = canBeDispelled;
		EffectValue = effectValue;
		AmountOfTileRequired = amountOfTileRequired;
		RequiredTiles = requiredTiles;
	}
	public void AddStack(int amount)
	{
		Stack += amount;
	}
	public void DecreaseTurn()
	{
		if (RemainingTurns > 0) 
		{
			RemainingTurns--;
		}
	}
}
