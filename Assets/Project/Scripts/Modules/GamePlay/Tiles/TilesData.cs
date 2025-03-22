using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesData : MonoBehaviour
{
    [SerializeField] private List<TempData> tempDataList = new List<TempData>();
    public Dictionary<TileBase, TileProperties> TilesDictionary = new Dictionary<TileBase, TileProperties>();

    void Awake()
    {
        for (int i = 0; i < tempDataList.Count; i++)
        {
            TilesDictionary[tempDataList[i].Tile] = tempDataList[i].Properties;
        }
    }
}

[Serializable]
public class TempData
{
    public TileBase Tile;
    public TileProperties Properties;
}
