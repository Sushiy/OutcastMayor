using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSFXManager : MonoBehaviour
{
    RandomAudioClip randomAudioClip;

    [SerializeField]
    private AudioClip[] stepSFX;

    // Start is called before the first frame update
    void Start()
    {
        randomAudioClip = GetComponent<RandomAudioClip>();   
    }

    public void OnStep()
    {
        //Analyze Ground

        randomAudioClip.PlayRandomClip(stepSFX);
    }
}
