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
    private IAIBehavior _ai;

    private BoundsInt _bounds;
    
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private int _score = 0;

    void Awake()
    {
        _diamondClick = GetComponent<DiamondClick>();
        _bounds = GamePlayManager.Instance.BoardBounds;
        GamePlayManager.Instance.OpponentCharacter = character;
		//send message for init 
        if(character.characterPortrait != null)
        {
        }
		DataManager.Instance.OpponentPortrait = character.characterPortrait;
		DataManager.Instance.OpponentSkillIcon = character.SkillIcon;
		DataManager.Instance.OpponentSkillRequirementAmount = character.activeConditionAmount;
		DataManager.Instance.OpponentHP = character.GetCurrentHP();
        DataManager.Instance.OpponentMaxHP = character.GetCurrentHP();
    }

    void OnEnable()
    {
        _ai = _aiObject.GetComponent<IAIBehavior>();
        _ai.SetDiamondClick(_diamondClick);
    }

    public void PlayTurn()
    {
        StartCoroutine(_ai.SelectTile());
    }
}
