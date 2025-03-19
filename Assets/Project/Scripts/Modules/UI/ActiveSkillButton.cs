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
		image = character.activeSkillIcon;
		if(character is IActiveSkill)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (character.IsActive)
		{
			character.Active();
		}
	}
}
