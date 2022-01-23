using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimationEventCatcher : MonoBehaviour
{
    public UnityEvent OnSwingStart;
    public UnityEvent OnSwingEnd;

    public UnityEvent OnStep;

    public void SwingStart()
    {
        OnSwingStart?.Invoke();
    }

    public void SwingEnd()
    {
        OnSwingEnd?.Invoke();
    }

    public void Step()
    {
        //Analyze Ground!
        OnStep?.Invoke();
    }
}
