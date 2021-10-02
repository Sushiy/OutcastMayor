using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    private void Awake()
    {
    }


    Vector2 moveInput;
    Vector2 lookInput;
    public PlayerMovement movement;
 
    public void OnMove(CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
        if (!UIManager.IsUIOpen)
        {
            movement.Move(moveInput);
        }
        else
        {
            movement.Move(Vector2.zero);
        }
    }
    public void OnLook(CallbackContext c)
    {
        lookInput = c.ReadValue<Vector2>();
        if (!UIManager.IsUIOpen)
        {
            movement.Look(lookInput);
        }
        else
        {
            movement.Look(Vector2.zero);
        }
    }

    bool interactPressed = false;
    public Interactor interactor;

    public void OnInteract(CallbackContext value)
    {
        interactPressed = value.performed;
        if (interactPressed && !UIManager.IsUIOpen)
        {
            interactor.Interact();
        }
    }

    bool inventoryPressed = false;
    public void OnInventory(CallbackContext value)
    {
        inventoryPressed = value.performed;
        if(inventoryPressed)
        {
            UIManager.instance.ToggleInventory();
        }
    }

    bool firePressed = false;

    public void OnFire(CallbackContext value)
    {
        firePressed = value.performed;
        //???
    }
}
