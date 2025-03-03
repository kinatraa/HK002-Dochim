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
    }

    void OnDisable()
    {
        MessageManager.Instance.RemoveSubcriber(MessageType.OnDataChanged, this);
    }

    public void Handle(Message message)
    {
        switch (message.type)
        {
            case MessageType.OnDataChanged:
                UIGameHUD.UpdateUI();
                break;
        }
    }
}
