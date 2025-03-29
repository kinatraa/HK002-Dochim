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
		skillInfoContainerRect.localPosition = new Vector3(0, -260, 0);
		characterSkillInfos[index].enabled = true;
		characterSkillInfos[index].text = characterPrefabs[index].skillDescriptions;
		if (index == 0)
		{
			characterSkillInfoContainers[index].enabled = true;
			//characterSkillInfos[index].text = characterPrefabs[index].skillDescriptions;
		}
		else if (index == 1) 
		{
			characterSkillInfoContainers[index].enabled = true;
		}
		else
		{
			characterSkillInfoContainers[index].enabled = true;
		}
	}
	private void OnPointerExit()
	{
		skillInfoContainerRect = initInfoContainerRect;
		//for (int i = 0; i < characterPrefabs.Count; i++) 
		//{
		//	characterSkillInfos[i]
		//}
	}
	private void OnPointerClick(int index)
	{
		_selectedCharacter = characterPrefabs[index];
		PlayerPrefs.SetInt("PlayerSelection", index);
		characterPrefabs.RemoveAt(index);
		var opponentCharacter =characterPrefabs[Random.Range(0, characterPrefabs.Count)];
		CharacterSelectionData.SelectedPlayerCharacter = _selectedCharacter;
		CharacterSelectionData.SelectedOpponentCharacter = opponentCharacter;
		SceneManager.LoadScene("Game");
	}
}
