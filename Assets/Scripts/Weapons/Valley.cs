using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valley : BaseWeapon
{
    GameObject AttackHighlightPrefab;
    GameObject DotPrefab;
    public Valley()
    {
        AttackDescription = "Shoot a valley missle that deals 1 DMG";
        AttackHighlightPrefab = GameAssets.i.PushHighlightAround;
        DotPrefab = GameAssets.i.Dot;

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
            if (tile.TryGetTileObject(out TileObject tileObject))
            {
                int damageAmount = 1;
                if (upgrades[1].enabled)
                    damageAmount += 2;

                // TODO no damage to buildings
                if (upgrades[0].enabled)
                {
                    // if(tile.IsBuilding())
                    //    damageAmount = 0;
                }

                tileObject.GetDamaged(damageAmount);
            }
            for(Direction dir = Direction.North; dir != Direction.COUNT; dir++)
            {
                if(TileManager.Instance.TryGetNeighborTileObject(tile, dir, out TileObject neighbor))
                {
                    neighbor.GetPushed(dir);
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

    public override void DisplayEffectOnTile(Tile from, Tile tile, Transform parent)
    {
        if (Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out int distance))
        {
            // Dots

            float riseAmount = 0.3f;
            float riseOffset = 0.3f;
            Vector3 startPoint = from.transform.position;
            Vector3 stepDirection = tile.transform.position - startPoint;

            int amountOfDots = 7;
            float stepAmount = stepDirection.magnitude / (amountOfDots + 2);
            stepDirection = stepDirection.normalized;
            stepDirection *= stepAmount;
            for(int i = 1; i<=amountOfDots; i++)
            {
                var dotPosition = startPoint + stepDirection * i;
                if(i <= amountOfDots / 2)
                {
                    dotPosition += Vector3.up * (riseAmount * i);
                }
                else
                {
                    dotPosition += Vector3.up * (riseAmount * (amountOfDots - i));
                }
                dotPosition += Vector3.up * riseOffset;
                GameManager.Instance.MyInstantiate(DotPrefab, dotPosition, Quaternion.identity, parent);
            }

            // Push arrows
            var position = TileManager.Instance.GetGridPosition(tile.GetXY());
            GameManager.Instance.MyInstantiate(AttackHighlightPrefab, position, Quaternion.identity, parent);
        }
        else
        {
            Debug.LogWarning($"ShowHighlights should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
        }
    }

    public override bool IsTileInRange(Tile from, Tile tile)
    {
        return GetPossibleAttackPlaces(from).Contains(tile);
    }
}
