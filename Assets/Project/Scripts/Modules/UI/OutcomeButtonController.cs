using HaKien;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OutcomeButtonController : MonoBehaviour
{
	public void Retry()
	{
		SceneManager.LoadScene("CharacterSelection");
	}
	public void Home()
	{
		SceneManager.LoadScene("Menu");
		MessageManager.Instance.SendMessage(new Message(MessageType.OnGameStart));
	}
	public void Exit()
	{

	}
}
