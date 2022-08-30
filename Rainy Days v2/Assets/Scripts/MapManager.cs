using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public Tilemap map;
    public List<TileData> tileDatas;
    public Dictionary<TileBase, TileData> dataFromTiles;
    public Player player;

    void Start()
    {
        // reading all kinds of tiles
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.Tiles)
            {
                dataFromTiles.Add(tile, tileData);

            }
        }

        SpawnPlayer();

    }


    void Update()
    {
        // find a* path on click
        if (Input.GetMouseButtonDown(0) && player.State == PlayerState.Neutral)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);
            APath path = FindAPath(player.currentGridPosition, gridPosition);
            Debug.Log(path);
            StopAllCoroutines();
            StartCoroutine(WalkPath(path));
        }

    }

    public IEnumerator WalkPath(APath path)
    {
        Field[] fields = path.Fields.ToArray();
        for (int i = 1; i < fields.Length; i++)
        {
            player.startTime = Time.time;
            Vector3 nf = map.CellToWorld(fields[i].GridPosition);
            player.nextField = new(nf.x, nf.y + 0.25f, 1);
            yield return new WaitForSeconds(1f);
        }
        player.currentGridPosition = fields[fields.Length - 1].GridPosition;
    }


    public APath FindAPath(Vector3Int start, Vector3Int finish)
    {
        int firstX, lastX, firstY, lastY;
        firstX = map.origin.x;
        firstY = map.origin.y;
        lastX = firstX + map.size.x;
        lastY = firstY + map.size.y;
        Dictionary<Vector3Int, Field> fields = new();
        const float heuristicScale = 1.5f;
        // browse all tiles
        for (int x = firstX; x <= lastX; x++)
        {
            for (int y = firstY; y <= lastY; y++)
            {
                Vector3Int gridPos = new(x, y);
                if (map.GetTile(gridPos) != null)
                {
                    TileData tile = dataFromTiles[map.GetTile(gridPos)];
                    if (CheckIfTileIsWalkable(tile))
                    {
                        float heuristic = Vector3Int.Distance(start, finish) * heuristicScale;
                        Field field = new(gridPos, tile, heuristic);
                        fields.Add(gridPos, field);
                    }
                }

            }
        }

        // neighbours
        foreach (Field field in fields.Values)
        {
            // south
            Vector3Int s = new(field.GridPosition.x - 1, field.GridPosition.y);
            if (map.GetTile(s) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(s)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[s]);
                }
            }
            // north
            Vector3Int n = new(field.GridPosition.x + 1, field.GridPosition.y);
            if (map.GetTile(n) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(n)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[n]);
                }
            }
            // east
            Vector3Int e = new(field.GridPosition.x, field.GridPosition.y - 1);
            if (map.GetTile(e) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(e)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[e]);
                }
            }
            // west
            Vector3Int w = new(field.GridPosition.x, field.GridPosition.y + 1);
            if (map.GetTile(w) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(w)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[w]);
                }
            }
            // north east
            Vector3Int ne = new(field.GridPosition.x + 1, field.GridPosition.y - 1);
            if (map.GetTile(ne) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(ne)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[ne]);
                }
            }
            // north west
            Vector3Int nw = new(field.GridPosition.x + 1, field.GridPosition.y + 1);
            if (map.GetTile(nw) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(nw)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[nw]);
                }
            }
            // south east
            Vector3Int se = new(field.GridPosition.x - 1, field.GridPosition.y - 1);
            if (map.GetTile(se) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(se)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[se]);
                }
            }
            // south west
            Vector3Int sw = new(field.GridPosition.x - 1, field.GridPosition.y + 1);
            if (map.GetTile(sw) != null)
            {
                TileData tile = dataFromTiles[map.GetTile(sw)];
                if (CheckIfTileIsWalkable(tile))
                {
                    field.Neighbours.Add(fields[sw]);
                }
            }
        }

        List<Field> done = new();
        Field startField = fields[start];
        Field finishField = fields[finish];
        Field currentField = startField;
        FieldsPriorityQueue queue = new();
        startField.BestScore = 0 + startField.Heuristic;
        startField.DijkstraScore = 0;
        startField.Path.Add(startField);

        while (currentField != finishField)
        {
            foreach (Field f in currentField.Neighbours)
            {
                if (!done.Contains(f))
                {
                    float score = currentField.DijkstraScore + Distance(currentField, f);
                    if (score + f.Heuristic < f.BestScore)
                    {
                        f.BestScore = score + f.Heuristic;
                        f.DijkstraScore = score;
                        queue.Enqueue(f);

                        f.Path.Clear();
                        foreach (Field field in currentField.Path)
                        {
                            f.Path.Add(field);
                        }
                        f.Path.Add(f);
                    }
                }
            }
            done.Add(currentField);
            currentField = queue.Dequeue();
        }

        APath bestPath = new(finishField.Path, finishField.BestScore);

        return bestPath;
    }

    private float Distance(Field from, Field to)
    {
        float result = 1f; // scale it perchance
        return result * to.TileData.PoisonLevel;
    }

    public bool CheckIfTileIsWalkable(TileData tile)
    {
        if (tile.Type == TileType.Locked)
            return false;
        else
            return true;
    }

    public void SpawnPlayer()
    {
        Vector3Int spawn = new(player.spawnGridPosition.x, player.spawnGridPosition.y, 1);
        player = Instantiate(player, map.CellToWorld(spawn), Quaternion.identity);
        player.currentGridPosition = player.spawnGridPosition;
    }
}
