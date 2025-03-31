using HaKien;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour,IMessageHandle
{
	public void Handle(Message message)
	{
		
	}
	private void OnEnable()
	{
		MessageManager.Instance.AddSubcriber(MessageType.OnSelectDifficulty, this);
	}
	private void OnDisable()
	{
		MessageManager.Instance.RemoveSubcriber(MessageType.OnSelectDifficulty, this);
	}

	[SerializeField] private Image[] soundOptions;
	[SerializeField] private TextMeshProUGUI[] soundSelectOptions;
	[SerializeField] private Image[] musicOptions;
	[SerializeField] private TextMeshProUGUI[] musicSelectOptions;

	private void Awake()
	{
		soundOptions[1].enabled = false;
		soundSelectOptions[1].color = Color.black;
		musicOptions[1].enabled = false;
		musicSelectOptions[1].color = Color.black;
	}
	public void ChangeSoundOption()
	{
		if (soundOptions[0].enabled)
		{
			soundOptions[1].enabled = true;
			soundOptions[0].enabled = false;
			soundSelectOptions[0].color = Color.black;
			soundSelectOptions[1].color= Color.white;
		}
		else
		{
			soundOptions[1].enabled = false;
			soundOptions[0].enabled = true;
			soundSelectOptions[0].color = Color.white;
			soundSelectOptions[1].color = Color.black;
		}
	}
	public void ChangeMusicOption()
	{
		if (musicOptions[0].enabled) 
		{
			musicOptions[1].enabled = true;
			musicOptions[0].enabled = false;
			musicSelectOptions[0].color= Color.black;
			musicSelectOptions[1].color =Color.white;
		}
		else
		{
			musicOptions[1].enabled = false;
			musicOptions[0].enabled = true;
			musicSelectOptions[0].color =Color.white;
			musicSelectOptions[1].color = Color.black;	
		}
	}
	public void SaveOption()
	{
		//upcoming
	}
}
