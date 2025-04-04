using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFlipUI : MonoBehaviour
{
	public Sprite headsSprite;
	public Sprite tailsSprite;
	public Image coinImage;
	public float flipTime = 1f;
	[SerializeField] private Button headButton;
	[SerializeField] private Button tailButton;
	[SerializeField] private GameObject CoinflipBackground;
	[SerializeField] private GameObject playerChooseBackground;

	private int flippingOutcome;
	private bool isFlipping = false;
	private int playerChoice = -1;
	private void Awake()
	{
		headButton.onClick.AddListener(() => SetPlayerChoice(0));
		tailButton.onClick.AddListener(() => SetPlayerChoice(1));
	}
	public void FlipCoin()
	{
		playerChooseBackground.GetComponent<CanvasGroup>().alpha = 0;

		if (!isFlipping && playerChoice != -1)
		{
			StartCoroutine(FlipAnimation());
		}
	}
	private void SetPlayerChoice(int choice)
	{
		playerChoice = choice;
		FlipCoin();
	}

	IEnumerator FlipAnimation()
	{
		isFlipping = true;
		float elapsedTime = 0f;
		Vector3 originalPos = coinImage.transform.localPosition;
		coinImage.transform.DORotate(new Vector3(0, 1080f, 0), flipTime, RotateMode.FastBeyond360)
			.SetEase(Ease.InOutQuad);


		coinImage.transform.DOLocalMoveY(originalPos.y + 100f, flipTime / 2)
			.SetLoops(2, LoopType.Yoyo)
			.SetEase(Ease.OutBounce);

		while (elapsedTime < flipTime)
		{
			coinImage.sprite = (Random.value > 0.5f) ? headsSprite : tailsSprite;
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		bool isHeads = Random.value > 0.5f;
		coinImage.sprite = isHeads ? headsSprite : tailsSprite;
		if (playerChoice == 0 && isHeads)
		{
			//flippingOutcome = isHeads ? 0 : 1;
			flippingOutcome = 0;
		}
		else if(playerChoice == 1 && !isHeads)
		{
			flippingOutcome = 0;
		}
		else
		{
			flippingOutcome = 1;
		}
		Debug.Log("Flip outcome: " + flippingOutcome);

		yield return new WaitForSeconds(1f);

		GamePlayManager.Instance.SetCoinFlipOutcome(flippingOutcome);
		isFlipping = false;
		CoinflipBackground.SetActive(false);
	}
	public int FlipOutcome()
	{
		return flippingOutcome;
	}
}
