using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveSkillButton : MonoBehaviour, IUIGameBase,IPointerClickHandler
{
	public Image image;
	public BaseCharacter character;

	private void Awake()
	{
		character = GetComponent<BaseCharacter>();

	}
	public void Hide()
	{
		
	}

	public void Show()
	{
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		
	}
}
