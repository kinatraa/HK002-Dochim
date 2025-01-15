using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class DiamondClick : MonoBehaviour
{
    public Tilemap Tilemap;

    private Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = Tilemap.WorldToCell(worldPoint);
            if (Tilemap.GetTile(gridPosition) != null)
            {
                Tilemap.SetTile(gridPosition, null);
                DropTile();
            }
        }
    }

    private void DropTile()
    {
        BoundsInt bounds = Tilemap.cellBounds;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                if (Tilemap.GetTile(new Vector3Int(x, y, 0)) != null) continue;
                Queue<Vector3Int> aboveTiles = new Queue<Vector3Int>();

                GetAboveTiles(aboveTiles, x, y);
                DropAboveTiles(aboveTiles, x, y);
            }
        }
    }

    private void GetAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        BoundsInt bounds = Tilemap.cellBounds;

        for (int k = y + 1; k < bounds.yMax; k++)
        {
            if (Tilemap.GetTile(new Vector3Int(x, k, 0)) == null) continue;

            aboveTiles.Enqueue(new Vector3Int(x, k, 0));
        }
    }

    private void DropAboveTiles(Queue<Vector3Int> aboveTiles, int x, int y)
    {
        while (aboveTiles.Count > 0)
        {
            Vector3Int tile = aboveTiles.Dequeue();
            Tilemap.SetTile(new Vector3Int(x, y++, 0), Tilemap.GetTile(tile));
            Tilemap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }
}