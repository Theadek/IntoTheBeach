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

    public static Vector2Int GetVector2IntFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Vector2Int(1, 0);
            case Direction.South:
                return new Vector2Int(-1, 0);
            case Direction.West:
                return new Vector2Int(0, 1);
            case Direction.East:
                return new Vector2Int(0, -1);
            default:
                Debug.LogWarning("GetVector2IntFromDirection function got unknown Direction value!");
                return Vector2Int.zero;
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
        direction = GetDirection(directionVector);
        return true;
    }

    public static bool TryGetDirectionLong(Vector2Int from, Vector2Int to, out Direction direction, out int distance)
    {
        Vector2Int directionVector = to - from;
        if((directionVector.x != 0 && directionVector.y != 0) ||
           (directionVector.x == 0 && directionVector.y == 0))
        {
            direction = Direction.COUNT;
            distance = 0;
            return false;
        }

        direction = GetDirection(directionVector / (int)directionVector.magnitude);
        distance = (int)directionVector.magnitude;
        return true;
    }

    private static Direction GetDirection(Vector2Int direction)
    {
        if (direction == new Vector2Int(1, 0))
        {
            return Direction.North;
        }
        else if (direction == new Vector2Int(-1, 0))
        {
            return Direction.South;

        }
        else if (direction == new Vector2Int(0, 1))
        {
            return Direction.West;

        }
        else if (direction == new Vector2Int(0, -1))
        {
            return Direction.East;

        }
        else
        {
            //not sure how it got here
            return Direction.COUNT;
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