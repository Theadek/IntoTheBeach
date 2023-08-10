using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    public static Vector2Int GetMovedVector2Int(Vector2Int from, Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return from + new Vector2Int(1, 0);
            case Direction.South:
                return from + new Vector2Int(-1, 0);
            case Direction.West:
                return from + new Vector2Int(0, 1);
            case Direction.East:
                return from + new Vector2Int(0, -1);
            default:
                return from;
        }
    }
}

public enum Direction
{
    North, // x++
    South, // x--
    West,  // y++
    East,  // y--
    COUNT
}