using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseWeapon
{
    // Attack
    public string AttackDescription;
    public abstract List<Tile> GetPossibleAttackPlaces(Tile from);
    public abstract bool IsTileInRange(Tile from, Tile tile);
    public abstract Task AttackTile(Tile from, Tile tile);
    public abstract void DisplayEffectOnTile(Tile from, Tile tile, Transform parent);
    public abstract int EnemyCalculateMovementScore(Tile tile);
    public abstract int EnemyCalculateAttackScore(Tile tile);
    public abstract Tile EnemyRecalculateAttackPlace(Tile fromTile, Tile toTile, Tile previousAttackTile);

    public bool isPasive;
    // Upgrades
    public List<Upgrade> upgrades;
    public struct Upgrade
    {
        public bool enabled;
        public int cost;
        public string description;
        public Upgrade(bool enabled, int cost, string description)
        {
            this.enabled = enabled;
            this.cost = cost;
            this.description = description;
        }
    }

    public enum Weapon
    {
        FistPunch,
        DirectShot,
        Volleys
    }

    public static BaseWeapon CreateWeapon(Weapon weapon)
    {
        switch (weapon)
        {
            case Weapon.FistPunch:
                return new FistPunch();
            case Weapon.DirectShot:
                return new DirectShot();
            case Weapon.Volleys:
                return new Valley();
            default:
                return null;
        }
    }
}
