using HaKien;
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
		if (!_character.IsReady)
			return;
		if (_gameTurnController.GetTurn() == 1)
			return;
		if (_character is IActiveSkill && !GamePlayManager.Instance.DiamondManager.IsDropping())
		{
			_character.Active();
		}
		DataManager.Instance.PlayerCurrentTilesAcquired = 0;
		MessageManager.Instance.SendMessage(new Message(MessageType.OnSkillActive));
		Debug.Log("Click");
	}
}
