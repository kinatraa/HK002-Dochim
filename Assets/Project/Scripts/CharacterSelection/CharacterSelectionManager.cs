using DG.Tweening;
using HaKien;
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
			pointerExit.callback.AddListener((data) => { OnPointerExit(index); });
			trigger.triggers.Add(pointerExit);
			EventTrigger.Entry pointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
			pointerClick.callback.AddListener((data) => { OnPointerClick(index); });
			trigger.triggers.Add(pointerClick);
		}
	}
	private void OnPointerEnter(int index)
	{
		characterSkillInfoContainers[index].enabled = true;
		characterSkillInfos[index].enabled = true;
		characterSkillInfoContainers[index].DOFade(1f, 2f).SetId(1);
		characterSkillInfos[index].DOFade(1f, 2f).SetId(1);
		characterSkillInfoContainers[index].rectTransform.DOAnchorPosY(-782, 1f).SetEase(Ease.OutQuad);
	}
	private void OnPointerExit(int index)
	{
		characterSkillInfoContainers[index].enabled = false;
		characterSkillInfos[index].enabled = false;
		characterSkillInfoContainers[index].rectTransform.DOAnchorPosY(-246, 1f).SetEase(Ease.OutQuad);
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
		MessageManager.Instance.SendMessage(new Message(MessageType.OnGameStart));
		SceneManager.LoadScene("Game");
	}
}
