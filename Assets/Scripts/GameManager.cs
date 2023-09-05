using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler OnSelectedObjectChanged;
    public event EventHandler OnSelectedTypeChanged;
    public event EventHandler OnGameStateChanged;
    public event EventHandler OnPlayerFinishTurn;

    public static GameManager Instance { get; private set; }
    private GameState gamestate;

    private TileObject selectedObject;
    private SelectionType selectionType = SelectionType.NULL;

    private Stack<TileObject> lastMovements = new Stack<TileObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        gamestate = GameState.Setup;
    }

    private void Start()
    {
        GameInput.Instance.OnRevertLastMove += GameInput_OnRevertLastMove;
        GameInput.Instance.OnRepair += GameInput_OnRepair;
        GameInput.Instance.OnEndTurn += GameInput_OnEndTurn;
        GameInput.Instance.OnFirstWeaponUse += GameInput_OnFirstWeaponUse;
        GameInput.Instance.OnSecondWeaponUse += GameInput_OnSecondWeaponUse;
    }

    private void GameInput_OnSecondWeaponUse(object sender, EventArgs e)
    {
        if (!IsPlayerTurn())
            return;
        if (!HasSelectedObject())
            return;
        if (selectedObject.GetHasDoneAction())
            return;
        if (!selectedObject.HasSecondWeapon())
            return;
        if (selectedObject.IsSecondWeaponPasive())
            return;

        if (IsSelectedSecondWeapon())
        {
            SetSelectedObject(selectedObject);
        }
        else
        {
            selectionType = SelectionType.SecondWeapon;
            OnSelectedTypeChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    private void GameInput_OnFirstWeaponUse(object sender, EventArgs e)
    {
        if (!IsPlayerTurn())
            return;
        if (!HasSelectedObject())
            return;
        if (selectedObject.GetHasDoneAction())
            return;
        if (!selectedObject.HasFirstWeapon())
            return;
        if (selectedObject.IsFirstWeaponPasive())
            return;

        if (IsSelectedFirstWeapon())
        {
            SetSelectedObject(selectedObject);
        }
        else
        {
            selectionType = SelectionType.FirstWeapon;
            OnSelectedTypeChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    private void GameInput_OnRepair(object sender, EventArgs e)
    {
        if (!IsPlayerTurn() || !HasSelectedObject())
            return;
        if (selectedObject.GetHasDoneAction())
            return;

        selectedObject.RepairSelf();
        SetSelectedObject(selectedObject);
        ClearLastPlayerMovement();
    }

    private void GameInput_OnEndTurn(object sender, EventArgs e)
    {
        if (!IsPlayerTurn())
            return;
        SetSelectedObject(null);
        EndPlayerTurn();
    }

    private void GameInput_OnRevertLastMove(object sender, EventArgs e)
    {
        if (!IsPlayerTurn())
            return;
        if(TryGetLastPlayerMovementTileObject(out TileObject tileObject))
        {
            TileManager.Instance.RevertMovePlayerTileObject(tileObject);
            SetSelectedObject(null);
        }
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
                // TODO - attack (not revertable);
                //        one time revert whole turn

                //Waiting for Player Input to finish Player Turn
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
            if (selectionType == SelectionType.NULL ||
                selectionType == SelectionType.Movement)
            {
                if (tile.TryGetTileObject(out TileObject tileObject))
                {
                    // Tile has Object
                    if (tileObject.IsPlayerType())
                    {
                        if (tileObject == selectedObject)
                        {
                            SetSelectedObject(null);
                        }
                        else
                        {
                            SetSelectedObject(tileObject);
                        }
                    }
                    else if (tileObject.IsEnemyType())
                    {
                        // TODO Show Enemies Attack Details???
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
                            TileManager.Instance.MovePlayerTileObject(selectedObject, tile);
                            SaveLastPlayerMovement();
                            SetSelectedObject(selectedObject);
                        }
                        else
                        {
                            SetSelectedObject(null);
                        }
                    }
                    else
                    {
                        SetSelectedObject(null);
                    }
                }
            }
            else if(selectionType == SelectionType.FirstWeapon)
            {
                if (selectedObject.IsTileInRangeFirstWeapon(tile))
                {
                    selectedObject.AttackTileFirstWeapon(tile);
                    TileManager.Instance.CheckHealthToDestroyed();
                    SetSelectedObject(null);
                    ClearLastPlayerMovement();
                }
                else
                {
                    SetSelectedObject(null);
                }
            }
            else if(selectionType == SelectionType.SecondWeapon)
            {
                if (selectedObject.IsTileInRangeSecondWeapon(tile))
                {
                    selectedObject.AttackTileSecondWeapon(tile);
                    TileManager.Instance.CheckHealthToDestroyed();
                    SetSelectedObject(null);
                    ClearLastPlayerMovement();
                }
                else
                {
                    SetSelectedObject(null);
                }
            }
        }
    }


    // DEBUG FOR NOW
    public void TileRightClicked(Tile tile)
    {
        if (IsPlayerTurn())
        {
            if (tile.TryGetTileObject(out TileObject tileObject))
            {
                tileObject.GetDamaged(1);
            }
        }
    }

    public void SetSelectedObject(TileObject selectedObject)
    {
        this.selectedObject = selectedObject;

        if(selectedObject == null)
        {
            selectionType = SelectionType.NULL;
        }
        else
        {
            selectionType = SelectionType.Movement;
        }

        OnSelectedObjectChanged?.Invoke(this, EventArgs.Empty);
        OnSelectedTypeChanged?.Invoke(this, EventArgs.Empty);
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


    private enum SelectionType
    {
        Movement,
        FirstWeapon,
        SecondWeapon,
        NULL
    }

    public bool IsSelectedMovement() => selectionType == SelectionType.Movement;
    public bool IsSelectedFirstWeapon() => selectionType == SelectionType.FirstWeapon;
    public bool IsSelectedSecondWeapon() => selectionType == SelectionType.SecondWeapon;


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

    public void EndPlayerTurn()
    {
        // TODO Check if moves not done, and display warning?
        ClearLastPlayerMovement();
        OnPlayerFinishTurn?.Invoke(this, EventArgs.Empty);
        ChangeGameState(GameState.Hazards);
    }

    private void ChangeGameState(GameState state)
    {
        gamestate = state;
        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SaveLastPlayerMovement()
    {
        lastMovements.Push(selectedObject);
    }

    private bool TryGetLastPlayerMovementTileObject(out TileObject tileObject)
    {
        return lastMovements.TryPop(out tileObject);
    }

    private void ClearLastPlayerMovement()
    {
        lastMovements.Clear();
    }


    //DEBUG

    public GameObject MyInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        return Instantiate(prefab, position, rotation, parent);
    }
}
