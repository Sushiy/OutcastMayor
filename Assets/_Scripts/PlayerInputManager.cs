using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    Vector2 moveInput;
    Vector2 lookInput;
    public PlayerMovement movement;
 
    public void OnMove(CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
        if (!inventoryView.Visible)
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
        if(!inventoryView.Visible)
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
        if (interactPressed && !inventoryView.Visible)
        {
            interactor.Interact();
        }
    }

    bool inventoryPressed = false;
    public InventoryView inventoryView;
    public void OnInventory(CallbackContext value)
    {
        inventoryPressed = value.performed;
        if(inventoryPressed)
        {
            if (inventoryView.Visible)
            {
                inventoryView.Hide();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                inventoryView.Show();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    bool firePressed = false;

    public void OnFire(CallbackContext value)
    {
        firePressed = value.performed;
        //???
    }
}
