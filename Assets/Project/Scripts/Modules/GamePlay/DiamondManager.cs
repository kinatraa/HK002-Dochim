using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DiamondManager : MonoBehaviour
{
    public Tilemap Tilemap;
    public TileBase[] DTiles;
    public int Rows = 8;
    public int Columns = 16;

    void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int i = -Rows / 2; i < Rows / 2; i++)
        {
            for (int j = -Columns / 4; j < Columns / 4; j++)
            {
                Tilemap.SetTile(new Vector3Int(i, j, 0), DTiles[Random.Range(0, DTiles.Length)]);
            }

            for (int j = Columns / 4; j < Columns - Columns / 4; j++)
            {
                Tilemap.SetTile(new Vector3Int(i, j, 0), DTiles[Random.Range(0, DTiles.Length)]);
            }
        }
    }
}
