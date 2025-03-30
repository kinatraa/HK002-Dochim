using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class AIController : MonoBehaviour
{
    private DiamondClick _diamondClick;

    [SerializeField] private GameObject _aiObject;
    [SerializeField] private BaseCharacter character;
    private AIBehavior _ai;

    private BoundsInt _bounds;
    
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private int _score = 0;

    void Awake()
    {
        _diamondClick = GetComponent<DiamondClick>();
        _bounds = GamePlayManager.Instance.BoardBounds;
        character = GamePlayManager.Instance.OpponentCharacter;
        Debug.Log(character.name);
		//send message for init 
		DataManager.Instance.OpponentHP = character.GetCurrentHP();
		DataManager.Instance.OpponentMaxHP = character.GetCurrentHP();
		DataManager.Instance.OpponentPortrait = character.characterPortrait;
		DataManager.Instance.OpponentSkillIcon = character.SkillIcon;
		DataManager.Instance.OpponentSkillRequirementAmount = character.activeConditionAmount;
	}

    void OnEnable()
    {
        _ai = _aiObject.GetComponent<AIBehavior>();
        _ai.SetDiamondClick(_diamondClick);
    }

    public void PlayTurn()
    {
        StartCoroutine(_ai.SelectTile());
    }
}
