using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameHUD : MonoBehaviour, IUIGameBase
{
    [SerializeField] private TextMeshProUGUI _playerScoreText;
    [SerializeField] private TextMeshProUGUI _opponentScoreText;

    void OnEnable()
    {
        UpdateUI();
    }

    void OnDisable()
    {
        
    }

    public void UpdateUI()
    {
        _playerScoreText.text = $"{DataManager.Instance.PlayerScore}";
        _opponentScoreText.text = $"{DataManager.Instance.OpponentScore}";
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
