using HaKien;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonClick : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		MessageManager.Instance.SendMessage(new Message(MessageType.OnSkillHover));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		MessageManager.Instance.SendMessage((new Message(MessageType.EndOfHover)));
	}
}
