using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int x;
    private int y;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;
    [SerializeField] private BoxCollider2D box;

    public bool availableToMoveTo;

    public void Initialize(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

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

    void OnMouseEnter()
    {
        if (availableToMoveTo)
        {

        }
        else
        {

        }
    }

    void OnMouseExit()
    {
        if (availableToMoveTo)
        {

        }
        else
        {

        }
    }

    private void BorderLightUp()
    {

    }

    public SpriteRenderer GetSprite()
    {
        return spriteRenderer;
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
