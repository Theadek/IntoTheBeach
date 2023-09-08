using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }

    }

    public GameObject pushN;
    public GameObject pushS;
    public GameObject pushE;
    public GameObject pushW;
    public GameObject tileHighlight;
    public GameObject Dot;
}