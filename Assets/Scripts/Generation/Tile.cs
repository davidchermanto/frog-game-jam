using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int id;
    
    private int x;
    private int y;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer borderRenderer;
    [SerializeField] private BoxCollider2D box;

    private World world;

    // Base color
    private float r = 0.67f;
    private float g = 0.56f;
    private float b = 0.23f;

    public bool availableToMoveTo;

    public void Initialize(Sprite sprite, World world, int id)
    {
        spriteRenderer.sprite = sprite;
        this.world = world;
        this.id = id;
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
        //Debug.Log(x + " / " + y);
        world.RequestHover(this);
    }

    void OnMouseExit()
    {
        if (world.GetReachableTiles().Contains(this))
        {
            BorderSemiLight();
        }
        else
        {
            BorderDimDown();
        }
    }

    void OnMouseDown()
    {
        world.RequestMove(this);
    }

    // Lights up the border, indicating that the tile is available to move to
    public void BorderLightUp()
    {
        borderRenderer.color = new Color(1, g, b, 1);
    }

    // Lights up semi, meaning it might be possible to move there
    public void BorderSemiLight()
    {
        borderRenderer.color = new Color(r, g, b, 0.3f);
    }

    public void BorderDimDown()
    {
        borderRenderer.color = new Color(r, g, b, 0.1f);
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
