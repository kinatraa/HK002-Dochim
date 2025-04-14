using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutcomePopup : MonoBehaviour
{
	[SerializeField] private GameObject _container;
	[SerializeField] private RectTransform _popup;
	[SerializeField] private TextMeshProUGUI _outCome;
	[SerializeField] private Image img;
	[SerializeField] private Sprite[] outComeImg;
	private void Awake()
	{
		_container.SetActive(false);
		_popup.anchoredPosition = new Vector2(0, -1000);
	}
	public void BattleEnd(bool winner)
	{
		_container.SetActive(true);
		if (winner)
		{
			_outCome.text = "Victory Is Your To Claim";
			img.sprite = outComeImg[0];

		}
		else
		{
			_outCome.text = "Better Luck Next Time";
			img.sprite = outComeImg[1];
		}
		_popup.DOAnchorPosY(0, 1f).SetEase(Ease.OutQuad);
	}
}
