using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectShot : BaseWeapon
{
    GameObject DotPrefab;
    public DirectShot()
    {
        AttackDescription = "Shoot a bullet that deals 1 DMG";
        DotPrefab = GameAssets.i.Dot;

        isPasive = false;
        upgrades = new List<Upgrade>
        {
            new Upgrade(false, 2, "Deal additional DMG"),
            new Upgrade(false, 3, "Deal additional DMG")
        };
    }

    public override void AttackTile(Tile from, Tile tile)
    {
        if (!IsTileInRange(from, tile)) return;

        if (Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out _))
        {
            if(TileManager.Instance.TryGetFirstTileObjectInDirection(from, direction, out TileObject tileObject))
            {
                //Shoot First Object
                int damageAmount = 1;
                if (upgrades[0].enabled)
                    damageAmount++;
                if (upgrades[1].enabled)
                    damageAmount++;

                tileObject.GetDamaged(damageAmount);
            }
            else
            {
                // Shoot till the end of the board

                // Shoot animation
                // Fire on trees tile
            }
        }
        else
        {
            Debug.LogWarning($"DirectShot Attack should have happened, but something failed. {from.GetXY()} => {tile.GetXY()}");
        }

    }

    public override List<Tile> GetPossibleAttackPlaces(Tile from)
    {
        List<Tile> possibleAttackPlaces = new List<Tile>();

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

        return possibleAttackPlaces;
    }

    public override void DisplayEffectOnTile(Tile from, Tile tile, Transform parent)
    {
        if (Helpers.TryGetDirectionLong(from.GetXY(), tile.GetXY(), out Direction direction, out _))
        {
            // Dots
            int amountOfDots;
            Vector3 lastPlaceLocation;
            if (TileManager.Instance.TryGetFirstTileObjectInDirection(from, direction, out TileObject tileObject))
            {
                lastPlaceLocation = tileObject.GetTile().transform.position;
                Helpers.TryGetDirectionLong(from.GetXY(), tileObject.GetTile().GetXY(), out _, out amountOfDots);
            }
            else
            {
                Tile lastTile = TileManager.Instance.GetLastTileInDirection(from, direction);
                lastPlaceLocation = lastTile.transform.position;
                //lastPlaceLocation += TileManager.Instance.GetVector3BetweenTiles(direction);
                Helpers.TryGetDirectionLong(from.GetXY(), lastTile.GetXY(), out _, out amountOfDots);
                amountOfDots++;
            }
            amountOfDots += 2;

            Vector3 startPoint = from.transform.position;
            Vector3 stepDirection = lastPlaceLocation - startPoint;

            float stepAmount = stepDirection.magnitude / amountOfDots;
            stepDirection = stepDirection.normalized;
            stepDirection *= stepAmount;
            for (int i = 1; i <= amountOfDots; i++)
            {
                var dotPosition = startPoint + stepDirection * i;

                GameManager.Instance.MyInstantiate(DotPrefab, dotPosition, Quaternion.identity, parent);
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
}
