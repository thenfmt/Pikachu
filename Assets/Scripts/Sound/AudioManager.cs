using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{   
    [SerializeField]
    private Sound[] soundList;

    private void Awake()
    {
        foreach(Sound sound in soundList)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void playSound(string name)
    {
        Sound sound = Array.Find(soundList, sound => sound.name == name);
        sound.source.Play();
    }

    public void stopSound(string name)
    {
        Sound sound = Array.Find(soundList, sound => sound.name == name);
        sound.source.Stop();
    }
}
