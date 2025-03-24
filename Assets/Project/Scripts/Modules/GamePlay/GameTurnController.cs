using HaKien;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnController : MonoBehaviour
{
    [SerializeField] private PlayerController _player1;
    [SerializeField] private AIController _player2;
    public int maxActionPerTurn = 3;
    private int remainingActions;
    private int _turn = 0;
    private BaseCharacter playerCharacter;
    private BaseCharacter opponentCharacter;
    private int turnCounter = 0;
    public float timer = 16;

	private void Update()
	{
		if (GamePlayManager.Instance.DiamondManager.IsDropping()) return;

        timer -= Time.deltaTime;
		
		if (timer < 0)
        {
            UseAction();
        }

		MessageManager.Instance.SendMessage(new Message(MessageType.OnTimeChanged));
	}
	void Start()
    {
        playerCharacter = GamePlayManager.Instance.PlayerCharacter;
        opponentCharacter = GamePlayManager.Instance.OpponentCharacter;
        _turn = -1;
        remainingActions = maxActionPerTurn;
		DataManager.Instance.PlayerRemainActionPoints = remainingActions;
        DataManager.Instance.OpponentRemainActionPoints = remainingActions;
		ChangeTurn();
	}

    public void PlayTurn()
    {
        if (_turn == 0)
        {
            Debug.Log("Player turn");
            _player1.IsPlayerTurn = true;
        }
        else
        {
            Debug.Log("AI turn");
            _player1.IsPlayerTurn = false;
            _player2.PlayTurn();
        }
    }
    public void UseAction()
    {
        remainingActions--;
        if(_turn == 0)
        {
            DataManager.Instance.PlayerRemainActionPoints = remainingActions;
            DataManager.Instance.PlayerCurrentTilesAcquired = playerCharacter.currentConditionAmount;
        }
        else
        {
			DataManager.Instance.OpponentRemainActionPoints = remainingActions;
            DataManager.Instance.OpponentCurrentTilesAcquired = opponentCharacter.currentConditionAmount;
		}
        Debug.Log("Actions left: " + remainingActions);
        if (remainingActions == 0)
        {
            turnCounter++;
            if (turnCounter == 2)
            {
                CaculateDamage();
                DataManager.Instance.PlayerScore = 0;
                DataManager.Instance.OpponentScore = 0;
                DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
                DataManager.Instance.OpponentHP = opponentCharacter.GetCurrentHP();
				turnCounter = 0;
            }
            EndOfRound();
            ChangeTurn();
        }
        else
        {
            PlayTurn();
        }

		timer = 16f;
	}
    public void CaculateDamage()
    {
		int damage = Mathf.Abs(DataManager.Instance.PlayerScore - DataManager.Instance.OpponentScore);
		if (DataManager.Instance.PlayerScore > DataManager.Instance.OpponentScore)
        {
            opponentCharacter.TakeDamamage(damage);
			Debug.Log(opponentCharacter.GetCurrentHP());
		}
        else
        {
            playerCharacter.TakeDamamage(damage);
			Debug.Log(playerCharacter.GetCurrentHP());
		}
    }

	public void AddExtraAction(int extra)
	{
		remainingActions += extra;
		Debug.Log("gain " + extra + " action");
	}
	public void ChangeTurn()
    {
        int damage = 0;
		if (_turn == -1)
        {
            _turn = 1;
        }
		_turn = 1 - _turn;

        remainingActions = maxActionPerTurn;
		if (_turn == 0)
		{
            playerCharacter.RemoveActiveSkill();
			//ModifyExistenceSkillTurn(playerCharacter);
			opponentCharacter.ApplyBloodLoss(playerCharacter);
            DataManager.Instance.PlayerRemainActionPoints = remainingActions;
            DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
        }
		else if (_turn == 1)
		{
			opponentCharacter.RemoveActiveSkill();
			//ModifyExistenceSkillTurn(opponentCharacter);
			playerCharacter.ApplyBloodLoss(opponentCharacter);
			DataManager.Instance.OpponentRemainActionPoints = remainingActions;
		}

		PlayTurn();
    }

    //private void ModifyExistenceSkillTurn(BaseCharacter currentCharacter)
    //{
    //    IActiveSkill activeSkill = currentCharacter.GetComponent<IActiveSkill>();
    //    if (activeSkill != null)
    //    {

    //    }
    //}


	private void EndOfRound()
    {
    }
    public int GetTurn()
    {
        return _turn;
    }

    public void SetTurn(int turn)
    {
        _turn = turn;
    }
}
