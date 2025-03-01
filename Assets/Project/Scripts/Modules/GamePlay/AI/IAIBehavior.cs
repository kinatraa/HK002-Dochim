using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IAIBehavior
{
    public void SetDiamondClick(DiamondClick diamondClick);
    public IEnumerator SelectTile();
    
    public List<Tuple<Vector3Int, Vector3Int>> GetSwapableTiles();
}
