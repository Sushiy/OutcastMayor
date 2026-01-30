using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimationEventCatcher : MonoBehaviour
{
    public UnityEvent<string> onToolAnimationEvent;

    public UnityEvent OnStep;
    public UnityEvent OnJump;

    public void OnToolAnimationEvent(string _value)
    {
        onToolAnimationEvent?.Invoke(_value);
    }

    public void SwingStart()
    {
        
    }
    public void SwingEnd()
    {
        
    }

    public void Step()
    {
        //Analyze Ground!
        OnStep?.Invoke();
    }

    public void Jump()
    {
        //Analyze Ground!
        OnJump?.Invoke();
    }
}
