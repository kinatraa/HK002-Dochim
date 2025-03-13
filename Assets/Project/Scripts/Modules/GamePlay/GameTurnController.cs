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

    void Start()
    {
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
        if(remainingActions == 0)
            ChangeTurn();
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
		PlayTurn();
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
