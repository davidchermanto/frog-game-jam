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

    public bool isPlayerTurn;

    public void Initialize(World world)
    {
        this.world = world;
        
        currentX = GlobalVars.boardSize / 2;
        currentY = GlobalVars.boardSize / 2;

        jumpRange = GlobalVars.baseJumpRange;
        tongueRange = GlobalVars.baseTongueRange;

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
        StartCoroutine(MoveAnim(currentX, targetX, currentY, targetY));

    }

    public void FinishTurn()
    {
        isPlayerTurn = false;
        world.FinishPlayerTurn();
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

    public int GetJumpRange()
    {
        return jumpRange;
    }

    public int GetTongueRange()
    {
        return tongueRange;
    }

    // Moves the player and ends their turn after
    private IEnumerator MoveAnim(int oldX, int newX, int oldY, int newY)
    {
        // Coordinate calculation
        float coordOldX = oldX * GlobalVars.tileSize;
        float coordOldY = oldY * GlobalVars.tileSize;

        float coordNewX = newX * GlobalVars.tileSize;
        float coordNewY = newY * GlobalVars.tileSize;

        float duration = 0.3f;
        float progress = 0;

        while (progress < 1)
        {
            progress += duration * Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(coordOldX, coordOldY, 0), 
                new Vector3(coordNewX, coordNewY, 0), progress);

            yield return new WaitForFixedUpdate();
        }

        currentX = newX;
        currentY = newY;

        FinishTurn();
    }
}
