using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class HardAI : AIBehavior
{
    [Tooltip("Accuracy in the range of 0 to 100 (0 means random)")]
    [SerializeField] private int _accuracy = 60;
    private List<Dictionary<TileColor, int>> colorCounter;
    private List<int> scoreCounter;
    
    public override IEnumerator SelectTile()
    {
        yield return base.SelectTile();

        _pretendDiamondManager.SetPossibleMoves(_possibleMoves);
        _pretendDiamondManager.CalculateAllCasesScore();
        colorCounter = _pretendDiamondManager.ColorCounter;
        scoreCounter = _pretendDiamondManager.ScoreCounter;
        
        int idx = GetMove();
        
        yield return new WaitForSeconds(0.5f);
        yield return _diamondClick.SelectTile(_possibleMoves[idx].Item1);
        yield return new WaitForSeconds(1f);
        yield return _diamondClick.SelectTile(_possibleMoves[idx].Item2);
        
        yield return null;
    }

    protected override int GetMove()
    {
        if (Random.Range(1, 101) > _accuracy)
        {
            return Random.Range(0, _possibleMoves.Count);
        }
        
        BaseCharacter character = GamePlayManager.Instance.OpponentCharacter;
        
        List<int> idxPool = new List<int>();
        
        for (int i = 0; i < colorCounter.Count; i++)
        {
            int cnt = 0;
            if (character.conditionTile.Count > 0)
            {
                foreach (var t in character.conditionTile)
                {
                    var c = _tilesData[t].Color;
                    cnt += colorCounter[i][c];
                }
            }
            else
            {
                foreach (var p in colorCounter[i])
                {
                    cnt += p.Value;
                }
            }

            if (cnt + character.currentConditionAmount >= character.activeConditionAmount)
            {
                idxPool.Add(i);
            }
        }
        
        int idx = 0;

        if (idxPool.Count > 0)
        {
            for (int i = 0; i < idxPool.Count; i++)
            {
                if (scoreCounter[idxPool[i]] > scoreCounter[idx])
                {
                    idx = idxPool[i];
                }
            }
        }
        else
        {
            // brute - force
            for (int i = 0; i < scoreCounter.Count; i++)
            {
                if (scoreCounter[i] > scoreCounter[idx])
                {
                    idx = i;
                }
            }
        }
        
        return idx;
    }
    
    /*private int Minimax(Tilemap board, int depth, bool isMaximizingPlayer, int alpha, int beta)
    {
        if (depth == 0)
            return EvaluateBoard(board);

        if (isMaximizingPlayer)  // AI turn
        {
            int maxEval = int.MinValue;
            foreach (var move in board.GetAllPossibleMoves())
            {
                Tilemap newBoard = board.MakeMove(move);
                int eval = Minimax(newBoard, depth - 1, false, alpha, beta);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break; // Cắt tỉa alpha-beta
            }
            return maxEval;
        }
        else  // Player turn
        {
            int minEval = int.MaxValue;
            foreach (var move in board.GetAllPossibleMoves())
            {
                Tilemap newBoard = board.MakeMove(move);
                int eval = Minimax(newBoard, depth - 1, true, alpha, beta);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break; // Cắt tỉa alpha-beta
            }
            return minEval;
        }
    }
    
    private int EvaluateBoard(Tilemap board)
    {
        return board.AI_Score - board.Player_Score;
    }*/
}
