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

    void Start()
    {
        playerCharacter = GamePlayManager.Instance.PlayerCharacter;
        opponentCharacter = GamePlayManager.Instance.OpponentCharacter;
        _turn = -1;
        remainingActions = maxActionPerTurn;

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
        Debug.Log("Actions left: " + remainingActions);
        if (remainingActions == 0)
        {
            EndOfRound();
            ChangeTurn();
        }
        else
        {
            PlayTurn();
        }
    }
	public void AddExtraAction(int extra)
	{
		remainingActions += extra;
		Debug.Log("gain " + extra + " action");
	}
	public void ChangeTurn()
    {
        if (_turn == -1)
        {
            _turn = 1;
        }
        
        _turn = 1 - _turn;
        remainingActions = maxActionPerTurn;
		if (_turn == 0 && opponentCharacter != null)
		{
			opponentCharacter.ApplyBloodLoss(playerCharacter);
		}
		else if (_turn == 1 && playerCharacter != null)
		{
			playerCharacter.ApplyBloodLoss(opponentCharacter);
		}

		PlayTurn();
    }

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
