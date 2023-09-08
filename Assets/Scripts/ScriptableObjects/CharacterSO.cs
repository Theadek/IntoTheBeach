using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSO : ScriptableObject
{
    public new string name;
    public int health;
    public int movement;
    public GameObject prefab;
    public BaseWeapon.Weapon[] availableAttacks;

    public bool movable;
    public bool pushable;
    public bool hasArmor;
    public bool hasShield;
    public bool explosive;
    public bool isFlying;

}
