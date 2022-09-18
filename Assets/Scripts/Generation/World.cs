using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // The corners of the world in size, x and y
    int currentLeft;
    int currentRight;
    int currentTop;
    int currentBot;

    int currentId;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileFolder;

    [SerializeField] private List<Sprite> tileSet;

    [SerializeField] private FrogController frog;

    private List<Tile> tiles = new List<Tile>();
    private List<Tile> currentReachableTiles = new List<Tile>();

    // If the mode is jumping. If false, then its tongueing
    private bool jumping;

    private bool isPlaying;
    private bool isPaused;

    public void Generate()
    {
        currentId = 0;

        for (int i = 0; i < GlobalVars.boardSize; i++)
        {
            for (int j = 0; j < GlobalVars.boardSize; j++)
            {
                GameObject newTileObj = Instantiate(tilePrefab);
                Tile newTile = newTileObj.GetComponent<Tile>();
                newTile.Initialize(tileSet[Random.Range(0, tileSet.Count)], this, currentId++);
                newTile.SetCoordinates(i, j);

                tiles.Add(newTile);
                newTileObj.transform.SetParent(tileFolder.transform);
            }
        }

        frog.Initialize(this);
    }

    public void StartGame()
    {
        jumping = true;
        isPlaying = true;
        StartPlayerTurn();
        // TODO
    }

    // When a mouse hovers over a tile, tile requests this
    public void RequestHover(Tile tile)
    {
        if (isPlaying && frog.isPlayerTurn)
        {
            bool playerCanReach = TileReachableByPlayer(tile.GetX(), tile.GetY(), jumping);
            bool isTileEmpty = CheckTileEmpty(tile.GetX(), tile.GetY());

            if (playerCanReach && isTileEmpty)
            {
                tile.BorderLightUp();
            }
        }
    }

    public void RequestMove(Tile tile)
    {
        if (isPlaying && frog.isPlayerTurn)
        {
            // Get direction
            int frogX = frog.GetX();
            int frogY = frog.GetY();

            int tileX = tile.GetX();
            int tileY = tile.GetY();

            string direction = "";

            // Top / down
            if (tileY > frogY)
            {
                direction = "up";
            }
            else if (tileY < frogY)
            {
                direction = "down";
            }
            // Left / Right
            if (tileX > frogX)
            {
                direction = "left";
            }
            else if (tileX < frogX)
            {
                direction = "right";
            }

            if (jumping)
            {
                // Try to move towards the tile
                if(CheckTileEmpty(tileX, tileY))
                {
                    frog.MoveTo(direction, tileX, tileY);
                }
            }
            else
            {
                // Tongue
            }
        }
    }

    public void StartPlayerTurn()
    {
        foreach(Tile tile in currentReachableTiles)
        {
            tile.BorderDimDown();
        }
        currentReachableTiles = new List<Tile>();

        currentReachableTiles = GetTilesInFrogRange();
        foreach (Tile tile in currentReachableTiles)
        {
            tile.BorderSemiLight();
        }

        frog.isPlayerTurn = true;
    }

    public void FinishPlayerTurn()
    {
        // Tick all other entities

        // Adjust world

        // Give turn back to player
        StartPlayerTurn();
    }

    public List<Tile> GetTilesInFrogRange()
    {
        List<Tile> inRanges = new List<Tile>();

        foreach(Tile tile in tiles)
        {
            if(TileReachableByPlayer(tile.GetX(), tile.GetY(), jumping))
            {
                inRanges.Add(tile);
            }
        }

        return inRanges;
    }

    // Checks if tile is empty
    public bool CheckTileEmpty(int currentX, int currentY)
    {
        // Check for player
        if(frog.GetX() == currentX && frog.GetY() == currentY)
        {
            return false;
        }

        // Todo: Check for entity


        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jump">If jump is false, then the function should calculate for tongue</param>
    /// <returns></returns>
    public bool TileReachableByPlayer(int x, int y, bool jump)
    {
        int playerRange;

        if (jump)
        {
            playerRange = frog.GetJumpRange();
        }
        else
        {
            playerRange = frog.GetTongueRange();
        }

        int playerX = frog.GetX();
        int playerY = frog.GetY();

        // Up or down
        if (playerX - x == 0)
        {
            if (Mathf.Abs(y - playerY) <= playerRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Left or right
        if(playerY - y == 0)
        {
            if (Mathf.Abs(x - playerX) <= playerRange)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    // If player moves 3x to the left, then expand world by 3 tiles to the left
    public void AdjustWorld(int moveX, int moveY)
    {

    }

    public List<Tile> GetReachableTiles()
    {
        return currentReachableTiles;
    }
}
