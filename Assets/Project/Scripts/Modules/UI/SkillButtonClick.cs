using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonClick : MonoBehaviour, IPointerClickHandler
{
	public BaseCharacter _character;
	private GameTurnController _gameTurnController;
	private SpriteRenderer _spriteRenderer;
	public void Init(GameTurnController gameTurnController)
	{
		_gameTurnController = gameTurnController;
		_character = GamePlayManager.Instance.PlayerCharacter;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (_character is IActiveSkill)
		{
			_spriteRenderer.sprite = _character.activeSkillIcon;
		}
		else
		{
			Debug.Log("Passive");
			_spriteRenderer.enabled = false;	
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_character.Active();
		Debug.Log("Click");
	}
}
