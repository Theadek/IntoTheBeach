using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderVisual : MonoBehaviour
{
    [SerializeField] private GameObject highlightTilePrefab;
    [SerializeField] private GameObject highlightAttackTilePrefab;


    private Grid grid;

    private void Start()
    {
        grid = TileManager.Instance.GetGrid();
        PathFinder.Instance.OnPossibleMovesChanged += PathFinding_OnPossibleMovesChanged;
        GameManager.Instance.OnSelectedTypeChanged += GameManager_OnSelectedTypeChanged;

    }

    private void GameManager_OnSelectedTypeChanged(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.TryGetSelectedObject(out TileObject tileObject)) return;

        List<Tile> tile_list;
        if (GameManager.Instance.IsSelectedFirstWeapon())
        {
            tile_list = tileObject.GetFirstWeaponPossiblePlaces();
        }
        else if (GameManager.Instance.IsSelectedSecondWeapon())
        {
            tile_list = tileObject.GetSecondWeaponPossiblePlaces();
        }
        else
        {
            return;
        }

        DestroyCurrentHighlight();
        foreach (Tile tile in tile_list)
        {
            var position = TileManager.Instance.GetGridPosition(tile.GetXY());
            Instantiate(highlightAttackTilePrefab, position, Quaternion.identity, transform);
        }


    }

    private void PathFinding_OnPossibleMovesChanged(object sender, System.EventArgs e)
    {
        DestroyCurrentHighlight();
        if (!GameManager.Instance.TryGetSelectedObject(out TileObject tileObject)) return;

        if (GameManager.Instance.IsSelectedMovement())
        {
            CreateNewMovementHighlight();
        }
    }

    private void DestroyCurrentHighlight()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateNewMovementHighlight()
    {
        List<Vector2Int> toHighlight = PathFinder.Instance.GetPossibleMovesVector2();
        foreach(Vector2Int coord in toHighlight)
        {
            var position = grid.GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));
            Instantiate(highlightTilePrefab, position, Quaternion.identity, transform);
        }
    }

}
