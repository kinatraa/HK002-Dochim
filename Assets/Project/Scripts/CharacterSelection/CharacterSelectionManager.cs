using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
	[SerializeField]private List<BaseCharacter> characterPrefabs;
	[SerializeField]private List<Image> characterPortraits;
	[SerializeField] private List<Image> characterSkillInfoContainers;
	[SerializeField] private List<TextMeshProUGUI> characterSkillInfos;
	private BaseCharacter _selectedCharacter;
	[SerializeField]private RectTransform skillInfoContainerRect;
	private RectTransform initInfoContainerRect;

	private void Awake()
	{
		initInfoContainerRect = skillInfoContainerRect;
		for (int i = 0; i < characterPortraits.Count; i++) 
		{
			characterPortraits[i].sprite = characterPrefabs[i].characterPortrait;
			characterSkillInfoContainers[i].enabled = false;
			characterSkillInfos[i].enabled = false;
		}
	}
	private void Start()
	{
		for (int i = 0; i < characterPortraits.Count; i++) 
		{
			int index = i;
			EventTrigger trigger = characterPortraits[i].gameObject.AddComponent<EventTrigger>();
			EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
			pointerEnter.callback.AddListener((data) => { OnPointerEnter(index); });
			trigger.triggers.Add(pointerEnter);
			EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
			pointerExit.callback.AddListener((data) => { OnPointerExit(); });
			trigger.triggers.Add(pointerExit);
			EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
			pointerClick.callback.AddListener((data) => { OnPointerClick(index); });
			trigger.triggers.Add(pointerClick);
		}
	}
	private void OnPointerEnter(int index)
	{
		characterSkillInfos[index].enabled = true;

	}
	private void OnPointerExit()
	{
	
	}
	private void OnPointerClick(int index)
	{
		_selectedCharacter = characterPrefabs[index];
		PlayerPrefs.SetInt("PlayerSelection", index);
		int opponentIndex;
		do
		{
			opponentIndex = Random.Range(0, characterPrefabs.Count);
		} while (opponentIndex == index);
		PlayerPrefs.SetInt("OpponentSelection", opponentIndex);
		SceneManager.LoadScene("Game");
	}
}
