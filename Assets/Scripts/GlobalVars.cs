using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVars
{
    public static int boardSize = 35;
    public static float tileSize = 2.56f;

    public static int baseJumpRange = 3;
    public static int baseTongueRange = 5;

    public static float maxSanity = 100;
    public static float sanityPerSecond = -2.5f;
    public static float sanityPerTurn = -2f;
    public static float sanityPerFly = 6;

    public static float scorePerTurn = 10;
    public static float scorePerFly = 250;

    public static int initialFlies = 60;
}
