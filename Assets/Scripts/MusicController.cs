using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [SerializeField] private List<AudioSource> speakers;

    void Awake()
    {
        Instance = this;
    }

    public void Play1()
    {
        ShutOff();
        speakers[0].volume = 1;
    }

    public void Play2()
    {
        ShutOff();
        speakers[1].volume = 1;
    }

    public void Play3()
    {
        ShutOff();
        speakers[2].volume = 1;
    }

    public void Play4()
    {
        ShutOff();
        speakers[3].volume = 1;
    }

    public void ShutOff()
    {
        foreach (AudioSource speaker in speakers)
        {
            speaker.volume = 0;
        }
    }

}
