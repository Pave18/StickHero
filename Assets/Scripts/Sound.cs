using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool loop = false;

    private AudioSource audioSource;
    

    public void SetSource(AudioSource source)
    {
        audioSource = source;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }


    public AudioSource GetSource()
    {
        return audioSource;
    }


    public void Play()
    {
        audioSource.Play();
    }


    public void Stop()
    {
        audioSource.Stop();
    }
}