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
    private bool isInAnimation = false;
    private void Update()
    {
		if ((GamePlayManager.Instance.State == GameState.PlayerTurn ||
		     GamePlayManager.Instance.State == GameState.OpponentTurn) &&
		    !GamePlayManager.Instance.DiamondManager.IsDropping())
		{
			timer -= Time.deltaTime;

			if (timer < 0)
			{
				UseAction();
			}

			MessageManager.Instance.SendMessage(new Message(MessageType.OnTimeChanged));
		}
	}
	private void Awake()
	{
		playerCharacter = GamePlayManager.Instance.PlayerCharacter;
		opponentCharacter = GamePlayManager.Instance.OpponentCharacter;
        Debug.Log(timer);
    }
    public void InitTurn()
    {
		if (GamePlayManager.Instance.CoinFlipOutCome == 0)
		{
			_turn = -1;
            //GamePlayManager.Instance.State = GameState.PlayerTurn;
		}
		else
		{
			_turn = 0;
			//GamePlayManager.Instance.State = GameState.OpponentTurn;
		}
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
            //GamePlayManager.Instance.State = GameState.PlayerTurn;
        }
        else
        {
            Debug.Log("AI turn");
            _player1.IsPlayerTurn = false;
            _player2.PlayTurn();
			//GamePlayManager.Instance.State = GameState.OpponentTurn;
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
                int previousPlayerHP = DataManager.Instance.PlayerHP;
                int previousOpponentHP = DataManager.Instance.OpponentHP;
                DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
                DataManager.Instance.OpponentHP = opponentCharacter.GetCurrentHP();
                StartCoroutine(HandleBattleAnimation(previousPlayerHP, previousOpponentHP));
            }
            else
            {
                ChangeTurn();
            }
        }
        else
        {
            PlayTurn();
        }

		timer = 16f;
	}
    private IEnumerator HandleBattleAnimation(int previousPlayerHP,int previousOpponentHP)
    {
        while(GamePlayManager.Instance.State == GameState.SkillAnimation)
            yield return null;
		GamePlayManager.Instance.State = GameState.BattleAnimation;
		UIManager.Instance.PlayBattleCollisionAnimation(DataManager.Instance.PlayerScore,DataManager.Instance.OpponentScore, previousPlayerHP, previousOpponentHP);
        yield return new WaitForSeconds(2f);
		DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
		DataManager.Instance.OpponentHP = opponentCharacter.GetCurrentHP();
		DataManager.Instance.PlayerScore = 0;
		DataManager.Instance.OpponentScore = 0;
		turnCounter = 0;
        CheckWinner();
		ChangeTurn();
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
        CheckWinner();
		if (GamePlayManager.Instance.State == GameState.PlayerWin || GamePlayManager.Instance.State == GameState.PlayerLose)
		{
			// Debug.Log("ChangeTurn: Game ended, skipping turn change.");
			return;
		}

		if (_turn == -1)
        {
            _turn = 1;
        }
		_turn = 1 - _turn;

        remainingActions = maxActionPerTurn;
		if (_turn == 0)
		{
			GamePlayManager.Instance.State = GameState.PlayerTurn;
			playerCharacter.RemoveActiveSkill();
			//ModifyExistenceSkillTurn(playerCharacter);
			opponentCharacter.ApplyBloodLoss(playerCharacter);
            DataManager.Instance.PlayerRemainActionPoints = remainingActions;
            DataManager.Instance.OpponentHP = opponentCharacter.GetCurrentHP();
            DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
            MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChangedDuringTurn));
        }
		else if (_turn == 1)
		{
			GamePlayManager.Instance.State = GameState.OpponentTurn;
			opponentCharacter.RemoveActiveSkill();
			//ModifyExistenceSkillTurn(opponentCharacter);
			playerCharacter.ApplyBloodLoss(opponentCharacter);
			DataManager.Instance.OpponentRemainActionPoints = remainingActions;
            DataManager.Instance.PlayerHP = playerCharacter.GetCurrentHP();
			DataManager.Instance.OpponentHP = opponentCharacter.GetCurrentHP();
			MessageManager.Instance.SendMessage(new Message(MessageType.OnDataChangedDuringTurn));
            //CheckWinner() ;
		}
		//isInAnimation = false;
		PlayTurn();
    }
	private void CheckWinner()
	{
		if (DataManager.Instance.PlayerHP == 0)
		{
			MessageManager.Instance.SendMessage(new Message(MessageType.OnGameLose));
		}
		else if (DataManager.Instance.OpponentHP == 0)
		{
			MessageManager.Instance.SendMessage(new Message(MessageType.OnGameWin));
		}
	}
	public int GetTurn()
    {
        return _turn;
    }

    public void SetTurn(int turn)
    {
        _turn = turn;
        //Debug.Log(_turn);
        //PlayTurn();
    }
}
