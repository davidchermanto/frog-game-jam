using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x;
    private int y;

    private Sprite sprite;

    public void SetCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;

        AdjustPosition();
    }

    public void AdjustPosition()
    {
        transform.position = new Vector3(x * GlobalVars.tileSize, y * GlobalVars.tileSize, 0);
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}
