﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class World : MonoBehaviour
{
    int currentId;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileFolder;

    [SerializeField] private List<Sprite> tileSet;
    [SerializeField] private GameObject flyPrefab;
    [SerializeField] private GameObject impact;

    [SerializeField] private FrogController frog;

    private List<Tile> tiles = new List<Tile>();
    private List<Tile> currentReachableTiles = new List<Tile>();

    private List<FlyController> flies = new List<FlyController>();

    // If the mode is jumping. If false, then its tongueing
    private bool jumping;

    private bool isPlaying;

    private float currentSanity;
    private float maxSanity;

    private double score;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image sanityBar;

    // Button
    [SerializeField] private Image leap;
    [SerializeField] private Image tongue;

    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private TextMeshProUGUI gameOverScore;

    bool shifted1 = false;
    bool shifted2 = false;

    void Update()
    {
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (jumping)
                {
                    SetTongue();
                }
                else
                {
                    SetJump();
                }
            }
        }
    }

    public void Generate()
    {
        foreach(Tile tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles = new List<Tile>();

        foreach(FlyController fly in flies)
        {
            Destroy(fly.gameObject);
        }

        flies = new List<FlyController>();

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

        maxSanity = GlobalVars.maxSanity;
        currentSanity = GlobalVars.maxSanity;

        for (int i = 0; i < GlobalVars.initialFlies; i++)
        {
            SpawnFly();
        }
    }

    public void StartGame()
    {
        shifted1 = false;
        shifted2 = false;
        jumping = true;
        isPlaying = true;
        score = 0;

        StartCoroutine(UpdateUI());
        StartCoroutine(DepleteSanity());
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
                        SFXController.Instance.PlayClip(1);
                        flies.Remove(fly);
                        GameObject destroyedFly = fly.gameObject;

                        GameObject newImpact = Instantiate(impact);
                        newImpact.transform.position = new Vector3(tileX * GlobalVars.tileSize, tileY * GlobalVars.tileSize, 0);

                        Destroy(destroyedFly);

                        score += GlobalVars.scorePerFly;
                        currentSanity += GlobalVars.sanityPerFly;
                    }
                    else
                    {
                        SFXController.Instance.PlayClip(4);
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

            GameObject newImpact = Instantiate(impact);
            newImpact.transform.position = new Vector3(randX * GlobalVars.tileSize, randY * GlobalVars.tileSize, 0);

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
            if (jumping)
            {
                tile.BorderSemiLight();
            }
            else
            {
                tile.BorderTongueSemiLight();
            }
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

        SpawnFly();

        currentSanity += GlobalVars.sanityPerTurn;
        score += GlobalVars.scorePerTurn;

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
        if(currentX < 0 || currentX > GlobalVars.boardSize - 1)
        {
            return false;
        }

        if (currentY < 0 || currentY > GlobalVars.boardSize - 1)
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

    public void GameOver()
    {
        isPlaying = false;
        gameOverScore.SetText(score.ToString());
        gameOverCanvas.gameObject.SetActive(true);
        SFXController.Instance.PlayClip(3);
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

    public IEnumerator DepleteSanity()
    {
        while (isPlaying)
        {
            currentSanity += Time.deltaTime * GlobalVars.sanityPerSecond;

            if(currentSanity < 33 || shifted2)
            {
                MusicController.Instance.Play4();
                shifted2 = true;
            }
            else if (currentSanity < 66 || shifted1)
            {
                MusicController.Instance.Play3();
                shifted1 = true;
            }
            else
            {
                MusicController.Instance.Play2();
            }

            if (currentSanity <= 0)
            {
                MusicController.Instance.Play1();
                GameOver();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // This is bad practice, but this is a game jam so...
    public IEnumerator UpdateUI()
    {
        while (isPlaying)
        {
            sanityBar.fillAmount = currentSanity / maxSanity;
            scoreText.text = score.ToString();

            yield return new WaitForFixedUpdate();
        }
    }

    public List<Tile> GetReachableTiles()
    {
        return currentReachableTiles;
    }

    public bool isJumping()
    {
        return jumping;
    }
}
