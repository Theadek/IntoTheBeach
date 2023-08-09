using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject TilePrefab;


    private Vector2Int tileCount = new Vector2Int(8, 8);
    private Grid grid;
    private GameObject[,] tiles;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        tiles = new GameObject[tileCount[0], tileCount[1]];
    }

    private void Start()
    {
        GenerateGrid();
        RandomizeMap();
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
        for(int y = 0; y < tileCount[1]; y++)
        {
            for (int x = 0; x < tileCount[0]; x++)
            {
                var position = grid.GetCellCenterWorld(new Vector3Int(x, y, 0));
                var tile = Instantiate(TilePrefab, position, TilePrefab.transform.rotation, transform);
                tile.GetComponent<Tile>().SetXY(x, y);
                tiles[x, y] = tile;
            }
        }
    }


    [SerializeField] private TileObject playerFighterPrefab;
    [SerializeField] private TileObject treePrefab;
    private void RandomizeMap(float chance = 0.5f)
    {
        //player
        int playerX = Random.Range(0, tileCount[0] - 1);
        int playerY = Random.Range(0, tileCount[1] - 1);
        var newPlayerObject = TileObject.CreateTileObject(tiles[playerX, playerY].GetComponent<Tile>(), playerFighterPrefab.GetCharacterSO());
        tiles[playerX, playerY].GetComponent<Tile>().TrySetObjectOnThisTile(newPlayerObject);

        //trees
        for (int y = 0; y < tileCount[1]; y++)
        {
            for (int x = 0; x < tileCount[0]; x++)
            {
                if (tiles[x, y].GetComponent<Tile>().HasObjectOnThisTile()) continue;
                if(Random.Range(0f,1f) <= chance)
                {
                    var newTileObject = TileObject.CreateTileObject(tiles[x, y].GetComponent<Tile>(), treePrefab.GetCharacterSO());
                    tiles[x, y].GetComponent<Tile>().TrySetObjectOnThisTile(newTileObject);
                }
            }
        }
    }

}
