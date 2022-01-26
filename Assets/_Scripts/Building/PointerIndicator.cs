using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerIndicator : MonoBehaviour
{
    Animator animator;
    int hashActive = Animator.StringToHash("bActive");

    bool visible = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        SetVisible(false);
    }

    public void SetVisible(bool value)
    {
        if(visible != value)
        {
            visible = value;
            animator.SetBool(hashActive, visible);
        }
    }
}
