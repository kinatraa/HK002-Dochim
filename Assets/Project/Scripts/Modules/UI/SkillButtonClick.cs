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
		_character = gameTurnController.GetPlayerCharacter();
		Debug.Log(_character.gameObject.name);
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if (_character is ActiveSkill)
		{
			_spriteRenderer.sprite = _character.activeSkillIcon;
			Debug.Log("Actvie");
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
