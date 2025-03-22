using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EasyAI : AIBehavior
{
    public override IEnumerator SelectTile()
    {
        yield return base.SelectTile();
        
        int randomIdx = Random.Range(0, _swappableTiles.Count);

        yield return new WaitForSeconds(0.5f);
        yield return _diamondClick.SelectTile(_swappableTiles[randomIdx].Item1);
        yield return new WaitForSeconds(1f);
        yield return _diamondClick.SelectTile(_swappableTiles[randomIdx].Item2);
        
        yield return null;
    }
}
