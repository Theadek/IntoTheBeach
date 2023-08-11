using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderVisual : MonoBehaviour
{
    [SerializeField] private GameObject highlightTilePrefab;


    private Grid grid;

    private void Start()
    {
        grid = TileManager.Instance.GetGrid();
        PathFinder.Instance.OnPossibleMovesChanged += PathFinding_OnPossibleMovesChanged;

    }


    private void PathFinding_OnPossibleMovesChanged(object sender, System.EventArgs e)
    {
        DestroyCurrentHighlight();
        if(GameManager.Instance.HasSelectedObject())
            CreateNewHighlight();
    }

    private void DestroyCurrentHighlight()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateNewHighlight()
    {
        List<Vector2Int> toHighlight = PathFinder.Instance.GetPossibleMovesVector2();
        foreach(Vector2Int coord in toHighlight)
        {
            var position = grid.GetCellCenterWorld(new Vector3Int(coord.x, coord.y, 0));
            Instantiate(highlightTilePrefab, position, Quaternion.identity, transform);
        }
    }

}
