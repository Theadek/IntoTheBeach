using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private int waitingForAnimationStage;

    private TileObject selectedObject;
    private SelectionType selectionType = SelectionType.NULL;

    private Stack<TileObject> lastMovements = new Stack<TileObject>();

    private List<TileObject> Enemies = new List<TileObject>();

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
        waitingForAnimationStage = 0;

        GameInput.Instance.OnRevertLastMove += GameInput_OnRevertLastMove;
        GameInput.Instance.OnRepair += GameInput_OnRepair;
        GameInput.Instance.OnEndTurn += GameInput_OnEndTurn;
        GameInput.Instance.OnFirstWeaponUse += GameInput_OnFirstWeaponUse;
        GameInput.Instance.OnSecondWeaponUse += GameInput_OnSecondWeaponUse;
        GameInput.Instance.OnCancel += Instance_OnCancel;
    }

    private void Instance_OnCancel(object sender, EventArgs e)
    {
        if (waitingForAnimationStage == 1) return;

        if (IsPlayerTurn())
        {
            SetSelectedObject(null);
        }
    }

    #region OnEvents
    private void GameInput_OnSecondWeaponUse(object sender, EventArgs e)
    {
        if (waitingForAnimationStage == 1)
            return;
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
        if (waitingForAnimationStage == 1)
            return;
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
        if (waitingForAnimationStage == 1)
            return;
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
        if (waitingForAnimationStage == 1)
            return;
        if (!IsPlayerTurn())
            return;
        SetSelectedObject(null);
        EndPlayerTurn();
    }

    private void GameInput_OnRevertLastMove(object sender, EventArgs e)
    {
        if (waitingForAnimationStage == 1)
            return;
        if (!IsPlayerTurn())
            return;
        if(TryGetLastPlayerMovementTileObject(out TileObject tileObject))
        {
            TileManager.Instance.RevertMovePlayerTileObject(tileObject);
            SetSelectedObject(null);
        }
    }

    #endregion

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
                // 1) Enemies Moves
                if(waitingForAnimationStage == 0)
                {
                    waitingForAnimationStage = 1;
                    EnemyMoves();
                }
                else if (waitingForAnimationStage == 2)
                {
                    waitingForAnimationStage = 0;
                    ChangeGameState(GameState.EnemyPrepareEmerge);
                }
                break;
            case GameState.EnemyPrepareEmerge:
                // TODO - Enemies choose places to emerge next turn
                if (waitingForAnimationStage == 0)
                {
                    waitingForAnimationStage = 1;
                    EnemyPrepareEmerge();
                }
                else if (waitingForAnimationStage == 2)
                {
                    waitingForAnimationStage = 0;
                    ChangeGameState(GameState.PlayerTurn);
                }
                break;
            case GameState.PlayerTurn:
                // TODO - one time revert whole turn

                //Waiting for Player Input to finish Player Turn
                break;
            case GameState.Hazards:
                // TODO - All units can take damage from hazards (like fire or meteors)
                ChangeGameState(GameState.EnemyAttack);
                break;
            case GameState.EnemyAttack:
                // TODO - Enemies finish telegraphed attacks
                if(waitingForAnimationStage == 0)
                {
                    waitingForAnimationStage = 1;
                    EnemyAttack();
                }
                else if(waitingForAnimationStage == 2)
                {
                    waitingForAnimationStage = 0;
                    ChangeGameState(GameState.EnemyEmerge);
                }
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

    private async void EnemyMoves()
    {
        foreach (TileObject enemy in Enemies)
        {
            PathFinder.PathNode pathNode = enemy.GetComponent<EnemyAI>().CalculateMovement();
            await TileManager.Instance.MoveTileObjectOnPath(
                enemy.GetTile(),
                pathNode);

            enemy.GetComponent<EnemyAI>().CalculateAttack();

            await Task.Delay(500);

        }
        waitingForAnimationStage = 2;
    }

    private async void EnemyAttack()
    {
        bool allEnemiesAttacked = false;
        while (!allEnemiesAttacked)
        {
            allEnemiesAttacked = true;
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (Enemies[i].GetComponent<EnemyAI>().IsAbleToAttack())
                {
                    if (Enemies[i].TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
                    {
                        await enemyAI.Attack();
                        await Task.Delay(500);
                    }
                    else
                    {
                        Debug.LogWarning("No EnemyAI Script in Enemy TileObject");
                    }
                    allEnemiesAttacked = false;
                    break;
                }
            }
        }
        waitingForAnimationStage = 2;
    }

    private async void EnemyPrepareEmerge()
    {
        //Calculate how many enemies + already waiting for emerge

        //Choose places to emerge

        //Show Places to emerge (Invoke an event)

        await Task.Yield();

        waitingForAnimationStage = 2;
    }


    public void TileLeftClicked(Tile tile)
    {
        if (waitingForAnimationStage == 1) return;

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
                            waitingForAnimationStage = 1;
                            MovePlayerAsync(selectedObject, tile);
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

    private async void MovePlayerAsync(TileObject tileObject, Tile to)
    {
        await TileManager.Instance.MovePlayerTileObjectOnPath(tileObject, to);
        SaveLastPlayerMovement();
        SetSelectedObject(tileObject);
        waitingForAnimationStage = 0;
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

    //Not used
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

    public void RegisterEnemy(TileObject enemy)
    {
        Enemies.Add(enemy);
    }

    public void UnregisterEnemy(TileObject enemy)
    {
        Enemies.Remove(enemy);
    }

    public void RecalculateEnemyAttacks()
    {
        foreach(TileObject enemy in Enemies)
        {
            enemy.GetComponent<EnemyAI>().RecalculateAttack();
        }
    }
}
