using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonClick : MonoBehaviour, IPointerClickHandler
{
	public BaseCharacter _character;
	private GameTurnController _gameTurnController;
	[SerializeField]private Image skillIndicator;
	public void Init(GameTurnController gameTurnController)
	{
		_gameTurnController = gameTurnController;
		_character = GamePlayManager.Instance.PlayerCharacter;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_character.Active();
		DataManager.Instance.PlayerCurrentTilesAcquired = 0;
		Debug.Log("Click");
	}
}
