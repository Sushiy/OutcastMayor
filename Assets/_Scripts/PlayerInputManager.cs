using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    Vector2 moveInput;
    Vector2 lookInput;
    public PlayerMovement movement;

    bool interactPressed = false;
    public Interactor interactor;

    bool inventoryPressed = false;

    public BuildingMode buildingMode;

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

    public void OnInteract(CallbackContext value)
    {
        interactPressed = value.performed;
        if (interactPressed && !UIManager.IsUIOpen)
        {
            interactor.Interact();
        }
    }

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
        if(firePressed)
        {
            if(buildingMode.isActive)
            {
                buildingMode.Build();
            }
        }
    }

    public void OnBuildMode(CallbackContext value)
    {
        if(value.performed)
        {
            if (!buildingMode.isActive)
                buildingMode.EnterBuildMode();
            else
                buildingMode.EndBuildMode();
        }
    }

    float rotateValue;
    public void OnRotate(CallbackContext value)
    {
        rotateValue = value.ReadValue<float>();
        print(rotateValue);
        if(buildingMode.isActive)
        {
            buildingMode.Rotate(rotateValue);
        }
    }


    public bool raycastHit
    {
        private set;
        get;
    }
    private RaycastHit hitInfo;
    public RaycastHit HitInfo
    {
        get
        {
            return hitInfo;
        }
    }

    public LayerMask raycastLayerMask;
    public Transform rayCastOrigin;

    public void Update()
    {
        raycastHit = Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hitInfo, 10.0f, raycastLayerMask);
        interactor.ProcessRayCast(raycastHit, hitInfo);
        if(buildingMode.isActive)
        {
            buildingMode.ProcessRayCast(raycastHit, hitInfo);
        }
        Debug.DrawLine(rayCastOrigin.position, hitInfo.point);
    }
}
