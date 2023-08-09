using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject TilePrefab;

    private Vector2Int TileCount = new Vector2Int(8, 8);
    private Grid grid;
    private GameObject[,] tiles;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        tiles = new GameObject[TileCount[0], TileCount[1]];
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Tile.ToggleShowCoordinates();
        }
    }

    private void RegenerateGrid()
    {
        DestroyGrid();
        GenerateGrid();
    }

    private void DestroyGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void GenerateGrid()
    {
        for(int y = 0; y < TileCount[1]; y++)
        {
            for (int x = 0; x < TileCount[0]; x++)
            {
                var position = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                var tile = Instantiate(TilePrefab, position, TilePrefab.transform.rotation, transform);
                tile.GetComponent<Tile>().SetXY(x, y);
                tiles[x, y] = tile;
            }
        }
    }

}
