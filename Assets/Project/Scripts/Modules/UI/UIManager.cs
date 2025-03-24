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
    }

    void OnDisable()
    {
        MessageManager.Instance.RemoveSubcriber(MessageType.OnDataChanged, this);
		MessageManager.Instance.RemoveSubcriber(MessageType.OnTimeChanged, this);
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
        }
    }
}
