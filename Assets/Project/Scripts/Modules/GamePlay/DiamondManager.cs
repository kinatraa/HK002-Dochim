using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiamondManager : MonoBehaviour
{
    public GameObject[] DPrefabs;
    public Transform Board;
    public int Rows = 8;
    public int Columns = 8;

    private GameObject[,] _diamonds;

    void Start()
    {
        _diamonds = new GameObject[Rows, Columns];
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                int k = Random.Range(0, DPrefabs.Length);

                GameObject diamond = Instantiate(DPrefabs[k], Board);
                _diamonds[i, j] = diamond;
            }
        }
    }
}
