using System;
using System.Collections;
using System.Collections.Generic;
using HaKien;
using UnityEngine;

public class UIManager : Singleton<UIManager>, IMessageHandle
{
    public UIGameHUD UIGameHUD;
    
    void OnEnable()
    {
        MessageManager.Instance.AddSubcriber(MessageType.OnDataChanged, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnTimeChanged, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnInitUI, this);
        MessageManager.Instance.AddSubcriber(MessageType.OnStatusChange, this);
    }

    void OnDisable()
    {
        MessageManager.Instance.RemoveSubcriber(MessageType.OnDataChanged, this);
		MessageManager.Instance.RemoveSubcriber(MessageType.OnTimeChanged, this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnInitUI,this);
        MessageManager.Instance.RemoveSubcriber(MessageType.OnStatusChange, this);
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
        }
    }
    public void PlayBattleCollisionAnimation(int playerScore, int opponentScore,int playerHP,int opponentHP)
    {
        UIGameHUD.BattleAnimation(playerScore,opponentScore, playerHP, opponentHP);
    }
}
