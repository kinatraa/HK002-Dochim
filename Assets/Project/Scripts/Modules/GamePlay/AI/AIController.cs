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
	[SerializeField] private List<GameObject> characterPool;

	void Awake()
    {
        _diamondClick = GetComponent<DiamondClick>();
        _bounds = GamePlayManager.Instance.BoardBounds;
		int index = PlayerPrefs.GetInt("OpponentSelection");
		character = characterPool[index].GetComponent<BaseCharacter>();
        GamePlayManager.Instance.OpponentCharacter = character;
        //send message for init 
        Debug.Log(character.GetCurrentHP());
        Debug.Log(character.currentConditionAmount);
        Debug.Log(character.activeConditionAmount);
		DataManager.Instance.OpponentMaxHP = character.GetCurrentHP();
		DataManager.Instance.OpponentHP = character.GetCurrentHP();
		DataManager.Instance.OpponentPortrait = character.characterPortrait;
		DataManager.Instance.OpponentSkillIcon = character.SkillIcon;
        DataManager.Instance.OpponentQuote = character.skillActiveQuote;
		DataManager.Instance.OpponentSkillRequirementAmount = character.activeConditionAmount;
		DataManager.Instance.OpponentCurrentTilesAcquired = 0;
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
