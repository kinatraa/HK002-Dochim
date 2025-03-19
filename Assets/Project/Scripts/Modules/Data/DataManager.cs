using System.Collections;
using System.Collections.Generic;
using HaKien;
using UnityEngine;

public class DataManager : Singleton<DataManager>, IMessageHandle
{
    public PlayerData PlayerData;

    public int PlayerScore
    {
        get => PlayerData.score;
        set
        {
            PlayerData.score = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }

    public int PlayerHP
    {
        get => PlayerData.health;
        set
        {
            PlayerData.health = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public int PlayerMaxHP
    {
        get => PlayerData.maxHealth;
        set
        {
            PlayerData.maxHealth = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public int PlayerRemainActionPoints
    {
        get => PlayerData.actionPoint;
        set
        {
            PlayerData.actionPoint = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    
    public OpponentData OpponentData;

    public int OpponentScore
    {
        get => OpponentData.score;
        set
        {
            OpponentData.score = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public int OpponentHP
    {
        get => OpponentData.health;
        set
        {
            OpponentData.health = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public int OpponentMaxHP
    {
        get => OpponentData.maxHealth;
        set
        {
            OpponentData.maxHealth = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public int OpponentRemainActionPoints
    {
        get => OpponentData.actionPoint;
        set
        {
            OpponentData.actionPoint= value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
    public void Handle(Message message)
    {
        throw new System.NotImplementedException();
    }
}
