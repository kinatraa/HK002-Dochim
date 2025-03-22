using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameHUD : MonoBehaviour, IUIGameBase
{
    [SerializeField] private TextMeshProUGUI _currentPlayerTurnScoreText;
    [SerializeField] private TextMeshProUGUI _currentOpponentTurnScoreText;
    [SerializeField] private TextMeshProUGUI _playerHP;
    [SerializeField] private Image _playerHPFill;
    [SerializeField] private Image _opponentHPFill;
    [SerializeField] private float  _fillSpeed;
    [SerializeField] private TextMeshProUGUI _opponentHP;
    [SerializeField] private TextMeshProUGUI _playerActionRemains;
    [SerializeField] private TextMeshProUGUI _opponentActionRemains;
    [SerializeField] private RectTransform _playerScoreFill;
    [SerializeField] private RectTransform _opponentScoreFill;

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
	void OnEnable()
    {
        UpdateUI();
    }

    void OnDisable()
    {
        
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
		_playerHP.text = $"{DataManager.Instance.PlayerHP}";
		_playerFillAmount = (DataManager.Instance.PlayerMaxHP == 0) ? 1 : ((float)DataManager.Instance.PlayerHP / (float)DataManager.Instance.PlayerMaxHP);
		_playerHPFill.DOFillAmount(_playerFillAmount, _fillSpeed);
		_opponentFillAmount = (DataManager.Instance.OpponentMaxHP == 0) ? 1 : ((float)DataManager.Instance.OpponentHP / (float)DataManager.Instance.OpponentMaxHP);
		_opponentHP.text = $"{DataManager.Instance.OpponentHP}";
        _opponentHPFill.DOFillAmount(_opponentFillAmount, _fillSpeed);
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 0) {
            _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
        }
        if (GamePlayManager.Instance.GameTurnController.GetTurn() == 1) {
            _opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
        }
        Debug.Log(DataManager.Instance.PlayerRemainActionPoints);
        _playerActionRemains.text = $"{DataManager.Instance.PlayerRemainActionPoints}";
        Debug.Log(_playerActionRemains.text);
		_opponentActionRemains.text = $"{DataManager.Instance.OpponentRemainActionPoints}";
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
