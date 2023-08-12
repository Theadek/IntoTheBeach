using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField] private CharacterSO characterSO;
    protected MovementReverter movementReverter;

    // Variables
    protected Tile tile;
    protected new string name;
    protected int health;
    protected int healthMax;
    protected int movement;
    [SerializeField] protected ObjectType objectType;

    // Abilities
    protected bool hasMoved; // 1 Move per turn
    protected bool canMove;  // Is not sneared in place

    protected bool movable;  // Can move (is not a building or mountain)
    protected bool pushable;
    protected bool hasArmor;
    protected bool hasShield;
    protected bool explosive;
    protected bool isFlying;


    private void Awake()
    {
        name = characterSO.name;
        health = characterSO.health;
        healthMax = characterSO.health;
        movement = characterSO.movement;

        hasMoved = false;
        canMove = characterSO.movable;

        movable = characterSO.movable;
        pushable = characterSO.pushable;
        hasArmor = characterSO.hasArmor;
        hasShield = characterSO.hasShield;
        explosive = characterSO.explosive;
        isFlying = characterSO.isFlying;


        movementReverter.previousPosition = null;
        movementReverter.canRevert = false;
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerFinishTurn += GameManager_OnPlayerFinishTurn;
    }

    private void GameManager_OnPlayerFinishTurn(object sender, System.EventArgs e)
    {
        hasMoved = false;
    }

    #region GET SET Tile
    public Tile GetTile()
    {
        return tile;
    }

    public void SetTile(Tile tile)
    {
        this.tile = tile;
    }
    #endregion

    #region GET movement
    public int GetMovement() => movement;

    public bool CanMove()
    {
        return !hasMoved && canMove && movable;
    }

    public void ObjectJustMoved()
    {
        hasMoved = true;
    }

    public void ResetMovement()
    {
        hasMoved = false;
    }
    #endregion

    #region GET isFlying
    public bool IsFlying() => isFlying;
    #endregion

    #region CharacterSO
    public CharacterSO GetCharacterSO()
    {
        return characterSO;
    }

    protected void SetCharacterSO(CharacterSO characterSO)
    {
        this.characterSO = characterSO;
    }
    #endregion

    #region Damage Heal Push

    public void GetDamaged(int amount)
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }
        else if(hasArmor)
        {
            amount -= 1;
            if (amount <= 0)
                return;
        }

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            GetDestroyed();
        }
    }

    private void GetDamagedFromPush()
    {
        if (hasShield)
        {
            hasShield = false;
            return;
        }
        int pushDamage = 1;
        health -= pushDamage;
        if (health <= 0)
        {
            health = 0;
            GetDestroyed();
        }
    }

    public void GetPushed(Direction direction)
    {
        if (!pushable) return;

        if(TileManager.Instance.TryGetNeighborTile(tile, direction, out Tile newTile))
        {
            if(newTile.TryGetTileObject(out TileObject newTileObject))
            {
                newTileObject.GetDamagedFromPush();
                this.GetDamagedFromPush();
            }
            else
            {
                TileManager.Instance.MoveTileObjectByOne(tile, direction);
            }
        }
    }

    protected void GetDestroyed()
    {
        if (explosive)
        {
            TileObject newTileObject;
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.North, out newTileObject))
            {
                int explosionDamage = 1;
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.South, out newTileObject))
            {
                int explosionDamage = 1;
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.West, out newTileObject))
            {
                int explosionDamage = 1;
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.East, out newTileObject))
            {
                int explosionDamage = 1;
                newTileObject.GetDamaged(explosionDamage);
            }
        }
        tile.RemoveTileObject();
        Destroy(gameObject);
    }


    public void GetHealed(int amount)
    {
        health += amount;
        if(health > healthMax)
            health = healthMax;
    }
    #endregion

    #region enum ObjectType
    // Enum Object Type
    public enum ObjectType
    {
        Player,
        Enemy,
        Neutral,
    }

    public bool IsPlayerType()
    {
        return objectType == ObjectType.Player;
    }

    public bool IsEnemyType()
    {
        return objectType == ObjectType.Enemy;
    }

    public ObjectType GetObjectType()
    {
        return objectType;
    }

    public bool HasSameType(TileObject otherTileObject)
    {
        return objectType == otherTileObject.objectType; //TODO shouldn't be otherTileObject.GetObjectType() ???
    }
    #endregion

    #region Movement Reverter
    protected struct MovementReverter
    {
        public Tile previousPosition;
        public bool canRevert;
    }

    public void SavePreviousPosition()
    {
        movementReverter.previousPosition = GetTile();
        movementReverter.canRevert = true;
    }

    public bool TryGetPreviousPosition(out Tile previousPosition)
    {
        if (!movementReverter.canRevert)
        {
            previousPosition = null;
            return false;
        }
        previousPosition = movementReverter.previousPosition;
        return true;
    }

    #endregion

    // Static TileObject Creation
    public static TileObject CreateTileObject(Tile parent, CharacterSO characterSO)
    {
        var newObject = Instantiate(characterSO.prefab, parent.transform.position, Quaternion.identity, parent.transform);
        var newTileObject = newObject.GetComponent<TileObject>();
        newTileObject.SetTile(parent);
        newTileObject.SetCharacterSO(characterSO);
        return newTileObject;
    }
}
