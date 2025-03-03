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
    
    public void Handle(Message message)
    {
        throw new System.NotImplementedException();
    }
}
