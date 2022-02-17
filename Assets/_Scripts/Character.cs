using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Statemachine
{
    protected Inventory inventory;
    public Inventory Inventory => inventory;
    protected Interactor interactor;
    public Interactor Interactor => interactor;
    protected CharacterAnimation characterAnimation;
    public CharacterAnimation CharacterAnimation => characterAnimation;
    protected IMovement movement;
    public IMovement Movement => movement;

    public bool isSleeping
    {
        protected set;
        get;
    }

    public Action<Character> OnStopSleeping;

    protected virtual void Awake()
    {
        inventory = GetComponent<Inventory>();
        interactor = GetComponent<Interactor>();
        interactor.SetParentCharacter(this);
        characterAnimation = GetComponent<CharacterAnimation>();
        movement = GetComponent<IMovement>();
    }

    public virtual void Sleep()
    {
        isSleeping = true;
        characterAnimation.SetSleeping(true);
    }

    public virtual void WakeUp()
    {
        isSleeping = false;
        characterAnimation.SetSleeping(false);
        OnStopSleeping?.Invoke(this);
    }
}