using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TileProperties
{
    public TileColor Color;
    public TileType Type;
}

public enum TileType
{
    Normal,
    Column,
    Row,
    Area,
}

public enum TileColor
{
    Red,
    Green,
    Cyan,
    Yellow,
    Purple,
}
