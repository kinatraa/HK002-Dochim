using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnController : MonoBehaviour
{
    [SerializeField] private PlayerController _player1;
    [SerializeField] private AIController _player2;

    private int _turn = 0;

    void Start()
    {
        _turn = 1;
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

    public void ChangeTurn()
    {
        _turn = 1 - _turn;
        
        PlayTurn();
    }
}
