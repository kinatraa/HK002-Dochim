using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utils
{
    public static List<(Vector3Int, Vector3Int)> GetAllPossibleMoves(Tilemap tilemap)
    {
        BoundsInt bounds = GamePlayManager.Instance.BoardBounds;
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
        };

        List<(Vector3Int, Vector3Int)> list = new List<(Vector3Int, Vector3Int)>();
        Vector3Int curTile = Vector3Int.zero;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                curTile.x = x;
                curTile.y = y;

                foreach (Vector3Int dir in directions)
                {
                    if (!GamePlayManager.Instance.IsInBound(curTile + dir)) continue;
                    if (!CanSwap(curTile, curTile + dir, 1, tilemap)) continue;

                    list.Add((curTile, curTile + dir));
                }
            }
        }

        return list;
    }

    //rev = 0: check after real swap
    //rev = 1: check before real swap
    public static bool CanSwap(Vector3Int a, Vector3Int b, int rev, Tilemap tilemap)
    {
        DiamondManager diamondManager = GamePlayManager.Instance.DiamondManager;
        BoundsInt bounds = GamePlayManager.Instance.BoardBounds;

        if (diamondManager.IsLocked(a) || diamondManager.IsLocked(b))
        {
            return false;
        }

        Vector3Int[] checkPoss = { a, b };

        int cnt = 1;
        Vector3Int pos = a;
        while (pos.x - 1 >= bounds.xMin)
        {
            --pos.x;
            if (rev == 1 && pos == b) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[rev], tilemap)) ++cnt;
            else break;
        }

        pos = a;
        while (pos.x + 1 < bounds.xMax)
        {
            ++pos.x;
            if (rev == 1 && pos == b) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[rev], tilemap)) ++cnt;
            else break;
        }

        if (cnt >= 3)
        {
            return true;
        }

        cnt = 1;
        pos = a;
        while (pos.y - 1 >= bounds.yMin)
        {
            --pos.y;
            if (rev == 1 && pos == b) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[rev], tilemap)) ++cnt;
            else break;
        }

        pos = a;
        while (pos.y + 1 < bounds.yMax)
        {
            ++pos.y;
            if (rev == 1 && pos == b) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[rev], tilemap)) ++cnt;
            else break;
        }

        if (cnt >= 3)
        {
            return true;
        }

        cnt = 1;
        pos = b;
        while (pos.x - 1 >= bounds.xMin)
        {
            --pos.x;
            if (rev == 1 && pos == a) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[1 - rev], tilemap)) ++cnt;
            else break;
        }

        pos = b;
        while (pos.x + 1 < bounds.xMax)
        {
            ++pos.x;
            if (rev == 1 && pos == a) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[1 - rev], tilemap)) ++cnt;
            else break;
        }

        if (cnt >= 3)
        {
            return true;
        }

        cnt = 1;
        pos = b;
        while (pos.y - 1 >= bounds.yMin)
        {
            --pos.y;
            if (rev == 1 && pos == a) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[1 - rev], tilemap)) ++cnt;
            else break;
        }

        pos = b;
        while (pos.y + 1 < bounds.yMax)
        {
            ++pos.y;
            if (rev == 1 && pos == a) break;
            if (GamePlayManager.Instance.SameTileColor(pos, checkPoss[1 - rev], tilemap)) ++cnt;
            else break;
        }

        if (cnt >= 3)
        {
            return true;
        }

        return false;
    }
}