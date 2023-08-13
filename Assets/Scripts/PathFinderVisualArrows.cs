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
        // Clear previous Arrows
        foreach(Transform child in transform)
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
}
