using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    int currentId;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileFolder;

    [SerializeField] private List<Sprite> tileSet;
    [SerializeField] private GameObject flyPrefab;

    [SerializeField] private FrogController frog;

    private List<Tile> tiles = new List<Tile>();
    private List<Tile> currentReachableTiles = new List<Tile>();

    private List<FlyController> flies = new List<FlyController>();

    // If the mode is jumping. If false, then its tongueing
    private bool jumping;

    private bool isPlaying;
    private bool isPaused;

    // Button
    [SerializeField] private Image leap;
    [SerializeField] private Image tongue;

    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SetJump();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                SetTongue();
            }
        }
    }

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

        for(int i = 0; i < GlobalVars.initialFlies; i++)
        {
            SpawnFly();
        }
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

            if (jumping && playerCanReach && isTileEmpty)
            {
                tile.BorderLightUp();
            }
            else if (playerCanReach)
            {
                tile.BorderTongueLightUp();
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

            if(TileReachableByPlayer(tileX, tileY, jumping))
            {
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
                    if (CheckTileEmpty(tileX, tileY))
                    {
                        frog.MoveTo(direction, tileX, tileY);
                    }
                }
                else if(!(frog.GetX() == tileX && frog.GetY() == tileY))
                {
                    // Tongue
                    FlyController fly = CheckTileForFlies(tile);

                    frog.Face(direction);
                    frog.TryTongue(tileX, tileY);

                    if(fly != null)
                    {
                        // Kill fly
                        flies.Remove(fly);
                        GameObject destroyedFly = fly.gameObject;

                        Destroy(destroyedFly);
                    }
                }
            }
        }
    }

    public void StartPlayerTurn()
    {
        UpdateRangeDisplay();

        frog.isPlayerTurn = true;
    }

    public void SpawnFly()
    {
        // Pick spawn spot
        int min = 0;
        int max = GlobalVars.boardSize;

        int randX = Random.Range(min, max);
        int randY = Random.Range(min, max);

        int rand = Random.Range(0, 3);
        string direction = "";

        if (CheckTileEmpty(randX, randY))
        {
            GameObject newFly = Instantiate(flyPrefab);
            FlyController newFlyController = newFly.GetComponent<FlyController>();
            newFlyController.Initialize(this, randX, randY);

            switch (rand)
            {
                case 0:
                    direction = "up";
                    break;
                case 1:
                    direction = "down";
                    break;
                case 2:
                    direction = "left";
                    break;
                case 3:
                    direction = "right";
                    break;
                default:
                    break;
            }

            newFlyController.Face(direction);

            flies.Add(newFlyController);
        }
    }

    public void UpdateRangeDisplay()
    {
        foreach (Tile tile in currentReachableTiles)
        {
            tile.BorderDimDown();
        }
        currentReachableTiles = new List<Tile>();

        currentReachableTiles = GetTilesInFrogRange();
        foreach (Tile tile in currentReachableTiles)
        {
            tile.BorderSemiLight();
        }
    }

    public void FinishPlayerTurn()
    {
        StartCoroutine(FinishPlayerTurnCor());
    }

    public IEnumerator FinishPlayerTurnCor()
    {
        // Tick all other entities
        foreach (FlyController fly in flies)
        {
            fly.Tick();
        }

        yield return new WaitForEndOfFrame();

        if(Random.Range(0, 1f) > 0.3f)
        {
            SpawnFly();
        }

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

    public FlyController CheckTileForFlies(Tile tile)
    {
        foreach(FlyController fly in flies)
        {
            if(fly.GetX() == tile.GetX() && fly.GetY() == tile.GetY())
            {
                return fly;
            }
        }

        return null;
    }

    // Checks if tile is empty
    public bool CheckTileEmpty(int currentX, int currentY)
    {
        if(currentX < 0 || currentX > GlobalVars.boardSize)
        {
            return false;
        }

        if (currentY < 0 || currentY > GlobalVars.boardSize)
        {
            return false;
        }

        // Check for player
        if (frog.GetX() == currentX && frog.GetY() == currentY)
        {
            return false;
        }

        foreach(FlyController fly in flies)
        {
            if(fly.GetX() == currentX && fly.GetY() == currentY)
            {
                return false;
            }
        }

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

    public void SetJump()
    {
        jumping = true;

        leap.color = new Color(1, 1, 1);
        tongue.color = new Color(0.4f, 0.4f, 0.4f);

        UpdateRangeDisplay();
    }

    public void SetTongue()
    {
        jumping = false;

        tongue.color = new Color(1, 1, 1);
        leap.color = new Color(0.4f, 0.4f, 0.4f);

        UpdateRangeDisplay();
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
