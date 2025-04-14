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
	}
	public void Exit()
	{

	}
}
