using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [SerializeField] private CharacterSO characterSO;

    private void Awake()
    {
        name = characterSO.name;
        health = characterSO.health;
        movement = characterSO.movement;
    }

    protected Tile tile;
    protected new string name;
    protected int health;
    protected int movement;
    [SerializeField] protected ObjectType objectType;

    protected void SetParentTile(Tile tile)
    {
        this.tile = tile;
    }

    public CharacterSO GetCharacterSO()
    {
        return characterSO;
    }

    protected void SetCharacterSO(CharacterSO characterSO)
    {
        this.characterSO = characterSO;
    }

    protected void GetDamaged(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            GetDestroyed();
        }
    }

    protected void GetDestroyed()
    {

    }

    // Enum Object Type
    protected enum ObjectType
    {
        Player,
        Enemy,
        Neutral,
    }

    public bool IsPlayerType()
    {
        return objectType == ObjectType.Player;
    }


    // Static TileObject Creation
    public static TileObject CreateTileObject(Tile parent, CharacterSO characterSO)
    {
        var newObject = Instantiate(characterSO.prefab, parent.transform.position, Quaternion.identity, parent.transform);
        var newTileObject = newObject.GetComponent<TileObject>();
        newTileObject.SetParentTile(parent);
        newTileObject.SetCharacterSO(characterSO);
        return newTileObject;
    }
}
