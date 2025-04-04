using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	[SerializeField] GameObject settingPopup;
	[SerializeField] GameObject popupBackground;
	[SerializeField] GameObject exitPopup;
	public void Start()
	{
		popupBackground.SetActive(false);
		settingPopup.SetActive(false);

	}
	public void Play()
	{
		SceneManager.LoadScene("CharacterSelection");
	}
	public void Stop()
	{
		Application.Quit();
	}
	public void Exit()
	{
		popupBackground?.SetActive(true);
		exitPopup.SetActive(true);
	}
	public void Setting()
	{
		popupBackground.SetActive(true);
		settingPopup.SetActive(true);
	}
	public void CloseSetting()
	{
		popupBackground.SetActive(false);
		settingPopup.SetActive(false);
	}
	public void CloseExit()
	{
		popupBackground!.SetActive(false);
		exitPopup.SetActive(false);
	}
}
