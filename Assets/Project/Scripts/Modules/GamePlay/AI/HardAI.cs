using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HardAI : AIBehavior
{
    public override IEnumerator SelectTile()
    {
        yield return base.SelectTile();

        _pretendDiamondManager.SetSwappableTiles(_swappableTiles);
        _pretendDiamondManager.CalculateAllCasesScore();
        List<Dictionary<TileColor, int>> colorCounter = _pretendDiamondManager.ColorCounter;
        List<int> scoreCounter = _pretendDiamondManager.ScoreCounter;

        int idx = 0;
        for (int i = 0; i < scoreCounter.Count; i++)
        {
            if (scoreCounter[i] > scoreCounter[idx])
            {
                idx = i;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        yield return _diamondClick.SelectTile(_swappableTiles[idx].Item1);
        yield return new WaitForSeconds(1f);
        yield return _diamondClick.SelectTile(_swappableTiles[idx].Item2);
        
        yield return null;
    }

    
}
