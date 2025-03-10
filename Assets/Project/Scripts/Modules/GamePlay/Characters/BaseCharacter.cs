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
	private void Start()
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
	public abstract void Trigger(List<Vector3Int> triggerPosition, int amount);

	public abstract void Active();
	public enum status
	{
		None = 0,
		BloodLost = 1,
		Charm = 2
	}
	
}
