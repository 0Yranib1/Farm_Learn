using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDetailsList_SO", menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> SoundDetailsList;
    
    public SoundDetails GetSoundDetails(SoundName soundName) => SoundDetailsList.Find(x => x.soundName == soundName);
}

[Serializable]
public class SoundDetails
{
    public SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)] 
    public float soundPitchMin ;

    [Range(0.1f, 1.5f)] 
    public float soundPitchMax ;

    [Range(0.1f, 1f)] 
    public float soundVolume ;
}