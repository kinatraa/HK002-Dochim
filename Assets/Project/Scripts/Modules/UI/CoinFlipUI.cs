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
	[SerializeField] private GameObject CoinflipBackground;
	private int flippingOutcome;
	private bool isFlipping = false;
	private void Awake()
	{
		FlipCoin();
	}
	public void FlipCoin()
	{
		if (!isFlipping)
		{
			StartCoroutine(FlipAnimation());
		}
	}

	IEnumerator FlipAnimation()
	{
		isFlipping = true;
		float elapsedTime = 0f;

		while (elapsedTime < flipTime)
		{
			coinImage.sprite = (Random.value > 0.5f) ? headsSprite : tailsSprite;
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		bool isHeads = Random.value > 0.5f;
		coinImage.sprite = isHeads ? headsSprite : tailsSprite;

		Debug.Log(isHeads ? "Player 1 go first!" : "Player 2 go first!");
		if (isHeads)
		{
			flippingOutcome = 0;
			Debug.Log(flippingOutcome);
		}
		else
		{
			flippingOutcome = 1;
			Debug.Log(flippingOutcome);
		}
		GamePlayManager.Instance.SetCoinFlipOutcome(flippingOutcome);
		isFlipping = false;
		CoinflipBackground.SetActive(false);
	}
	public int FlipOutcome()
	{
		return flippingOutcome;
	}
}
