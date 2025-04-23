using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HaKien;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class AIController : MonoBehaviour
{
    private DiamondClick _diamondClick;

    [SerializeField] private GameObject[] _aiOptions;
    [SerializeField] private GameObject _aiObject;
    [SerializeField] private BaseCharacter character;
    private AIBehavior _ai;

    private BoundsInt _bounds;
    
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private int _score = 0;
	[SerializeField] private List<GameObject> characterPool;

	void Awake()
    {
        int difficultyindex = PlayerPrefs.GetInt("GameDifficulty");
        _aiObject = _aiOptions[difficultyindex];
        _diamondClick = GetComponent<DiamondClick>();
        _bounds = GamePlayManager.Instance.BoardBounds;
		int index = PlayerPrefs.GetInt("OpponentSelection");
		character = characterPool[index].GetComponent<BaseCharacter>();
        GamePlayManager.Instance.OpponentCharacter = character;
        //send message for init 
        DataManager.Instance.OpponentCharacter = character;
        Debug.Log(character.GetCurrentHP());
        Debug.Log(character.currentConditionAmount);
        Debug.Log(character.activeConditionAmount);
		DataManager.Instance.OpponentMaxHP = character.GetCurrentHP();
		DataManager.Instance.OpponentHP = character.GetCurrentHP();
		DataManager.Instance.OpponentPortrait = character.characterPortrait;
		DataManager.Instance.OpponentSkillIcon = character.SkillIcon;
		DataManager.Instance.OpponentConditionTilesSprite = character.conditionTile
        .OfType<Tile>()
        .Select(tile => tile.sprite)
        .Where(sprite => sprite != null)
        .ToList();
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
	    CheckForActiveSkill();
        StartCoroutine(_ai.SelectTile());
    }
    
    private void CheckForActiveSkill()
    {
	    if (!character.IsReady || GamePlayManager.Instance.DiamondManager.IsDropping() ||
	        GamePlayManager.Instance.State != GameState.OpponentTurn)
	    {
		    return;
	    }
		    
	    if (character is IActiveSkill)
	    {
		    character.Active();
	    }
	    
	    DataManager.Instance.OpponentCurrentTilesAcquired = 0;
	    MessageManager.Instance.SendMessage(new Message(MessageType.OnSkillActive));
    }
}
