using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	[SerializeField] GameObject settingPopup;
	public void Start()
	{
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
	public void Setting()
	{
		settingPopup.SetActive(true);
	}
	public void CloseSetting()
	{
		settingPopup.SetActive(false);
	}
}
