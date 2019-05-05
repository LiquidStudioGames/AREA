using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="/Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent
{
    public AudioClip[] clips;


    //this will be changed to ScriptableVariable form to make sound normalized overall
    public float volume;
    [Range(0, 2)]
    public float pitch;

    public override void Play(AudioSource audio)
    {
        if (clips.Length == 0) return;


    }

}
