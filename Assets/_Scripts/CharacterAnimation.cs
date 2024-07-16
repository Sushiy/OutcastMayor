using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    //Parameters
    private int tJumpHash = Animator.StringToHash("tJump");
    private int tSwingHash = Animator.StringToHash("tSwing");
    private int bIsRunningHash = Animator.StringToHash("bIsRunning");
    private int bIsSleepingHash = Animator.StringToHash("bIsSleeping");
    private int fSpeedForwardHash = Animator.StringToHash("fSpeedForward");
    private int FSpeedSideHash = Animator.StringToHash("fSpeedSide");
    private int iCarryStateHash = Animator.StringToHash("iCarryState");

    //Public Methods:
    public void SetJump()
    {
        _animator.SetTrigger(tJumpHash);
    }
    public void SetRunning(bool value)
    {
        _animator.SetBool(bIsRunningHash, value);
    }
    public void SetSleeping(bool value)
    {
        _animator.SetBool(bIsSleepingHash, value);
    }
    public void SetSpeedForward(float value)
    {
        _animator.SetFloat(fSpeedForwardHash, value);
    }
    public void SetSpeedSide(float value)
    {
        _animator.SetFloat(FSpeedSideHash, value);
    }

    public void SetSwing()
    {
        _animator.SetTrigger(tSwingHash);
    }

    /// <summary>
    /// Carry animation state: 0 means nothing, 1 means carry
    /// </summary>
    /// <param name="i"></param>
    public void SetCarryState(int i)
    {
        _animator.SetInteger(iCarryStateHash, i);
    }
}
