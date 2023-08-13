using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistPunch : BaseWeapon
{
    public FistPunch()
    {
        AttackDescription = "Deal 1 DMG and push enemy 1 tile back";
        AttackHighlightPrefabN = GameAssets.i.fistPunchHighlightN;
        AttackHighlightPrefabS = GameAssets.i.fistPunchHighlightS;
        AttackHighlightPrefabE = GameAssets.i.fistPunchHighlightE;
        AttackHighlightPrefabW = GameAssets.i.fistPunchHighlightW;
        upgrades = new List<Upgrade>();
        upgrades.Add(new Upgrade(false, 1, "Deal additional DMG"));
        upgrades.Add(new Upgrade(false, 2, "Dash To Target"));
    }

    public override void AttackTile(Tile from, Tile tile)
    {
        if(!IsTileInRange(from, tile)) return;

        if(Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out int distance))
        {
            if(distance > 1)
            {
                // TODO Move
            }
            if(tile.TryGetTileObject(out TileObject obj))
            {
                int damageAmount = 1;
                if (upgrades[0].enabled)
                    damageAmount++;

                obj.GetDamaged(damageAmount);
                obj.GetPushed(direction);
            }
        }
        else
        {
            Debug.LogWarning($"FistPunch Attack should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
        }

    }

    public override List<Tile> GetPossibleAttackPlaces(Tile from)
    {
        List<Tile> possibleAttackPlaces = new List<Tile>();
        if (upgrades[1].enabled)
        {
            //Dash to Enemy
            for (Direction direction = Direction.North; direction < Direction.COUNT; direction++)
            {
                Tile current = from;
                while (true)
                {
                    if (TileManager.Instance.TryGetNeighborTile(current, direction, out Tile neighbor))
                    {
                        possibleAttackPlaces.Add(neighbor);
                        if (neighbor.HasObjectOnThisTile())
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            //Just neighbors
            for (Direction direction = Direction.North; direction < Direction.COUNT; direction++)
            {
                if (TileManager.Instance.TryGetNeighborTile(from, direction, out Tile neighbor))
                {
                    possibleAttackPlaces.Add(neighbor);
                }
            }
        }
        return possibleAttackPlaces;
    }

    public override bool IsTileInRange(Tile from, Tile tile)
    {
        return GetPossibleAttackPlaces(from).Contains(tile);
    }

    //public override void GetHighlights(Tile from, Tile tile, Transform parent)
    //{
    //    if(Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out int distance))
    //    {
    //        GameObject AttackHighlightPrefab;
    //        //AtackHighlightPrefab
    //        switch (direction)
    //        {
    //            case Direction.North:
    //                AttackHighlightPrefab = AttackHighlightPrefabN;
    //                break;
    //            case Direction.South:
    //                AttackHighlightPrefab = AttackHighlightPrefabS;
    //                break;
    //            case Direction.East:
    //                AttackHighlightPrefab = AttackHighlightPrefabE;
    //                break;
    //            case Direction.West:
    //                AttackHighlightPrefab = AttackHighlightPrefabW;
    //                break;
    //            default:
    //                Debug.LogWarning($"ShowHighlights should have happened, but couldn't get direction. {from.GetXY()} => {tile.GetXY()}");
    //                return;
    //        }

    //        var position = TileManager.Instance.GetGridPosition(tile.GetXY());
    //        Instantiate(AttackHighlightPrefab, position, Quaternion.identity, parent);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"ShowHighlights should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
    //    }
    //}
}
