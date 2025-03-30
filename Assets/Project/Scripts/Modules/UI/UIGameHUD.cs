using DG.Tweening;
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

    [Header("Player Status")]
    [SerializeField] CanvasGroup _playerStatusCanvasGroup;
    [SerializeField] Image[] _playersCurrentStatusIconList;
    [SerializeField] TextMeshProUGUI[] _playersCurrentStatusTextList;

    [Header("Opponent Status")]
    [SerializeField] CanvasGroup _opponentStatusCanvasGroup;
    [SerializeField] Image[] _opponentCurrentStatusIconList;
    [SerializeField] TextMeshProUGUI[] _opponentCurrentStatusTextList;

    [SerializeField] Sprite defaultStatusIcon;

    [Header("AnimationBullShit")]
    private Vector2 _playerPortraitInitPos;
    private Vector2 _opponentPortraitInitPos;
    [SerializeField] float _animationDurationTime;
    [SerializeField] RectTransform _collisionPosition;

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
    [SerializeField] private RectTransform _playerScoreFill;
    [SerializeField] private RectTransform _playerSkillDesContainer;
	[SerializeField] private TextMeshProUGUI _playerSkillDescription;
	[Header("Opponent")]
	//init UI 
	[SerializeField] private TextMeshProUGUI _opponentActiveSkillIndicator;
    [SerializeField] private TextMeshProUGUI _opponentPassiveSkillIndicator;
    [SerializeField] private Image _opponentSkillIcon;
    [SerializeField] private Image _opponentPortrait;

	[SerializeField] private Image _opponentHPFill;
    [SerializeField] private TextMeshProUGUI _opponentHP;
    
    [SerializeField] private TextMeshProUGUI _opponentActionRemains;
    
    [SerializeField] private RectTransform _opponentScoreFill;
    
    [SerializeField] private Image _opponentCoolDownIndicator;
    [SerializeField] private RectTransform _opponentSkillDesContainer;
	[SerializeField] private TextMeshProUGUI _opponentSkillDescription;

	private float _opponentFillAmount;
	private float _playerFillAmount;
    private float _scoreDifferent;
    private float _playerScoreToFloat;
    private float _opponentScoreToFloat;
    private Image _playerRemainingActionsBoard;
    private Image _opponentRemainingActionsBoard;
    private float maxWidth = 1510;
    private float baseWidth = 755;

    private float differenceRatio;
	private void Awake()
	{
        _playerPortraitInitPos = _playerPortrait.rectTransform.anchoredPosition;
        _opponentPortraitInitPos = _opponentPortrait.rectTransform.anchoredPosition;
		_playerRemainingActionsBoard = _playerActionRemains.GetComponentInParent<Image>();
        _opponentRemainingActionsBoard = _opponentActionRemains.GetComponentInParent<Image>();
	}

    //animation 
    public void BattleAnimation(int playerScore,int opponentScore,int playerHP,int opponentHP)
    {
		_playerPortrait.rectTransform.anchoredPosition = _playerPortraitInitPos;
		_opponentPortrait.rectTransform.anchoredPosition = _opponentPortraitInitPos;

        Vector2 collisionPoint = (_playerPortraitInitPos + _opponentPortraitInitPos) / 2f;
        Sequence battleSequence = DOTween.Sequence();
        battleSequence.Append(_playerPortrait.rectTransform.DOAnchorPos(collisionPoint, _animationDurationTime).SetEase(Ease.InOutQuad));
        battleSequence.Join(_opponentPortrait.rectTransform.DOAnchorPos(collisionPoint, _animationDurationTime).SetEase(Ease.InOutQuad));
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
				_playerPortrait.rectTransform.DOShakeAnchorPos(_animationDurationTime, 10f);
				_opponentPortrait.rectTransform.DOShakeAnchorPos(_animationDurationTime, 10f);
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
        _opponentPortrait.sprite = DataManager.Instance.OpponentPortrait;
        _opponentSkillIcon.sprite = DataManager.Instance.OpponentSkillIcon;
        _opponentActiveSkillIndicator.text = $"/{DataManager.Instance.OpponentSkillRequirementAmount}";
        _playerHP.text = $"{DataManager.Instance.PlayerHP}";
        _opponentHP.text = $"{DataManager.Instance.OpponentHP}";
		Debug.Log(_opponentHP.text);
		_playerCoolDownIndicator.fillAmount = 0;
        _opponentCoolDownIndicator.fillAmount = 0;

		playerCharacter = GamePlayManager.Instance.PlayerCharacter;
		opponentCharacter = GamePlayManager.Instance.OpponentCharacter;
		_playerSkillDescription.text = playerCharacter.skillDescriptions;

	}
    public void UpdateUI()
    {

        _currentPlayerTurnScoreText.text = $"{DataManager.Instance.PlayerScore}";
        _currentOpponentTurnScoreText.text = $"{DataManager.Instance.OpponentScore}";
        _playerScoreToFloat = float.Parse(_currentPlayerTurnScoreText.text);
        _opponentScoreToFloat = float.Parse(_currentOpponentTurnScoreText.text);
        _scoreDifferent = Mathf.Abs(_playerScoreToFloat - _opponentScoreToFloat);
        differenceRatio = (_scoreDifferent / (_playerScoreToFloat + _opponentScoreToFloat)) * maxWidth;
        if (_playerScoreToFloat > _opponentScoreToFloat)
        {
            float newPlayerWidth = baseWidth + differenceRatio;
            float newOpponentWidth = baseWidth - differenceRatio;

            newPlayerWidth = Mathf.Clamp(newPlayerWidth, 0, maxWidth);
            newOpponentWidth = Mathf.Clamp(newOpponentWidth, 0, maxWidth);

            _playerScoreFill.DOSizeDelta(new Vector2(newPlayerWidth, _playerScoreFill.sizeDelta.y), _fillSpeed);
            _opponentScoreFill.DOSizeDelta(new Vector2(newOpponentWidth, _opponentScoreFill.sizeDelta.y), _fillSpeed);
        }
        else if (_playerScoreToFloat < _opponentScoreToFloat) 
        {
            float newOpponentWidth = baseWidth + differenceRatio;
            float newPlayerWidth = baseWidth - differenceRatio;

			newPlayerWidth = Mathf.Clamp(newPlayerWidth, 0, maxWidth);
			newOpponentWidth = Mathf.Clamp(newOpponentWidth, 0, maxWidth);

			_playerScoreFill.DOSizeDelta(new Vector2(newPlayerWidth, _playerScoreFill.sizeDelta.y), _fillSpeed);
			_opponentScoreFill.DOSizeDelta(new Vector2(newOpponentWidth, _opponentScoreFill.sizeDelta.y), _fillSpeed);
		}
		else
		{
			_playerScoreFill.DOSizeDelta(new Vector2(baseWidth, _playerScoreFill.sizeDelta.y), _fillSpeed);
			_opponentScoreFill.DOSizeDelta(new Vector2(baseWidth, _opponentScoreFill.sizeDelta.y), _fillSpeed);
		}
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 0) {
            _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
            _playerRemainingActionsBoard.enabled = true;
            _playerActionRemains.enabled = true;
            _opponentRemainingActionsBoard.enabled = false;
            _opponentActionRemains.enabled = false;
        }
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 1) {
            _opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
			_playerRemainingActionsBoard.enabled = false;
			_playerActionRemains.enabled = false;
			_opponentRemainingActionsBoard.enabled = true;
			_opponentActionRemains.enabled = true;
		}
        _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
		_opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
        SkillUpdate();
        UpdateStatus();
	}
    public void SkillHover()
    {
        Debug.Log("B");
        _playerSkillDesContainer.gameObject.SetActive(true);
    }
    public void EndOfSkillHover()
    {
        Debug.Log("A");
        _playerSkillDesContainer.gameObject.SetActive(false);
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
