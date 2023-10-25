using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Valley : BaseWeapon
{
    GameObject AttackHighlightPrefabN;
    GameObject AttackHighlightPrefabS;
    GameObject AttackHighlightPrefabE;
    GameObject AttackHighlightPrefabW;
    GameObject DotPrefab;
    public Valley()
    {
        AttackDescription = "Shoot a valley missle that deals 1 DMG";
        AttackHighlightPrefabN = GameAssets.i.pushN;
        AttackHighlightPrefabS = GameAssets.i.pushS;
        AttackHighlightPrefabE = GameAssets.i.pushE;
        AttackHighlightPrefabW = GameAssets.i.pushW;
        DotPrefab = GameAssets.i.Dot;

        isPasive = false;
        upgrades = new List<Upgrade>
        {
            new Upgrade(false, 1, "No DMG to buildings"),
            new Upgrade(false, 3, "Deal additional 2 DMG")
        };
    }

    public override async Task AttackTile(Tile from, Tile tile)
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
            List<Task> tasks = new List<Task>();
            for(Direction dir = Direction.North; dir != Direction.COUNT; dir++)
            {
                if(TileManager.Instance.TryGetNeighborTileObject(tile, dir, out TileObject neighbor))
                {
                    tasks.Add(neighbor.GetPushed(dir));
                }
            }
            await Task.WhenAll(tasks);
            TileManager.Instance.CheckHealthToDestroyed();
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
            for(Direction dir = Direction.North; dir < Direction.COUNT; dir++)
            {
                if(TileManager.Instance.TryGetNeighborTile(tile, dir, out _))
                {
                    var position = TileManager.Instance.GetGridPosition(tile.GetXY());
                    GameManager.Instance.MyInstantiate(GetHighlightPrefabFromDirection(dir), position, Quaternion.identity, parent);
                }
            }

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

    private GameObject GetHighlightPrefabFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return AttackHighlightPrefabN;
            case Direction.South:
                return AttackHighlightPrefabS;
            case Direction.East:
                return AttackHighlightPrefabE;
            case Direction.West:
                return AttackHighlightPrefabW;
            default:
                return null;
        }
    }

    public override int EnemyCalculateMovementScore(Tile tile)
    {
        return 0;
    }

    public override int EnemyCalculateAttackScore(Tile tile)
    {
        return 0;
    }

    public override Tile EnemyRecalculateAttackPlace(Tile fromTile, Tile toTile, Tile previousAttackTile)
    {
        return null;
    }
}
