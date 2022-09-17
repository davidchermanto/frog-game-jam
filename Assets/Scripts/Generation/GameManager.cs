using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private World generator;

    void Start()
    {
        generator.Generate();
    }
}
