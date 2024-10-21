using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------------------------- Audio Source -------------------------------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource SFXSource_Extra;
    [SerializeField] AudioSource ambienceSource;

    [Header("------------------------------- Audio Clips -------------------------------")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip footstep;
    public AudioClip swimming;
}
