using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that will be implemented for further creation of AudioEvent Triggers
/// </summary>
public abstract class AudioEvent : ScriptableObject
{
    public abstract void Play(AudioSource audio);
}
