using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManeger : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip[] sounds;

    public enum SoundType
    {
        TypeGameOver,
        TypePop,
        TypeSelect,
        TypeMove,
    }

    public static SoundManeger Instance;
    AudioSource audioSource;

    public void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType clipeType)
    {
        audioSource.PlayOneShot(sounds[(int)clipeType]); 
    }
}
