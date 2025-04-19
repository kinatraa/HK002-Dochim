using System;
using System.Collections;
using System.Collections.Generic;
using HaKien;
using UnityEngine;

public class UIManager : Singleton<UIManager>, IMessageHandle
{
    public UIGameHUD UIGameHUD;
    public OutcomePopup OutcomePopup;
    public bool _playerWon;
    void OnEnable()
    {
        MessageManager.Instance.AddSubcriber(MessageType.OnDataChanged, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnTimeChanged, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnInitUI, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnStatusChange, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnSkillActive, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnDataChangedDuringTurn, this); 
        MessageManager.Instance.AddSubcriber(MessageType.OnGameWin,this);
        MessageManager.Instance.AddSubcriber(MessageType.OnGameLose,this);
    }

    void OnDisable()
    {
        MessageManager.Instance.RemoveSubcriber(MessageType.OnDataChanged, this);
		MessageManager.Instance.RemoveSubcriber(MessageType.OnTimeChanged, this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnInitUI,this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnStatusChange, this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnSkillActive, this);
		MessageManager.Instance.RemoveSubcriber(MessageType.OnDataChangedDuringTurn, this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnGameWin,this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnGameLose, this);
	}

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case MessageType.OnDataChanged:
                UIGameHUD.UpdateUI();
                break;
            case MessageType.OnTimeChanged:
                UIGameHUD.UpdateTimer();
                break;
            case MessageType.OnInitUI:
                UIGameHUD.Init();
                break;
            case MessageType.OnStatusChange:
                UIGameHUD.UpdateStatus();
                break;
            case MessageType.OnDataChangedDuringTurn:
                UIGameHUD.HPChangedBySkill();
                break;
            case MessageType.OnSkillActive:
				GamePlayManager.Instance.State = GameState.SkillAnimation;
				UIGameHUD.SkillActiveCutScene();
                break;
            case MessageType.OnGameWin:
                GamePlayManager.Instance.State = GameState.PlayerWin;
                _playerWon = true;
                OutcomePopup.BattleEnd(_playerWon);
                break;
            case MessageType.OnGameLose:
                GamePlayManager.Instance.State = GameState.PlayerLose;
				_playerWon = false;
                OutcomePopup.BattleEnd(_playerWon);
                break;
        }
    }
    public void PlayBattleCollisionAnimation(int playerScore, int opponentScore,int playerHP,int opponentHP)
    {
        UIGameHUD.BattleAnimation(playerScore,opponentScore, playerHP, opponentHP);
    }

}
