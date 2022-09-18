using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private World world;

    void Start()
    {
        world.Generate();
        world.StartGame();
    }
}
