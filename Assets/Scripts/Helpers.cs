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

    public static bool TryGetDirection(Vector2Int from, Vector2Int to, out Direction direction)
    {
        Vector2Int directionVector = to - from;
        if (directionVector.magnitude != 1)
        {
            direction = Direction.COUNT;
            return false;
        }

        if(directionVector == new Vector2Int(1, 0))
        {
            direction = Direction.North;
            return true;
        }
        else if (directionVector == new Vector2Int(-1, 0))
        {
            direction = Direction.South;
            return true;
        }
        else if (directionVector == new Vector2Int(0, 1))
        {
            direction = Direction.West;
            return true;
        }
        else if (directionVector == new Vector2Int(0, -1))
        {
            direction = Direction.East;
            return true;
        }
        else
        {
            //not sure how it got here
            direction = Direction.COUNT;
            return false;
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