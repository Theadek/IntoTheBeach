using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnSelectedObject;
    public event EventHandler OnGameStateChanged;

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

    private void Update()
    {
        switch(gamestate)
        {
            case GameState.Setup:
                // TODO - Player Should be able to deploy units
                ChangeGameState(GameState.EnemyMove);
                break;
            case GameState.EnemyMove:
                // TODO - Enemies should find places to move
                ChangeGameState(GameState.EnemyPrepareEmerge);
                break;
            case GameState.EnemyPrepareEmerge:
                // TODO - Enemies choose places to emerge next turn
                ChangeGameState(GameState.PlayerTurn);
                break;
            case GameState.PlayerTurn:
                // TODO - Player can choose to move -> revert move;
                //        attack (not revertable);
                //        one time revert whole turn
                //        Ends when player press finish turn

                //ChangeGameState(GameState.Hazards);
                break;
            case GameState.Hazards:
                // TODO - All units can take damage from hazards (like fire or meteors)
                ChangeGameState(GameState.EnemyAttack);
                break;
            case GameState.EnemyAttack:
                // TODO - Enemies finish telegraphed attacks
                ChangeGameState(GameState.EnemyEmerge);
                break;
            case GameState.EnemyEmerge:
                // TODO - Enemies emerges from ground or get stopped
                ChangeGameState(GameState.EnemyMove);
                break;
            case GameState.Win:
                // TODO - ?
                break;
            case GameState.GameOver:
                // TODO - Show Game Over Screen
                break;
        }
    }

    public void TileLeftClicked(Tile tile)
    {
        if (IsPlayerTurn())
        {
            if (tile.TryGetObjectOnThisTile(out TileObject tileObject))
            {
                // Tile has Object
                if (tileObject.IsPlayerType())
                {
                    if(tileObject == selectedObject)
                    {
                        SetSelectedObject(null);
                    }
                    else
                    {
                        SetSelectedObject(tileObject);
                    }
                }
            }
            else
            {
                // Tile is empty
                if (HasSelectedObject())
                {
                    if (PathFinder.Instance.ListConstainsXY(tile.GetXY()) &&
                        selectedObject.CanMove())
                    {
                        TileManager.Instance.MoveTileObject(selectedObject, tile);
                    }
                }
                SetSelectedObject(null);
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
        EnemyMove,
        EnemyPrepareEmerge,
        PlayerTurn,
        Hazards,
        EnemyAttack,
        EnemyEmerge,
        Win,
        GameOver
    }

    public bool IsPlayerTurn()
    {
        return gamestate == GameState.PlayerTurn;
    }

    private void ChangeGameState(GameState state)
    {
        gamestate = state;
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
