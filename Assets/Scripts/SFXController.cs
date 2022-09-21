using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public static SFXController Instance;

    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private AudioSource speaker;

    void Awake()
    {
        Instance = this;
    }

    public void PlayClip(int id)
    {
        speaker.PlayOneShot(clips[id]);
    }
}
