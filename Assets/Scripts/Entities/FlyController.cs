using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : MonoBehaviour
{
    private World world;

    private int currentX;
    private int currentY;

    [SerializeField] private SpriteRenderer flySprite;

    public void Initialize(World world, int x, int y)
    {
        this.world = world;

        currentX = x;
        currentY = y;

        AdjustPosition();
    }

    public void Tick()
    {
        int rand = Random.Range(0, 4);
        string direction = "";
        int targetX = currentX;
        int targetY = currentY;

        switch (rand)
        {
            case 0:
                direction = "up";
                targetY++;
                break;
            case 1:
                direction = "down";
                targetY--;
                break;
            case 2:
                direction = "left";
                targetX--;
                break;
            case 3:
                direction = "right";
                targetX++;
                break;
            default:
                break;
        }

        MoveTo(direction, targetX, targetY);
    }

    public void MoveTo(string direction, int targetX, int targetY)
    {
        if (world.CheckTileEmpty(targetX, targetY))
        {
            Face(direction);

            MoveAnim(direction, targetX, targetY);
        }
    }

    public void Face(string direction)
    {
        switch (direction)
        {
            case "up":
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                break;
            case "down":
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case "left":
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                break;
            case "right":
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

    public void MoveAnim(string direction, int targetX, int targetY)
    {
        StartCoroutine(MoveAnim(currentX, targetX, currentY, targetY));
    }

    // Moves the player and ends their turn after
    private IEnumerator MoveAnim(int oldX, int newX, int oldY, int newY)
    {
        // Coordinate calculation
        float coordOldX = oldX * GlobalVars.tileSize;
        float coordOldY = oldY * GlobalVars.tileSize;

        float coordNewX = newX * GlobalVars.tileSize;
        float coordNewY = newY * GlobalVars.tileSize;

        float duration = 0.12f;
        float progress = 0;

        while (progress < 1)
        {
            progress += 1 / duration * Time.deltaTime;
            transform.position = Vector3.Lerp(new Vector3(coordOldX, coordOldY, 0),
                new Vector3(coordNewX, coordNewY, 0), progress);

            yield return new WaitForFixedUpdate();
        }

        currentX = newX;
        currentY = newY;
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
