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
		flippingOutcome = isHeads ? 0 : 1;

		Debug.Log(isHeads ? "Player 1 go first!" : "Player 2 go first!");
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
