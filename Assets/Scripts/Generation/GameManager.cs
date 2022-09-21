using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private World world;

    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject menuCanvas;

    public void StartGame()
    {
        SFXController.Instance.PlayClip(0);
        world.Generate();
        world.StartGame();
        gameOverCanvas.SetActive(false);
        menuCanvas.SetActive(false);
    }
}
