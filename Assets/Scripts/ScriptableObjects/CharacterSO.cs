using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterSO : ScriptableObject
{
    public new string name;
    public int health;
    public int movement;
    public bool isFlying;
    public GameObject prefab;
    public AvailableAttacks[] availableAttacks;

    public enum AvailableAttacks
    {
        Punch,
        Shoot,
        BossAttack
    }

}
