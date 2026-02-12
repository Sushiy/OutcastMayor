using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerIndicator : MonoBehaviour
{
    Animator animator;
    int hashActive = Animator.StringToHash("bActive");

    bool visible = false;

    [SerializeField]
    Shapes.Disc xRotationDisc;
    [SerializeField]
    Shapes.Disc xRotationEdge;

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
            xRotationDisc.enabled = visible;
            xRotationEdge.enabled = visible;
        }
    }
    
    public void UpdateYRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(-90, angle, 0);
    }

    public void UpdateXRotation(float angle)
    {
        xRotationDisc.AngRadiansEnd = angle * Mathf.Deg2Rad;
        xRotationEdge.AngRadiansEnd = angle * Mathf.Deg2Rad;
    }
}
