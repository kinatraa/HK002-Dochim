using System.Collections;
using System.Collections.Generic;
using HaKien;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : Singleton<DataManager>, IMessageHandle
{
	private void Start()
	{
		if (GamePlayManager.Instance.PlayerCharacter != null)
		{
			Debug.Log("A");
			GamePlayManager.Instance.PlayerCharacter.OnStatusChanged += UpdatePlayerStatus;
		}
		if (GamePlayManager.Instance.OpponentCharacter != null)
		{
			Debug.Log("B");
			GamePlayManager.Instance.OpponentCharacter.OnStatusChanged += UpdateOpponentStatus;
		}
	}

	private void OnDestroy()
	{
		if (GamePlayManager.Instance.PlayerCharacter != null)
		{
			GamePlayManager.Instance.PlayerCharacter.OnStatusChanged -= UpdatePlayerStatus;
		}
		if (GamePlayManager.Instance.OpponentCharacter != null)
		{
			GamePlayManager.Instance.OpponentCharacter.OnStatusChanged -= UpdateOpponentStatus;
		}
	}

    private void UpdatePlayerStatus(List<StatusData> statusList)
    {
        PlayerCurrentActiveStatus = new List<StatusData>(statusList);
        MessageManager.Instance.SendMessage(new Message(MessageType.OnStatusChange));
    }
    private void UpdateOpponentStatus(List<StatusData> statusList)
    {
        OpponentCurrentActiveStatus = new List<StatusData>(statusList);
        MessageManager.Instance.SendMessage(new Message(MessageType.OnStatusChange));
    }
	public PlayerData PlayerData;

    public Sprite PlayerPortrait
    {
        get => PlayerData.portrait;
        set
        {
            PlayerData.portrait = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }
    public Sprite PlayerSkillIcon
    {
        get => PlayerData.skillIcon;
        set
        {
            PlayerData.skillIcon = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }
    public int PlayerSkillRequirementAmount
    {
        get => PlayerData.skillRequirementAmount;
        set
        {
            PlayerData.skillRequirementAmount = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }

    public int PlayerCurrentTilesAcquired
    {
        get => PlayerData.currentTilesAcquired;
        set
        {
            PlayerData.currentTilesAcquired = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
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
    public List<StatusData> PlayerCurrentActiveStatus
    {
        get => PlayerData.currentActiveStatus;
        set
        {
            PlayerData.currentActiveStatus = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnStatusChange));
        }
    }
    
    public OpponentData OpponentData;

    public Sprite OpponentPortrait
    {
        get => OpponentData.portrait;
        set
        {
            OpponentData.portrait = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }
    public Sprite OpponentSkillIcon
    {
        get => OpponentData.skillIcon;
        set
        {
            OpponentData.skillIcon = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }
    public int OpponentSkillRequirementAmount
    {
        get => OpponentData.skillRequirementAmount;
        set
        {
            OpponentData.skillRequirementAmount = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnInitUI));
        }
    }
    public int OpponentCurrentTilesAcquired
    {
        get => OpponentData.currentTilesAcquired;
        set
        {
            OpponentData.currentTilesAcquired = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChanged));
        }
    }
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
    public List<StatusData> OpponentCurrentActiveStatus
    {
        get => OpponentData.currentActiveStatus;
        set
        {
            OpponentData.currentActiveStatus = value;
            MessageManager.Instance.SendMessage(new Message(MessageType.OnStatusChange));
        }
    }
    public void Handle(Message message)
    {
        throw new System.NotImplementedException();
    }
}
