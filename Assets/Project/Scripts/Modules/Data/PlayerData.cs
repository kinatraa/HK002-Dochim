using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerData
{
    public BaseCharacter character;
    public Sprite portrait;
    public Sprite skillIcon;
    public int skillRequirementAmount;
    public int currentTilesAcquired;
	public int score = 0;
    public int health;
    public int actionPoint;
    public int maxHealth;
    public List<StatusData> currentActiveStatus;
    public string quote;
    public List<Sprite> conditionTilesSprite;
}
