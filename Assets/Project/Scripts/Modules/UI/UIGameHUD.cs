using DG.Tweening;
using HaKien;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGameHUD : MonoBehaviour, IUIGameBase
{
    [SerializeField] private TextMeshProUGUI _currentPlayerTurnScoreText;
    [SerializeField] private TextMeshProUGUI _currentOpponentTurnScoreText;
    [SerializeField] private Sprite[] _turnState;
	[Header("PlayerConditionTiles")]
	[SerializeField] private Image[] _playerConditionTiles; 
    [Header("Player Status")]
    [SerializeField] CanvasGroup _playerStatusCanvasGroup;
    [SerializeField] Image[] _playersCurrentStatusIconList;
    [SerializeField] TextMeshProUGUI[] _playersCurrentStatusTextList;
	[Header("OpponentConditionTiles")]
	[SerializeField] private Image[] _opponentConditionTiles;
    [Header("Opponent Status")]
    [SerializeField] CanvasGroup _opponentStatusCanvasGroup;
    [SerializeField] Image[] _opponentCurrentStatusIconList;
    [SerializeField] TextMeshProUGUI[] _opponentCurrentStatusTextList;

    [SerializeField] Sprite defaultStatusIcon;

    [Header("AnimationBullShit")]
    private Vector2 _playerPortraitInitPos;
    private Vector2 _opponentPortraitInitPos;
    [SerializeField] float _animationDurationTime;
    [SerializeField] private Animator _skillAnimator;
    private Image _skillImage;
	[SerializeField] float _skillAnimationFrameDuration = 0.08f;

	private BaseCharacter playerCharacter;
    private BaseCharacter opponentCharacter;
    private GameObject currentIcon;
	[SerializeField] private float _fillSpeed;

    [SerializeField] private TextMeshProUGUI _timer;

    [Header("Player")]    
    //init UI 
    [SerializeField] private TextMeshProUGUI _playerHP;
    [SerializeField] private Image _playerHPFill;
    [SerializeField] private TextMeshProUGUI _playerActionRemains;
    [SerializeField] private TextMeshProUGUI _playerPassiveSkillIndicator;
    [SerializeField] private TextMeshProUGUI _playerActiveSkillIndicator;
    [SerializeField] private Image _playerSkillIcon;
    [SerializeField] private Image _playerPortrait;
    [SerializeField] private Image _playerCoolDownIndicator;
    [SerializeField] private Image _playerScoreFill;
    [SerializeField] private Image _playerRemainingActionsBoard;
    [SerializeField] private TextMeshProUGUI _playerSkillPopup;
    [SerializeField] private RectTransform _playerSkillPopupRect;
	[Header("Opponent")]
	//init UI 
	[SerializeField] private TextMeshProUGUI _opponentActiveSkillIndicator;
    [SerializeField] private TextMeshProUGUI _opponentPassiveSkillIndicator;
    [SerializeField] private Image _opponentSkillIcon;
    [SerializeField] private Image _opponentPortrait;

	[SerializeField] private Image _opponentHPFill;
    [SerializeField] private TextMeshProUGUI _opponentHP;
    
    [SerializeField] private TextMeshProUGUI _opponentActionRemains;
    
    [SerializeField] private Image _opponentScoreFill;
    
    [SerializeField] private Image _opponentCoolDownIndicator;
    [SerializeField] private Image _opponentRemainingActionsBoard;
	[SerializeField] private TextMeshProUGUI _opponentSkillPopup;
    [SerializeField] private RectTransform _opponentSkillPopupRect;
	private float _opponentFillAmount;
	private float _playerFillAmount;
    private float _scoreDifferent;
    private float _playerScoreToFloat;
    private float _opponentScoreToFloat;
    private float baseScoreFill = 0.5f;
    private float maxScoreFill = 1f;
    private float differenceRatio;
	private void Awake()
	{
        _playerPortraitInitPos = _playerPortrait.rectTransform.anchoredPosition;
        _opponentPortraitInitPos = _opponentPortrait.rectTransform.anchoredPosition;
		_playerRemainingActionsBoard = _playerActionRemains.GetComponentInParent<Image>();
        _opponentRemainingActionsBoard = _opponentActionRemains.GetComponentInParent<Image>();
        _skillImage = _skillAnimator.GetComponent<Image>();
        _opponentScoreFill.fillAmount = baseScoreFill;
        _playerScoreFill.fillAmount = maxScoreFill;
		//_skillAnimationImage.gameObject.SetActive(false);
		//_skillAnimationImage.color = new Color(1, 1, 1, 0);
        /*_skillAnimationImage.enabled = false;*/
        _skillImage.enabled = false;
		_playerSkillPopup.text = "<font=\"MyFont\">AAAA</font>";
	}

	//animation 
    public void BattleAnimation(int playerScore,int opponentScore,int playerHP,int opponentHP)
    {
		_playerPortrait.rectTransform.anchoredPosition = _playerPortraitInitPos;
		_opponentPortrait.rectTransform.anchoredPosition = _opponentPortraitInitPos;

        Vector2 collisionPoint = (_playerPortraitInitPos + _opponentPortraitInitPos) / 2f;
        Sequence battleSequence = DOTween.Sequence();
        battleSequence.AppendCallback(() =>
        {
            Vector2 playerKnockbackPos = collisionPoint - new Vector2(200f, 0f);
            Vector2 opponentKnockbackPos = collisionPoint + new Vector2(200f, 0f);
            if (playerScore > opponentScore)
			{
				_opponentPortrait.rectTransform.DOAnchorPos(opponentKnockbackPos, _animationDurationTime).SetEase(Ease.OutBack);
			}
			else if (playerScore < opponentScore)
			{
				_playerPortrait.rectTransform.DOAnchorPos(playerKnockbackPos, _animationDurationTime).SetEase(Ease.OutBack);
			}
			else
			{
				_playerPortrait.rectTransform.DOShakeAnchorPos(_animationDurationTime, 2f);
				_opponentPortrait.rectTransform.DOShakeAnchorPos(_animationDurationTime, 2f);
			}
		});
        battleSequence.AppendCallback(() =>
        {
            StartCoroutine(UpdateHP(playerHP, opponentHP));
        });
        battleSequence.AppendInterval(_fillSpeed);
		battleSequence.AppendCallback(() =>
		{
			_playerPortrait.rectTransform.DOAnchorPos(_playerPortraitInitPos, _animationDurationTime).SetEase(Ease.InOutQuad);
			_opponentPortrait.rectTransform.DOAnchorPos(_opponentPortraitInitPos, _animationDurationTime).SetEase(Ease.InOutQuad);
		});
	}
    private IEnumerator UpdateHP(int previousPlayerHP,int previousOpponentHP)
    {
        _playerHP.text = $"{previousPlayerHP}";
        _playerFillAmount = (DataManager.Instance.PlayerMaxHP == 0) ? 1 : ((float)previousPlayerHP/ (float)DataManager.Instance.PlayerMaxHP);
        _playerHPFill.DOFillAmount(_playerFillAmount, _fillSpeed);
		_opponentHP.text = $"{previousOpponentHP}";
		_opponentFillAmount = (DataManager.Instance.OpponentMaxHP == 0) ? 1 : ((float)previousOpponentHP / (float)DataManager.Instance.OpponentMaxHP);
        _opponentHPFill.DOFillAmount(_opponentFillAmount, _fillSpeed);

		yield return new WaitForSeconds(_fillSpeed);

		_playerHP.text = $"{DataManager.Instance.PlayerHP}";
		_playerFillAmount = (DataManager.Instance.PlayerMaxHP == 0) ? 1 : ((float)DataManager.Instance.PlayerHP / (float)DataManager.Instance.PlayerMaxHP);
		_playerHPFill.DOFillAmount(_playerFillAmount, _fillSpeed);

		_opponentHP.text = $"{DataManager.Instance.OpponentHP}";
		_opponentFillAmount = (DataManager.Instance.OpponentMaxHP == 0) ? 1 : ((float)DataManager.Instance.OpponentHP / (float)DataManager.Instance.OpponentMaxHP);
		_opponentHPFill.DOFillAmount(_opponentFillAmount, _fillSpeed);
	}
	/*private IEnumerator PlaySkillAnimation(Sprite[] animationFrames, System.Action onCompleteCallback) 
	{
		_skillAnimationImage.enabled = true;
		_skillAnimationImage.color = new Color(_skillAnimationImage.color.r, _skillAnimationImage.color.g, _skillAnimationImage.color.b, 0f);
		_skillAnimationImage.DOFade(1f, 0.2f);


		for (int i = 0; i < animationFrames.Length; i++)
		{
			_skillAnimationImage.sprite = animationFrames[i];
			yield return new WaitForSeconds(_skillAnimationFrameDuration);
		}

		_skillAnimationImage.DOFade(0f, 0.2f).OnComplete(() => {
			Debug.Log("PlaySkillAnimation Fade Out Complete.");
			_skillAnimationImage.enabled = false; 
			_skillAnimationImage.sprite = null;  
			onCompleteCallback?.Invoke(); 
		});
	}*/
	void OnEnable()
    {
        UpdateUI();
    }

    void OnDisable()
    {
        
    }
    public void UpdateTimer()
    {
        int t = (int)GamePlayManager.Instance.Timer;

		_timer.text = t.ToString();
    }
    public void Init()
    {
        _playerPortrait.sprite = DataManager.Instance.PlayerPortrait;
        _playerSkillIcon.sprite = DataManager.Instance.PlayerSkillIcon;
        _playerActiveSkillIndicator.text = $"/{DataManager.Instance.PlayerSkillRequirementAmount}";
        _playerSkillPopup.text = $"{DataManager.Instance.PlayerQuote}";
        _opponentPortrait.sprite = DataManager.Instance.OpponentPortrait;
        _opponentSkillIcon.sprite = DataManager.Instance.OpponentSkillIcon;
        _opponentActiveSkillIndicator.text = $"/{DataManager.Instance.OpponentSkillRequirementAmount}";
        _playerHP.text = $"{DataManager.Instance.PlayerHP}";
        _opponentHP.text = $"{DataManager.Instance.OpponentHP}";
		_opponentSkillPopup.text = $"{DataManager.Instance.OpponentQuote}";
		_playerCoolDownIndicator.fillAmount = 0;
        _opponentCoolDownIndicator.fillAmount = 0;

		playerCharacter = GamePlayManager.Instance.PlayerCharacter;
		opponentCharacter = GamePlayManager.Instance.OpponentCharacter;

		for (int i = 0; i < DataManager.Instance.PlayerConditionTilesSprite.Count; i++) 
		{
			_playerConditionTiles[i].sprite = DataManager.Instance.PlayerConditionTilesSprite[i];
			_playerConditionTiles[i].enabled = true;
		}
		for(int i = 0;i<DataManager.Instance.OpponentConditionTilesSprite.Count; i++)
		{
			_opponentConditionTiles[i].sprite = DataManager.Instance.OpponentConditionTilesSprite[i];
			_opponentConditionTiles[i].enabled = true;
		}

		if(playerCharacter is AuthenticItalian)
		{
			RectTransform test = _playerPortrait.GetComponent<RectTransform>();
			test.rotation = Quaternion.Euler(180, 0, 180);
		}
		if(opponentCharacter is ImpalerHilda)
		{
			RectTransform test = _opponentPortrait.GetComponent <RectTransform>();
			test.rotation = Quaternion.Euler(180, 0, 180);
		}

	}
    
	public void SkillActiveCutScene()
	{
		Sequence cutsceneSequence = DOTween.Sequence();
		SetupSequence(ref cutsceneSequence);
		
		// Set State
		GamePlayManager.Instance.State = GameState.SkillAnimation;

		cutsceneSequence.Play();
	}

	private void SetupSequence(ref Sequence sequence)
	{
		int currentTurn = GamePlayManager.Instance.GameTurnController.GetTurn();
		RectTransform targetRect = null;
		BaseCharacter currentCharacter = null;
		
		if (currentTurn == 0) // Player
		{
			targetRect = _playerSkillPopupRect;
			currentCharacter = GamePlayManager.Instance.PlayerCharacter;
		}
		else if (currentTurn == 1) // Opponent
		{
			targetRect = _opponentSkillPopupRect;
			currentCharacter = GamePlayManager.Instance.OpponentCharacter;
		}
		
		if (currentTurn == 0) // Player
		{
			targetRect.anchoredPosition = new Vector2(-1400, targetRect.anchoredPosition.y);
			sequence.Append(targetRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutQuad));
			sequence.AppendInterval(1f);
			sequence.Append(targetRect.DOAnchorPosX(1400, 0.5f).SetEase(Ease.OutQuad));
			sequence.Append(targetRect.DOAnchorPosX(-1400, 0f).SetEase(Ease.OutQuad));
		}
		else // Opponent
		{
			targetRect.anchoredPosition = new Vector2(1400, targetRect.anchoredPosition.y);
			sequence.Append(targetRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutQuad));
			sequence.AppendInterval(1f);
			sequence.Append(targetRect.DOAnchorPosX(-1400, 0.5f).SetEase(Ease.OutQuad));
			sequence.Append(targetRect.DOAnchorPosX(1400, 0f).SetEase(Ease.OutQuad));
		}
		
		sequence.OnComplete(() => {
			//Skill animation
			if (currentCharacter.SkillAnimation)
			{
				_skillAnimator.Play(currentCharacter.SkillAnimation.name, 0, 0f);
			}
		});
	}

	public void UpdateScore()
    {
		_playerScoreToFloat = float.Parse(_currentPlayerTurnScoreText.text);
		_opponentScoreToFloat = float.Parse(_currentOpponentTurnScoreText.text);

		_scoreDifferent = Mathf.Abs(_playerScoreToFloat - _opponentScoreToFloat);

		float totalScore = _playerScoreToFloat + _opponentScoreToFloat;
		differenceRatio = (totalScore == 0) ? 0 : (_scoreDifferent / totalScore);

		float maxFillVariation = 0.5f;
		float fillChange = differenceRatio * maxFillVariation;

		float targetFill;

		if (_playerScoreToFloat > _opponentScoreToFloat)
			
		{
			targetFill = baseScoreFill - fillChange;
		}
		else if (_playerScoreToFloat < _opponentScoreToFloat)
		{
			targetFill = baseScoreFill + fillChange;
		}
		else
		{
			targetFill = baseScoreFill;
		}

		targetFill = Mathf.Clamp01(targetFill);

		_opponentScoreFill.DOFillAmount(targetFill, _fillSpeed);
	}
    public void UpdateUI()
    {
        _currentPlayerTurnScoreText.text = $"{DataManager.Instance.PlayerScore}";
        _currentOpponentTurnScoreText.text = $"{DataManager.Instance.OpponentScore}";
        UpdateScore();
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 0) {
            _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
            _playerRemainingActionsBoard.sprite = _turnState[0];
            _playerActionRemains.enabled = true;
            _opponentRemainingActionsBoard.sprite = _turnState[1];
            _opponentActionRemains.enabled = false;
        }
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 1) {
            _opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
			_playerRemainingActionsBoard.sprite = _turnState[1];
			_playerActionRemains.enabled = false;
			_opponentRemainingActionsBoard.sprite = _turnState[0];
			_opponentActionRemains.enabled = true;
		}
        _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
		_opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
        SkillUpdate();
        UpdateStatus();
	}
	public void SkillUpdate()
	{
		_playerActiveSkillIndicator.text = $"{DataManager.Instance.PlayerCurrentTilesAcquired}/{DataManager.Instance.PlayerSkillRequirementAmount}";
		_opponentActiveSkillIndicator.text = $"{DataManager.Instance.OpponentCurrentTilesAcquired}/{DataManager.Instance.OpponentSkillRequirementAmount}";
		float playerProgress = (DataManager.Instance.PlayerSkillRequirementAmount == 0) ? 1 : (float)DataManager.Instance.PlayerCurrentTilesAcquired / DataManager.Instance.PlayerSkillRequirementAmount;
		float opponentProgress = (DataManager.Instance.OpponentSkillRequirementAmount == 0) ? 1 :  (float)DataManager.Instance.OpponentCurrentTilesAcquired / DataManager.Instance.OpponentSkillRequirementAmount;

		_playerCoolDownIndicator.DOFillAmount(playerProgress, _fillSpeed);
        _opponentCoolDownIndicator.DOFillAmount(opponentProgress, _fillSpeed);

	}
    public void UpdateStatus()
    {
		UpdateCharacterEffect(DataManager.Instance.PlayerCurrentActiveStatus,_playerStatusCanvasGroup,_playersCurrentStatusIconList,_playersCurrentStatusTextList);
        UpdateCharacterEffect(DataManager.Instance.OpponentCurrentActiveStatus, _opponentStatusCanvasGroup, _opponentCurrentStatusIconList, _opponentCurrentStatusTextList);
    }
    private void UpdateCharacterEffect(List<StatusData> statusList,CanvasGroup canvasGroup, Image[] statusImages, TextMeshProUGUI[] stackTexts)
    {
		if (statusList == null || statusList.Count == 0)
        {
			canvasGroup.alpha = 0f;
            return;
        }
		canvasGroup.alpha = 1f;
        for (int i = 0; i < statusImages.Length; i++) 
        {
            if (i < statusList.Count)
            {
                statusImages[i].gameObject.SetActive(true);
                statusImages[i].sprite = defaultStatusIcon;
                if (stackTexts[i] != null)
                    stackTexts[i].text = statusList[i].Stack.ToString();
            }
            else
            {
                statusImages[i].gameObject.SetActive(false);
				if (stackTexts[i] != null) stackTexts[i].text = "";
			}
        }
    }

    public void HPChangedBySkill()
    {
        Debug.Log("A");
        _playerHP.text = $"{DataManager.Instance.PlayerHP}";
        _playerFillAmount = (DataManager.Instance.PlayerMaxHP == 0) ? 1 : ((float)DataManager.Instance.PlayerHP / (float)DataManager.Instance.PlayerMaxHP);
        _playerHPFill.DOFillAmount(_playerFillAmount, _fillSpeed);
        _opponentFillAmount = (DataManager.Instance.OpponentMaxHP == 0) ? 1 : ((float)DataManager.Instance.OpponentHP / (float)DataManager.Instance.OpponentMaxHP);
        _opponentHP.text = $"{DataManager.Instance.OpponentHP}";
        _opponentHPFill.DOFillAmount(_opponentFillAmount, _fillSpeed);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
