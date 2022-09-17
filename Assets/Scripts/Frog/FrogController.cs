using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private World world;

    private int currentX;
    private int currentY;

    private float currentSanity;
    private float maxSanity;

    private int jumpRange;
    private int tongueRange;

    public void Initialize(World world)
    {
        this.world = world;
        
        currentX = GlobalVars.boardSize / 2;
        currentY = GlobalVars.boardSize / 2;

        jumpRange = 3;
        tongueRange = 5;

        AdjustPosition();
    }

    public void MoveTo(string direction, int force)
    {
        int forceX = 0;
        int forceY = 0;

        switch (direction)
        {
            case "up":
                forceY += force;
                break;
            case "down":
                forceY -= force;
                break;
            case "left":
                forceX -= force;
                break;
            case "right":
                forceX += force;
                break;
            default:
                break;
        }

        int targetX = currentX + forceX;
        int targetY = currentY - forceY;

        if(world.CheckTileEmpty(targetX, targetY))
        {
            MoveAnim(direction, targetX, targetY);
        }
    }

    // Animates moving to the new "Current Position"
    public void MoveAnim(string direction, int targetX, int targetY)
    {

    }

    public void AdjustPosition()
    {
        transform.position = new Vector3(currentX * GlobalVars.tileSize, currentY * GlobalVars.tileSize, 0);
    }

    public int GetX()
    {
        return currentX;
    }

    public int GetY()
    {
        return currentY;
    }
}
