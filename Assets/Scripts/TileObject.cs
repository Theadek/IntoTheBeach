using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public event EventHandler OnHealthChanged;


    [SerializeField] private CharacterSO characterSO;
    protected MovementReverter movementReverter;
    protected List<BaseWeapon> weapons;

    // Variables
    protected Tile tile;
    protected new string name;
    protected int health;
    protected int healthMax;
    protected int movement;
    [SerializeField] protected TileObjectType objectType;

    // Abilities
    protected bool hasDoneAction;
    protected bool hasMoved; // 1 Move per turn / or can't move after attacking
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

        hasDoneAction = false;
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

        weapons = new List<BaseWeapon>();
        foreach(var availableAttack in characterSO.availableAttacks)
        {
            weapons.Add(BaseWeapon.CreateWeapon(availableAttack));
        }
    }

    private void Start()
    {
        GameManager.Instance.OnPlayerFinishTurn += GameManager_OnPlayerFinishTurn;
    }

    private void GameManager_OnPlayerFinishTurn(object sender, System.EventArgs e)
    {
        hasMoved = false;
        hasDoneAction = false;
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

    #region GET PROPERTIES
    public bool IsFlying() => isFlying;
    public bool IsPushable() => pushable;
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

    public int GetCurrentHealth() => health;
    public int GetMaxHealth() => healthMax;

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
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
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
        }
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task GetPushed(Direction direction)
    {
        if (!pushable) return;

        if(TileManager.Instance.TryGetNeighborTile(tile, direction, out Tile newTile))
        {
            if(newTile.TryGetTileObject(out TileObject newTileObject))
            {
                await TileManager.Instance.MoveTileObjecyByOneAndBackOnPath(tile, direction);
                newTileObject.GetDamagedFromPush();
                this.GetDamagedFromPush();
            }
            else
            {
                await TileManager.Instance.MoveTileObjectByOneOnPath(tile, direction);
            }
        }
    }

    public void GetDestroyed()
    {
        if (explosive)
        {
            TileObject newTileObject;
            int explosionDamage = 1;
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.North, out newTileObject))
            {
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.South, out newTileObject))
            {
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.West, out newTileObject))
            {
                newTileObject.GetDamaged(explosionDamage);
            }
            if (TileManager.Instance.TryGetNeighborTileObject(tile, Direction.East, out newTileObject))
            {
                newTileObject.GetDamaged(explosionDamage);
            }
        }
        if (IsEnemyType())
        {
            GameManager.Instance.UnregisterEnemy(this);
        }
        tile.RemoveTileObject();
        Destroy(gameObject);
    }


    protected void GetHealed(int amount)
    {
        health += amount;
        if(health > healthMax)
            health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region enum ObjectType
    // Enum Object Type
    public enum TileObjectType
    {
        Player,
        Enemy,
        Building,
        Terrain
    }

    public bool IsPlayerType() => objectType == TileObjectType.Player;
    public bool IsEnemyType() => objectType == TileObjectType.Enemy;
    public bool IsBuildingType() => objectType == TileObjectType.Building;
    public bool IsTerrainType() => objectType == TileObjectType.Terrain;

    public TileObjectType GetObjectType() => objectType;
    public bool HasSameType(TileObject otherTileObject) => objectType == otherTileObject.objectType;
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

    private void ClearPreviousPosition()
    {
        movementReverter.previousPosition = null;
        movementReverter.canRevert = false;
    }

    #endregion

    #region HasDoneAction
    public bool GetHasDoneAction() => hasDoneAction;
    #endregion

    #region Repair Action
    public void RepairSelf()
    {
        GetHealed(1);
        // TODO Also clears bad effects like being on fire
        hasMoved = true;
        hasDoneAction = true;
    }
    #endregion

    #region Weapons
    // First Weapon
    public bool HasFirstWeapon()
    {
        return weapons.Count >= 1;
    }
    public List<Tile> GetFirstWeaponPossiblePlaces()
    {
        return weapons[0].GetPossibleAttackPlaces(GetTile());
    }
    public bool IsTileInRangeFirstWeapon(Tile tile)
    {
        return weapons[0].IsTileInRange(GetTile(), tile);
    }
    public void AttackTileFirstWeapon(Tile tile)
    {
        weapons[0].AttackTile(GetTile(), tile);
        hasMoved = true;
        hasDoneAction = true;
    }
    public bool IsFirstWeaponPasive()
    {
        return weapons[0].isPasive;
    }

    public BaseWeapon GetFirstWeapon()
    {
        return weapons[0];
    }
    // Second Weapon
    public bool HasSecondWeapon()
    {
        return weapons.Count >= 2;
    }
    public List<Tile> GetSecondWeaponPossiblePlaces()
    {
        return weapons[1].GetPossibleAttackPlaces(GetTile());
    }
    public bool IsTileInRangeSecondWeapon(Tile tile)
    {
        return weapons[1].IsTileInRange(GetTile(), tile);
    }
    public void AttackTileSecondWeapon(Tile tile)
    {
        weapons[1].AttackTile(GetTile(), tile);
        hasMoved = true;
        hasDoneAction = true;
    }
    public bool IsSecondWeaponPasive()
    {
        return weapons[0].isPasive;
    }
    public BaseWeapon GetSecondWeapon()
    {
        return weapons[1];
    }

    public List<BaseWeapon> GetWeapons()
    {
        return weapons;
    }
    #endregion

    #region animation move

    public async Task MoveToPositionAnimated(PathFinder.PathNode newPosition, float speed=5f)
    {
        List<PathFinder.PathNode> pathNodes = new List<PathFinder.PathNode>();
        PathFinder.PathNode current = newPosition;
        while(current != null)
        {
            pathNodes.Add(current);
            current = current.previousNode;
        }
        for(int i = pathNodes.Count - 1; i >= 0; i--)
        {
            await SlideToPositionAnimated(TileManager.Instance.GetTile(pathNodes[i].XY), speed);
        }
    }

    public async Task SlideToPositionAnimated(Tile to, float slideSpeed = 5f)
    {
        Vector3 dirVec3 = to.transform.position - transform.position;
        while(transform.position != to.transform.position)
        {
            Vector3 vec3Amount = dirVec3 * slideSpeed * Time.deltaTime;
            if(vec3Amount.magnitude >= (to.transform.position - transform.position).magnitude)
            {
                transform.position = to.transform.position;
            }
            else
            {
                transform.position += vec3Amount;
            }
            await Task.Yield();
        }
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
