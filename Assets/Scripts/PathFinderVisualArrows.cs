using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PathFinderVisualArrows : MonoBehaviour
{
    [SerializeField] private GameObject northArrowPrefab;
    [SerializeField] private GameObject sountArrowPrefab;
    [SerializeField] private GameObject eastArrowPrefab;
    [SerializeField] private GameObject westArrowPrefab;

    private Grid grid;

    private void Start()
    {
        grid = TileManager.Instance.GetGrid();
        Tile.OnAnyPointerEnterEvent += Tile_OnAnyPointerEnterEvent;
        GameManager.Instance.OnSelectedObjectChanged += GameManager_OnSelectedObject;
    }

    private void GameManager_OnSelectedObject(object sender, System.EventArgs e)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void Tile_OnAnyPointerEnterEvent(object sender, Tile.OnAnyPointerEnterEventArgs e)
    {
        if (GameManager.Instance.IsSelectedMovement())
        {
            // Clear previous Arrows
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            //Get Direction
            var currentPathNode = PathFinder.Instance.GetPathNodeForXY(e._xy);
            if (currentPathNode == null)
            {
                return;
            }
            while (currentPathNode.previousNode != null)
            {
                if (Helpers.TryGetDirection(currentPathNode.previousNode.XY, currentPathNode.XY, out Direction direction))
                {
                    GameObject arrowPrefabToCreate = null;
                    switch (direction)
                    {
                        case Direction.North:
                            arrowPrefabToCreate = northArrowPrefab;
                            break;
                        case Direction.South:
                            arrowPrefabToCreate = sountArrowPrefab;
                            break;
                        case Direction.East:
                            arrowPrefabToCreate = eastArrowPrefab;
                            break;
                        case Direction.West:
                            arrowPrefabToCreate = westArrowPrefab;
                            break;
                    }
                    //Create Arrow
                    var position = grid.GetCellCenterWorld(new Vector3Int(currentPathNode.XY.x, currentPathNode.XY.y, 0));
                    Instantiate(arrowPrefabToCreate, position, Quaternion.identity, transform);
                }
                //Change currentPathNode
                currentPathNode = currentPathNode.previousNode;
            }
        }
        else if(GameManager.Instance.IsSelectedFirstWeapon())
        {
            // Clear previous Arrows
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            GameManager.Instance.TryGetSelectedObject(out TileObject tileObject);
            if (!tileObject.IsTileInRangeFirstWeapon(TileManager.Instance.GetTile(e._xy)))
            {
                return;
            }

            var currentTilePosition = TileManager.Instance.GetGridPosition(e._xy);
            Helpers.TryGetDirectionLong(tileObject.GetTile().GetXY(), e._xy, out Direction direction, out _);
            GameObject prefab;
            switch(direction)
            {
                case Direction.North:
                    prefab = tileObject.GetFirstWeapon().AttackHighlightPrefabN;
                    break;
                case Direction.South:
                    prefab = tileObject.GetFirstWeapon().AttackHighlightPrefabS;
                    break;
                case Direction.West:
                    prefab = tileObject.GetFirstWeapon().AttackHighlightPrefabW;
                    break;
                case Direction.East:
                    prefab = tileObject.GetFirstWeapon().AttackHighlightPrefabE;
                    break;
                default: return;
            }
            Instantiate(prefab, currentTilePosition, Quaternion.identity, transform);
        }
        else if (GameManager.Instance.IsSelectedSecondWeapon())
        {
            // Clear previous Arrows
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            GameManager.Instance.TryGetSelectedObject(out TileObject tileObject);
            if (!tileObject.IsTileInRangeSecondWeapon(TileManager.Instance.GetTile(e._xy)))
            {
                return;
            }

            var currentTilePosition = TileManager.Instance.GetGridPosition(e._xy);
            Helpers.TryGetDirectionLong(tileObject.GetTile().GetXY(), e._xy, out Direction direction, out _);
            GameObject prefab;
            switch (direction)
            {
                case Direction.North:
                    prefab = tileObject.GetSecondWeapon().AttackHighlightPrefabN;
                    break;
                case Direction.South:
                    prefab = tileObject.GetSecondWeapon().AttackHighlightPrefabS;
                    break;
                case Direction.West:
                    prefab = tileObject.GetSecondWeapon().AttackHighlightPrefabW;
                    break;
                case Direction.East:
                    prefab = tileObject.GetSecondWeapon().AttackHighlightPrefabE;
                    break;
                default: return;
            }
            Instantiate(prefab, currentTilePosition, Quaternion.identity, transform);
        }

    }
}
