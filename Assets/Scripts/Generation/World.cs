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

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileFolder;

    [SerializeField] private List<Sprite> tileSet;

    [SerializeField] private FrogController frog;

    private List<Tile> tiles = new List<Tile>();

    public void Generate()
    {
        for(int i = 0; i < GlobalVars.boardSize; i++)
        {
            for(int j = 0; j < GlobalVars.boardSize; j++)
            {
                GameObject newTileObj = Instantiate(tilePrefab);
                Tile newTile = newTileObj.GetComponent<Tile>();
                newTile.Initialize(tileSet[Random.Range(0, tileSet.Count)]);
                newTile.SetCoordinates(i, j);

                tiles.Add(newTile);
                newTileObj.transform.SetParent(tileFolder.transform);
            }
        }

        frog.Initialize(this);
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

    // If player moves 3x to the left, then expand world by 3 tiles to the left
    public void AdjustWorld(int moveX, int moveY)
    {

    }
}
