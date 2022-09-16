using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GenerateWorld generator;

    void Start()
    {
        generator.Generate();
    }
}
