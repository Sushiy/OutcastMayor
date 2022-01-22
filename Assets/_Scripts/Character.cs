using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Inventory inventory;
    public Inventory Inventory => inventory;
    protected Interactor interactor;
    public Interactor Interactor => interactor;
    protected CharacterAnimation characterAnimation;
    public CharacterAnimation CharacterAnimation => characterAnimation;
    protected IMovement movement;
    public IMovement Movement => movement;

    protected virtual void Awake()
    {
        inventory = GetComponent<Inventory>();
        interactor = GetComponent<Interactor>();
        interactor.SetParentCharacter(this);
        characterAnimation = GetComponent<CharacterAnimation>();
        movement = GetComponent<IMovement>();
    }
}