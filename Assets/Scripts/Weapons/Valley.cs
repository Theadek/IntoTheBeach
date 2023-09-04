using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valley : BaseWeapon
{
    public Valley()
    {
        AttackDescription = "Shoot a valley missle that deals 1 DMG";
        AttackHighlightPrefabN = GameAssets.i.fistPunchHighlightN;
        AttackHighlightPrefabS = GameAssets.i.fistPunchHighlightS;
        AttackHighlightPrefabE = GameAssets.i.fistPunchHighlightE;
        AttackHighlightPrefabW = GameAssets.i.fistPunchHighlightW;
        isPasive = false;
        upgrades = new List<Upgrade>
        {
            new Upgrade(false, 1, "No DMG to buildings"),
            new Upgrade(false, 3, "Deal additional 2 DMG")
        };
    }

    public override void AttackTile(Tile from, Tile tile)
    {
        if (!IsTileInRange(from, tile)) return;

        if (Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out _))
        {
            if (TileManager.Instance.TryGetFirstTileObjectInDirection(from, direction, out TileObject tileObject))
            {
                int damageAmount = 1;
                if (upgrades[1].enabled)
                    damageAmount += 2;

                // TODO no damage to buildings
                if (upgrades[0].enabled)
                {
                    // if(tile.IsBuilding())
                        damageAmount = 0;
                }

                tileObject.GetDamaged(damageAmount);
                for(Direction dir = Direction.North; dir != Direction.COUNT; dir++)
                {
                    if(TileManager.Instance.TryGetNeighborTileObject(tile, dir, out TileObject neighbor))
                    {
                        neighbor.GetPushed(dir);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"Valley Attack should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
        }

    }

    public override List<Tile> GetPossibleAttackPlaces(Tile from)
    {
        List<Tile> possibleAttackPlaces = new List<Tile>();

        //Dash to Enemy
        for (Direction direction = Direction.North; direction < Direction.COUNT; direction++)
        {
            Tile current = from;
            // Skip 1 tile
            if (TileManager.Instance.TryGetNeighborTile(current, direction, out Tile neighbor))
            {
                current = neighbor;
            }
            else
            {
                continue;
            }
            while (true)
            {
                if (TileManager.Instance.TryGetNeighborTile(current, direction, out neighbor))
                {
                    possibleAttackPlaces.Add(neighbor);
                    current = neighbor;
                }
                else
                {
                    break;
                }
            }
        }

        return possibleAttackPlaces;
    }

    public override bool IsTileInRange(Tile from, Tile tile)
    {
        return GetPossibleAttackPlaces(from).Contains(tile);
    }
}
