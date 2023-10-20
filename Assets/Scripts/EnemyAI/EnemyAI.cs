using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public event EventHandler OnEnemyAttackChanged;

    private TileObject tileObject;
    private List<BaseWeapon> weapons;

    private BaseWeapon weaponToUse;
    private Tile tileToAttack;
    private Tile tileFromToAttack;

    private void Awake()
    {
        tileObject = GetComponent<TileObject>();
        weapons = tileObject.GetWeapons();
    }

    public PathFinder.PathNode CalculateMovement()
    {
        List<PathFinder.PathNode> listOfPossibleMoves = PathFinder.Instance.GetListOfPossibleMoves(tileObject);
        List<TileScore> tileScores = new List<TileScore>();
        foreach (PathFinder.PathNode pathNode in listOfPossibleMoves)
        {
            int bestScoreBetweenWeapons = -999;
            Tile tile = TileManager.Instance.GetTile(pathNode.XY);
            foreach (BaseWeapon weapon in weapons)
            {
                int score = weapon.EnemyCalculateMovementScore(tile);
                if(score > bestScoreBetweenWeapons)
                {
                    bestScoreBetweenWeapons = score;
                }
            }
            tileScores.Add(new TileScore { pathNode = pathNode, score = bestScoreBetweenWeapons });
        }
        tileScores.Sort();

        //Get first or second best
        if(tileScores.Count == 1)
        {
            return tileScores[0].pathNode;
        }
        else
        {
            int index = UnityEngine.Random.Range(0, 2);
            return tileScores[index].pathNode;
        }
    }

    public void CalculateAttack()
    {
        int bestScoreBetweenWeapons = -999;
        foreach (BaseWeapon weapon in weapons)
        {
            List<Tile> possibleAttackPlaces = weapon.GetPossibleAttackPlaces(tileObject.GetTile());
            foreach(Tile tile in possibleAttackPlaces)
            {
                int score = weapon.EnemyCalculateAttackScore(tile);
                if(score > bestScoreBetweenWeapons)
                {
                    bestScoreBetweenWeapons = score;
                    weaponToUse = weapon;
                    tileToAttack = tile;
                }
            }
        }
        tileFromToAttack = tileObject.GetTile();
        OnEnemyAttackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RecalculateAttack()
    {
        if(tileToAttack == null || weaponToUse == null) return;

        tileToAttack = weaponToUse.EnemyRecalculateAttackPlace(tileFromToAttack, tileObject.GetTile(), tileToAttack);
        if(tileToAttack == null)
        {
            weaponToUse = null;
        }
        tileFromToAttack = tileObject.GetTile();
        OnEnemyAttackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Attack()
    {
        if(weaponToUse != null && tileToAttack != null)
        {
            weaponToUse.AttackTile(tileObject.GetTile(), tileToAttack);
            weaponToUse = null;
            tileToAttack = null;
            tileFromToAttack = null;
            OnEnemyAttackChanged?.Invoke(this, EventArgs.Empty);
            TileManager.Instance.CheckHealthToDestroyed();
        }
    }

    public bool IsAbleToAttack()
    {
        if (weaponToUse != null && tileToAttack != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private class TileScore : IComparable
    {
        public PathFinder.PathNode pathNode;
        public int score;

        int IComparable.CompareTo(object obj)
        {
            TileScore tileScore = obj as TileScore;
            if(score < tileScore.score)
            {
                return 1;
            }
            else if(score == tileScore.score)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    public BaseWeapon GetWeapon() => weaponToUse;
    public Tile GetTile() => tileToAttack;
    public TileObject GetTileObject() => tileObject;

}