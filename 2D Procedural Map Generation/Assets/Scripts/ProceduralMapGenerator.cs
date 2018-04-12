using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Random = UnityEngine.Random;

public class ProceduralMapGenerator : MonoBehaviour
{
    //The point where the first block of the map will be spawned.
    [Header("Position of first tile")]
    [SerializeField] private Transform _startPoint;

    [Space(10)]
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _colliderTilemap;
    [SerializeField] private Tilemap _backgroundTilemap;

    private Vector3Int _startTilePosition;

    [Space(10)]
    [Header("Generation Settings")]
    [Range(0, 100)] public ushort mapLength, heightLimit, depthLimit;
    [Range(0, 10)] public ushort levelHeight;

    [Space(10)]
    [Header("Tiles")]
    [SerializeField] private List<Tile> _availableGroundTiles = new List<Tile>();
    // availableGroundTiles[0] = Black tile 1x1px
    // availableGroundTiles[1] = Small slope leading up to the right 1000x2000px
    // availableGroundTiles[2] = Big slope leading up to the right 1000x3000px
    // availableGroundTiles[3] = Small slope leading down to the right 1000x2000px
    // availableGroundTiles[4] = Big slope leading down to the right 1000x3000px

    // Same as ground tile indexing but flipped sprites
    [SerializeField] private List<Tile> _availableRoofTiles = new List<Tile>();
    [SerializeField] private List<Tile> _availableWallTiles = new List<Tile>();

    internal List<Vector3Int> SpawnedTilePositions = new List<Vector3Int>();

    private List<Vector3Int> _firstColumnTiles = new List<Vector3Int>();
	
    public void GenerateMap()
    {
        if (mapLength <= 0)
        {
            return;
        }

        // Gets the tile position from the transform and then put a black square there.
        _startTilePosition = new Vector3Int(Mathf.FloorToInt(_startPoint.position.x), Mathf.FloorToInt(_startPoint.position.y), Mathf.FloorToInt(_startPoint.position.z));
        SetRowOfTiles(_startTilePosition, _availableGroundTiles[0]);

        for (int amountOfTiles = 1; amountOfTiles < mapLength;)
        {
            Vector3Int currentTilePosition = _firstColumnTiles.LastOrDefault();
            Vector3Int betweenTilePosition = new Vector3Int();
            Vector3Int skipTilePosition = new Vector3Int();

            if (_firstColumnTiles.Count + 2 > mapLength)
            {
                betweenTilePosition.y = currentTilePosition.y;
                betweenTilePosition.x = _firstColumnTiles.LastOrDefault().x + 1;
                SetRowOfTiles(betweenTilePosition, _availableGroundTiles[0]);
                amountOfTiles++;
                break;
            }

            betweenTilePosition = new Vector3Int(currentTilePosition.x + 1, currentTilePosition.y, currentTilePosition.z);
            skipTilePosition = GetNextTilePosition(currentTilePosition);
            Tile betweenTile = GetBetweenTile(currentTilePosition, skipTilePosition);


            SetRowOfTiles(betweenTilePosition, betweenTile);
            amountOfTiles++;

            SetRowOfTiles(skipTilePosition, _availableGroundTiles[0]);
            amountOfTiles++;
        }
    }

    private Vector3Int GetNextTilePosition (Vector3Int currentTilePosition)
    {
        Vector3Int nextTilePosition = new Vector3Int(currentTilePosition.x + 2, 0, 0);

        int nextTileHeight = Random.Range(currentTilePosition.y - 2, currentTilePosition.y + 3);

        //If the tile height is over the heightLimit
        if ((nextTileHeight > _startTilePosition.y + heightLimit))
        {
            //Set the height to heightLimit
            nextTilePosition.y = _startTilePosition.y + heightLimit;
            return nextTilePosition;
        }

        //If the tile height is under the depthLimit
        if ((nextTileHeight < _startTilePosition.y - depthLimit))
        {

            //Set the height to depthLimit
            nextTilePosition.y = _startTilePosition.y - depthLimit;

            return nextTilePosition;
        }

        nextTilePosition.y = nextTileHeight;

        return nextTilePosition;
    }

    internal Vector3Int GetTilemapMiddleTile()
    {
        return SpawnedTilePositions[(SpawnedTilePositions.Count) / 2];
    }

