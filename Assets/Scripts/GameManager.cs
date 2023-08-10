using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnSelectedObject;

    public static GameManager Instance { get; private set; }
    private GameState gamestate;

    private TileObject selectedObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gamestate = GameState.Setup;
    }

    public void TileLeftClicked(Tile tile)
    {
        if (IsPlayerTurn() || true) //TODO Change that after GameState is implemented
        {
            if (tile.TryGetObjectOnThisTile(out TileObject tileObject))
            {
                if (tileObject.IsPlayerType())
                {
                    SetSelectedObject(tileObject);
                }
                Debug.Log($"Clicked on {tileObject.name}");
            }
            else
            {
                if (HasSelectedObject())
                {
                    if (PathFinding.Instance.ListConstainsXY(tile.GetXY()))
                    {
                        TileManager.Instance.MoveTileObject(selectedObject, tile);
                    }
                }
                SetSelectedObject(null);

                Debug.Log("Clicked on Empty Tile");
            }
        }
    }


    public void TileRightClicked(Tile tile)
    {
        // Display Tooltip
    }

    public void SetSelectedObject(TileObject selectedObject)
    {
        this.selectedObject = selectedObject;
        OnSelectedObject?.Invoke(this, EventArgs.Empty);
    }

    public bool HasSelectedObject()
    {
        return Instance.selectedObject != null;
    }

    public bool TryGetSelectedObject(out TileObject tileObject)
    {
        tileObject = selectedObject;
        return HasSelectedObject();
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
