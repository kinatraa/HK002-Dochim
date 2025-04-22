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
	[SerializeField] private TMP_Dropdown difficultyDropdown;
	//playerpref
	private const string DiffcicultyKey = "GameDifficulty";
	private const string SoundStateKey = "SoundState";
	private const string MusicStateKey = "MusicState";

	private void Awake()
	{
		//SetAlpha(soundOptions[1], 0);
		//soundSelectOptions[1].color = Color.black;
		//SetAlpha(musicOptions[1], 0);
		//musicSelectOptions[1].color = Color.black;
		LoadSetting();
	}
	public void LoadSetting()
	{
		difficultyDropdown.value  = PlayerPrefs.GetInt(DiffcicultyKey, 0);
		difficultyDropdown.RefreshShownValue();
		bool isSoundOn = PlayerPrefs.GetInt(SoundStateKey, 1) == 1;
		UpdateSoundUI(isSoundOn);
		bool isMusicOn = PlayerPrefs.GetInt(MusicStateKey, 1) == 1;
		UpdateMusicUI(isMusicOn);
	}
	public void SaveOption()
	{
		PlayerPrefs.SetInt(DiffcicultyKey, difficultyDropdown.value);
		bool isSoundOn = IsUIOptionOn(soundOptions);
		PlayerPrefs.SetInt(SoundStateKey, isSoundOn ? 1 : 0);
		AudioManager.Instance.SetSFXState(isSoundOn);
		bool isMusicOn = IsUIOptionOn(musicOptions);
		PlayerPrefs.SetInt(MusicStateKey, isMusicOn ? 1 : 0);
		AudioManager.Instance.SetMusicState(isMusicOn);
		PlayerPrefs.Save();
	}
	public void ChangeSoundOption()
	{
		bool currentlyOn = IsUIOptionOn(soundOptions);
		UpdateSoundUI(!currentlyOn);
	}
	private bool IsUIOptionOn(Image[] options)
	{
		return Mathf.Approximately(options[0].color.a, 1f);
	}
	private void SetAlpha(Image img,float alpha)
	{
		Color color = img.color;
		color.a = alpha;
		img.color = color;
	}
	public void ChangeMusicOption()
	{
		bool currentlyOn = IsUIOptionOn(musicOptions);
		UpdateMusicUI(!currentlyOn);
	}
	private void UpdateSoundUI(bool isOn)
	{
		if (isOn)
		{
			SetAlpha(soundOptions[0], 1f);
			SetAlpha(soundOptions[1], 0f);
			soundSelectOptions[0].color = Color.white;
			soundSelectOptions[1].color = Color.black;
		}
		else
		{
			SetAlpha(soundOptions[0], 0f);
			SetAlpha(soundOptions[1], 1f);
			soundSelectOptions[0].color = Color.black;
			soundSelectOptions[1].color = Color.white;
		}
	}
	private void UpdateMusicUI(bool isOn)
	{
		if (isOn)
		{
			SetAlpha(musicOptions[0], 1f);
			SetAlpha(musicOptions[1], 0f);
			musicSelectOptions[0].color = Color.white;
			musicSelectOptions[1].color = Color.black;
		}
		else
		{
			SetAlpha(musicOptions[0], 0f);
			SetAlpha(musicOptions[1], 1f);
			musicSelectOptions[0].color = Color.black;
			musicSelectOptions[1].color = Color.white;
		}
	}
}