    internal Vector3 GetTileMapAverageMidPoint()
    {
        if (mapLength <= 0)
        {
            return Vector3.zero;
        }

        float middleYAxis = 0;
        float middleXAxis = SpawnedTilePositions[SpawnedTilePositions.Count / 2].x;

        for (int i = 0; i < SpawnedTilePositions.Count; i++)
        {
            middleYAxis += SpawnedTilePositions[i].y;
        }
        middleYAxis /= SpawnedTilePositions.Count;

        return new Vector3(middleXAxis, middleYAxis, 0);
    }


    private void SetRowOfTiles(Vector3Int startPointVector3Int, Tile firstTile)
    {
        SetTile(startPointVector3Int, firstTile, _colliderTilemap);

        //If level height is 0, no background and no ceiling is spawned.
        if (levelHeight <= 0)
        {
            _firstColumnTiles.Add(startPointVector3Int);
            return;
        }

        Vector3Int backgroundTilePosition = new Vector3Int {z = 0};

        int backgroundAmountOffset = 0;

        if (_availableGroundTiles.FindIndex(x => x == firstTile) == 0)
        {
            backgroundTilePosition.x = startPointVector3Int.x;
            backgroundTilePosition.y = startPointVector3Int.y + 1;
            backgroundAmountOffset = 0;
        }
        else if (_availableGroundTiles.FindIndex(x => x == firstTile) == 1)
        {
            backgroundTilePosition.x = startPointVector3Int.x;
            backgroundTilePosition.y = startPointVector3Int.y + 1;
            backgroundAmountOffset = 1;
        }
        else if (_availableGroundTiles.FindIndex(x => x == firstTile) == 2)
        {
            backgroundTilePosition.x = startPointVector3Int.x;
            backgroundTilePosition.y = startPointVector3Int.y + 1;
            backgroundAmountOffset = 2;
        }
        else if (_availableGroundTiles.FindIndex(x => x == firstTile) == 3)
        {
            backgroundTilePosition.x = startPointVector3Int.x;
            backgroundTilePosition.y = startPointVector3Int.y;
            backgroundAmountOffset = 1;
        }
        else if (_availableGroundTiles.FindIndex(x => x == firstTile) == 4)
        {
            backgroundTilePosition.x = startPointVector3Int.x;
            backgroundTilePosition.y = startPointVector3Int.y - 1;
            backgroundAmountOffset = 2;
        }

        //Background spawning
        for (int i = 0; i < levelHeight + backgroundAmountOffset; i++)
        {
            Tile backgroundTile = GetRandomBackgroundTile();
            SetTile(backgroundTilePosition, backgroundTile, _backgroundTilemap);
            backgroundTilePosition.y += 1;
        }

        int groundTileIndex = _availableGroundTiles.FindIndex(x => x == firstTile);
        SetTile(backgroundTilePosition, _availableRoofTiles[groundTileIndex], _colliderTilemap);

        _firstColumnTiles.Add(startPointVector3Int);
    }

    private Tile GetRandomBackgroundTile()
    {
        int r = Random.Range(0, _availableWallTiles.Count);
        return _availableWallTiles[r];
    }

    void SetTile(Vector3Int pos, Tile tile, Tilemap map)
    {
        map.SetTile(pos, tile);
        SpawnedTilePositions.Add(pos);
    }

    //Figures out the correct transition tile between the two positions.
    Tile GetBetweenTile(Vector3Int firstPos, Vector3Int thirdPos)
    {
        if (firstPos.y == thirdPos.y)
        {
            return _availableGroundTiles[0];
        }

        if (firstPos.y + 1 == thirdPos.y)
        {
            return _availableGroundTiles[1];
        }

        if (firstPos.y - 1 == thirdPos.y)
        {
            return _availableGroundTiles[3];
        }

        if (firstPos.y + 2 == thirdPos.y)
        {
            return _availableGroundTiles[2];
        }

        if (firstPos.y - 2 == thirdPos.y)
        {
            return _availableGroundTiles[4];
        }

        Debug.LogWarning("There is an error with method \"GetBetweenTile\" in ProceduralMapGenerator.cs");
        return new Tile();
    }

    public void ResetGame()
    {
        _colliderTilemap.ClearAllTiles();
        _backgroundTilemap.ClearAllTiles();
        SpawnedTilePositions.Clear();
        _firstColumnTiles.Clear();
    }
}
