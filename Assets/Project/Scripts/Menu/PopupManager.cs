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
		SetAlpha(soundOptions[1], 0);
		soundSelectOptions[1].color = Color.black;
		SetAlpha(musicOptions[1], 0);
		musicSelectOptions[1].color = Color.black;
	}
	public void ChangeSoundOption()
	{
		Debug.Log("Click");
		if (isOn(soundOptions[0]))
		{
			SetAlpha(soundOptions[1], 255);
			SetAlpha(soundOptions[0], 0);
			soundSelectOptions[0].color = Color.black;
			soundSelectOptions[1].color= Color.white;
		}
		else
		{
			SetAlpha(soundOptions[1], 0);
			SetAlpha(soundOptions[0], 255);
			soundSelectOptions[0].color = Color.white;
			soundSelectOptions[1].color = Color.black;
		}
	}
	private bool isOn(Image img)
	{
		Color color = img.color;
		return color.a == 255;
	}
	private void SetAlpha(Image img,float alpha)
	{
		Color color = img.color;
		color.a = alpha;
		img.color = color;
	}
	public void ChangeMusicOption()
	{
		if (isOn(musicOptions[0])) 
		{
			SetAlpha(musicOptions[1], 255);
			SetAlpha(musicOptions[0], 0);
			musicSelectOptions[0].color= Color.black;
			musicSelectOptions[1].color =Color.white;
		}
		else
		{
			SetAlpha(musicOptions[1], 0);
			SetAlpha(musicOptions[0], 255);
			musicSelectOptions[0].color =Color.white;
			musicSelectOptions[1].color = Color.black;	
		}
	}
	public void SaveOption()
	{
		//upcoming
	}
}
