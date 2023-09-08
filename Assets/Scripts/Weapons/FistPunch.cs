using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FistPunch : BaseWeapon
{
    GameObject AttackHighlightPrefab;
    GameObject AttackHighlightPrefabN;
    GameObject AttackHighlightPrefabS;
    GameObject AttackHighlightPrefabE;
    GameObject AttackHighlightPrefabW;
    public FistPunch()
    {
        AttackDescription = "Deal 1 DMG and push enemy 1 tile back";
        AttackHighlightPrefab = GameAssets.i.tileHighlight;
        AttackHighlightPrefabN = GameAssets.i.pushN;
        AttackHighlightPrefabS = GameAssets.i.pushS;
        AttackHighlightPrefabE = GameAssets.i.pushE;
        AttackHighlightPrefabW = GameAssets.i.pushW;
        isPasive = false;
        upgrades = new List<Upgrade>
        {
            new Upgrade(false, 3, "Deal additional 2 DMG"),
            new Upgrade(false, 2, "Dash To Target")
        };
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
                    damageAmount+=2;

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
                        current = neighbor;
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

    public override void DisplayEffectOnTile(Tile from, Tile tile, Transform parent)
    {
        if (Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out int distance))
        {
            GameObject AttackHighlightPrefabDir = null;
            //AtackHighlightPrefab
            switch (direction)
            {
                case Direction.North:
                    AttackHighlightPrefabDir = AttackHighlightPrefabN;
                    break;
                case Direction.South:
                    AttackHighlightPrefabDir = AttackHighlightPrefabS;
                    break;
                case Direction.East:
                    AttackHighlightPrefabDir = AttackHighlightPrefabE;
                    break;
                case Direction.West:
                    AttackHighlightPrefabDir = AttackHighlightPrefabW;
                    break;
                default:
                    Debug.LogWarning($"ShowHighlights should have happened, but couldn't get direction. {from.GetXY()} => {tile.GetXY()}");
                    return;
            }
            var position = TileManager.Instance.GetGridPosition(tile.GetXY());
            GameManager.Instance.MyInstantiate(AttackHighlightPrefab, position, Quaternion.identity, parent);

            if (TileManager.Instance.TryGetNeighborTile(tile, direction, out _))
            {
                GameObject go = GameManager.Instance.MyInstantiate(AttackHighlightPrefabDir, position, Quaternion.identity, parent);
                if (tile.TryGetTileObject(out TileObject neighborTileObject))
                {
                    if (!neighborTileObject.IsPushable())
                    {
                        Color color = go.GetComponentInChildren<SpriteRenderer>().color;
                        color.a = .2f;
                        go.GetComponentInChildren<SpriteRenderer>().color = color;
                    }
                }
                else
                {
                    Color color = go.GetComponentInChildren<SpriteRenderer>().color;
                    color.a = .2f;
                    go.GetComponentInChildren<SpriteRenderer>().color = color;
                }
            }
        }
        else
        {
            Debug.LogWarning($"ShowHighlights should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
        }
    }

    public override int EnemyCalculateMovementScore(Tile tile)
    {
        int score = 0;
        for (Direction direction = Direction.North; direction < Direction.COUNT; direction++)
        {
            TileObject target;
            if (upgrades[1].enabled)
            {
                if(!TileManager.Instance.TryGetFirstTileObjectInDirection(tile, direction, out target))
                {
                    continue;
                }

            }
            else
            {
                if (!TileManager.Instance.TryGetNeighborTileObject(tile, direction, out target))
                {
                    continue;
                }
            }
            if (target.IsPlayerType() || target.IsBuildingType()) score += 10;
            if (target.IsTerrainType()) score += 1;
            // TODO if tile is attacked by other Enemy then -20???
        }
        return score;
    }

    public override int EnemyCalculateAttackScore(Tile tile)
    {
        if(tile.TryGetTileObject(out TileObject target))
        {
            if (target.IsEnemyType()) return -9999;
            if (target.IsPlayerType() || target.IsBuildingType()) return 10;
            if (target.IsTerrainType()) return 1;
        }
        return -9999;
    }

    public override Tile EnemyRecalculateAttackPlace(Tile fromTile, Tile toTile, Tile previousAttackTile)
    {
        //Get AttackDirection
        Helpers.TryGetDirectionLong(fromTile.GetXY(), previousAttackTile.GetXY(), out Direction direction, out _);

        if (upgrades[1].enabled)
        {
            //Dash to Enemy
            if (TileManager.Instance.TryGetFirstTileObjectInDirection(toTile, direction, out TileObject target))
            {
                return target.GetTile();
            }
            else
            {
                return TileManager.Instance.GetLastTileInDirection(toTile, direction);
            }
        }
        else
        {
            if (fromTile == toTile) return previousAttackTile;
            if (TileManager.Instance.TryGetNeighborTile(toTile, direction, out Tile neighborTile))
            {
                return neighborTile;
            }
            else
            {
                return null;
            }
        }


    }
}
