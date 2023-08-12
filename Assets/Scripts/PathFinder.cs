using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public event EventHandler OnPossibleMovesChanged;

    public static PathFinder Instance { get; private set; }

    private List<PathNode> possibleMoves;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        possibleMoves = null;
    }

    private void Start()
    {
        GameManager.Instance.OnSelectedObject += GameManager_OnSelectedObject;
    }

    private void GameManager_OnSelectedObject(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.TryGetSelectedObject(out TileObject tileObject))
        {
            CreateListOfPossibleMoves(tileObject);
        }
        else
        {
            ClearPossibleMoves();
        }
    }

    public class PathNode {
        public PathNode(Vector2Int xy, PathNode previous = null)
        {
            XY = xy;
            previousNode = previous;
        }
        public Vector2Int XY;
        public PathNode previousNode;
    }

    private void CreateListOfPossibleMoves(TileObject tileObject)
    {
        if (!tileObject.CanMove())
            return;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        List<PathNode> toSearch = new List<PathNode>();
        possibleMoves = new List<PathNode>();


        toSearch.Add(new PathNode(tileObject.GetTile().GetXY(), null));
        visited.Add(toSearch[0].XY);

        for (int distance=0; distance < tileObject.GetMovement(); distance++)
        {
            List<PathNode> newToSearch = new List<PathNode>();
            while (toSearch.Any())
            {
                var current = toSearch[0];
                toSearch.Remove(current);
                Tile currentTile = TileManager.Instance.GetTile(current.XY);

                // TODO Check if Direction++ works
                for (Direction direction = 0; direction < Direction.COUNT; direction++)
                {
                    Vector2Int possiblePlace = Helpers.GetMovedVector2Int(current.XY, direction);
                    if (visited.Contains(possiblePlace)) continue;

                    PathNode newPathNode = new PathNode(possiblePlace, current);

                    if (TileManager.Instance.TryGetNeighborTile(currentTile, direction, out Tile neighbor))
                    {
                        if(neighbor.TryGetObjectOnThisTile(out TileObject neighborObject))
                        {
                            if(tileObject.HasSameType(neighborObject) || tileObject.IsFlying())
                            {
                                newToSearch.Add(newPathNode);
                            }
                        }
                        else
                        {
                            //Empty Tile
                            newToSearch.Add(newPathNode);
                            possibleMoves.Add(newPathNode);
                        }
                        visited.Add(newPathNode.XY);
                    }
                }

            }
            toSearch = newToSearch;
        }
        OnPossibleMovesChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ClearPossibleMoves()
    {
        possibleMoves = null;
        OnPossibleMovesChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool ListConstainsXY(Vector2Int XY)
    {
        if(possibleMoves == null) return false;
        foreach(var pathNode in possibleMoves)
        {
            if (pathNode.XY == XY) return true;
        }
        return false;
    }

    public List<Vector2Int> GetPossibleMovesVector2()
    {
        List<Vector2Int> output = new List<Vector2Int>();
        foreach (var pathNode in possibleMoves)
        {
            output.Add(pathNode.XY);
        }
        return output;
    }

    public PathNode GetPathNodeForXY(Vector2Int XY)
    {
        if (possibleMoves == null) return null;
        foreach (var pathNode in possibleMoves)
        {
            if (pathNode.XY == XY) return pathNode;
        }
        return null;
    }

}
