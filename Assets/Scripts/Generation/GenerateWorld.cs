using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
    [SerializeField] private int boardSize;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject tileFolder;

    private List<Tile> tiles = new List<Tile>();

    public void Generate()
    {
        for(int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                GameObject newTileObj = Instantiate(tilePrefab);
                Tile newTile = newTileObj.GetComponent<Tile>();
                newTile.SetCoordinates(i, j);

                tiles.Add(newTile);
                newTileObj.transform.SetParent(tileFolder.transform);
            }
        }
    }

    
}
