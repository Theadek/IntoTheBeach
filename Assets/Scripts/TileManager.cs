using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [SerializeField] private GameObject TilePrefab;


    private Vector2Int tileCount = new Vector2Int(8, 8);
    private Grid grid;
    private Tile[,] tiles;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        grid = GetComponent<Grid>();
        tiles = new Tile[tileCount[0], tileCount[1]];
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
                var tile = Instantiate(TilePrefab, position, TilePrefab.transform.rotation, transform).GetComponent<Tile>();
                tile.SetXY(x, y);
                tiles[x, y] = tile;
            }
        }
    }

    public Tile[,] GetTiles()
    {
        return tiles;
    }

    public Tile GetTile(Vector2Int xy)
    {
        if(xy.x < 0 || xy.x >= tileCount.x) return null;
        if(xy.y < 0 || xy.y >= tileCount.y) return null;
        return tiles[xy.x, xy.y];
    }

    public Grid GetGrid() => grid;

    public bool TryGetNeighborTile(Tile tile, Direction direction, out Tile neighbor)
    {
        neighbor = null;

        if (direction == Direction.North)
        {
            if (tile.GetXY().x + 1 >= tileCount[0])
                return false;

            neighbor = tiles[tile.GetXY().x + 1, tile.GetXY().y];
            return true;
        }
        else if (direction == Direction.South)
        {
            if (tile.GetXY().x - 1 < 0)
                return false;

            neighbor = tiles[tile.GetXY().x - 1, tile.GetXY().y];
            return true;
        }
        else if (direction == Direction.West)
        {
            if (tile.GetXY().y + 1 >= tileCount[1])
                return false;

            neighbor = tiles[tile.GetXY().x, tile.GetXY().y + 1];
            return true;
        }
        else if (direction == Direction.East)
        {
            if (tile.GetXY().y - 1 < 0)
                return false;

            neighbor = tiles[tile.GetXY().x, tile.GetXY().y - 1];
            return true;
        }

        return false;
    }


    public bool TryGetNeighborTileObject(Tile tile, Direction direction, out TileObject neighbor)
    {
        neighbor = null;
        var neighborTileFound = TryGetNeighborTile(tile, direction, out Tile neighborTile);
        if (!neighborTileFound)
        {
            return false;
        }

        if(neighborTile.TryGetObjectOnThisTile(out neighbor))
        {
            return true;
        }
        return false;
    }

    public void MoveTileObject(TileObject from, Tile to)
    {
        to.TrySetObjectOnThisTile(from);
    }
    public void MoveTileObject(Tile from, Tile to)
    {
        MoveTileObject(from.GetObjectOnThisTile(), to);
    }

    public void MoveTileObjectByOne(Tile from, Direction direction)
    {
        if(TryGetNeighborTile(from, direction, out Tile tile)){
            MoveTileObject(from, tile);
        }
    }
    public void MoveTileObjectByOne(TileObject from, Direction direction)
    {
        MoveTileObjectByOne(from.GetTile(), direction);
    }



    // Alpha Testing Script
    [SerializeField] private TileObject playerFighterPrefab;
    [SerializeField] private TileObject treePrefab;
    private void RandomizeMap(float chance = 0.3f)
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
