using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioClip : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayRandomClip(AudioClip[] clips)
    {
        int i = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[i]);
    }

    public void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

}
