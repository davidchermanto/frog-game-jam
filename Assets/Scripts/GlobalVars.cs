using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVars
{
    public static int boardSize = 35;
    public static float tileSize = 2.56f;

    public static int baseJumpRange = 3;
    public static int baseTongueRange = 4;

    public static float maxSanity = 100;
    public static float sanityPerSecond = -1;
    public static float sanityPerTurn = -4;
    public static float sanityPerFly = 6;

    public static int initialFlies = 32;
}
