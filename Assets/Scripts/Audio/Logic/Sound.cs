using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent( typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void SetSound(SoundDetails soundDetails)
    {
        audioSource.clip = soundDetails.soundClip;
        audioSource.volume = soundDetails.soundVolume;
        audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);
    }
}
