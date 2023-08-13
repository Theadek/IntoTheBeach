using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon
{
    // Attack
    public string AttackDescription;
    public abstract List<Tile> GetPossibleAttackPlaces(Tile from);
    public abstract bool IsTileInRange(Tile from, Tile tile);
    public abstract void AttackTile(Tile from, Tile tile);
    public GameObject AttackHighlightPrefabN;
    public GameObject AttackHighlightPrefabS;
    public GameObject AttackHighlightPrefabE;
    public GameObject AttackHighlightPrefabW;
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
            default:
                return null;
        }
    }
}
