using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{

    protected Tile tile;
    protected int health;
    protected ObjectType objectType;




    protected enum ObjectType
    {
        Player,
        Enemy,
        Neutral,
    }
}
