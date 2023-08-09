using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private GameState gamestate;

    private TileObject selectedObject;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        gamestate = GameState.Setup;
    }


    public void SetSelectedObject(TileObject selectedObject)
    {
        this.selectedObject = selectedObject;
    }

    private enum GameState
    {
        Setup,
        PlayerTurn,
        Hazards,
        EnemySpawn,
        EnemyTurn,
        EnemyFinishing,
        NeutralTurn,
        GameOver
    }

    public bool IsPlayerTurn()
    {
        return gamestate == GameState.PlayerTurn;
    }
}
