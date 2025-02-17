using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAI : AIBehavior
{
    public override List<Vector3Int> SelectTile()
    {
        List<Vector3Int> selectedPos = new List<Vector3Int>();
        
        for (int i = _bounds.xMin; i < _bounds.xMax; i++)
        {
            for (int j = _bounds.yMin; j < _bounds.yMax; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);

                foreach (var dir in _directions)
                {
                    if(!GamePlayManager.Instance.ValidClick(pos + dir)) continue;
                    
                    selectedPos.Add(pos);
                    selectedPos.Add(pos + dir);
                    //wrong
                    return selectedPos;
                }
            }
        }
        
        return selectedPos;
    }
}
