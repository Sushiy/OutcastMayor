using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    public bool IsPointerOverUI
    {
        get
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
    }

    Vector2 moveInput;
    Vector2 lookInput;
    public PlayerMovement movement;

    bool interactPressed = false;
    public Interactor interactor;

    bool inventoryPressed = false;
    bool secondaryPressed = false;

    public BuildingMode buildingMode;
    public bool topDownBuilding = false;
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
        if (UIManager.IsUIOpen || (topDownBuilding && buildingMode.isActive))
        {
            movement.Look(Vector2.zero);
        }
        else
        {
            movement.Look(lookInput);
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
    public void OnSecondary(CallbackContext value)
    {
        secondaryPressed = value.performed;
        if (secondaryPressed && buildingMode.isActive)
        {
            UIManager.instance.ToggleBuildingView();
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
        if(firePressed && (!UIManager.IsUIOpen || buildingMode.isActive && topDownBuilding && !IsPointerOverUI))
        {
            if(buildingMode.isActive)
            {
                buildingMode.Build();
            }
        }
    }

    public Cinemachine.CinemachineVirtualCamera camOTS;
    public Cinemachine.CinemachineVirtualCamera camTD;

    public void OnBuildMode(CallbackContext value)
    {
        if(value.performed)
        {
            if (!buildingMode.isActive)
            {
                buildingMode.EnterBuildMode();
                if(topDownBuilding)
                {
                    UIManager.forceCursor = true;
                    UIManager.ShowCursor();
                    camOTS.Priority = 0;
                    camTD.Priority = 10;
                }
            }
            else
            {
                buildingMode.EndBuildMode();
                if(topDownBuilding)
                {
                    UIManager.forceCursor = false;
                    UIManager.HideCursor();
                    camOTS.Priority = 10;
                    camTD.Priority = 0;
                }
            }
        }
    }

    float rotateValue;
    public void OnRotate(CallbackContext value)
    {
        float v = value.ReadValue<float>();
        if (v != 0 && !value.performed)
            return;
        rotateValue = v;
        //print(rotateValue + ";" + value.performed.ToString());
        if(buildingMode.isActive)
        {
            buildingMode.Rotate(rotateValue);
        }
    }

    Vector2 mousePosition;
    public void OnPosition(CallbackContext c)
    {
        mousePosition = c.ReadValue<Vector2>();
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
        if(topDownBuilding && buildingMode.isActive)
        {
            raycastHit = Physics.Raycast(Camera.main.ScreenPointToRay(mousePosition), out hitInfo, 10.0f, raycastLayerMask);
        }
        else
        {
            raycastHit = Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hitInfo, 10.0f, raycastLayerMask);
            interactor.ProcessRayCast(raycastHit, hitInfo);
            Debug.DrawLine(rayCastOrigin.position, hitInfo.point);
        }

        if (buildingMode.isActive)
        {
            buildingMode.ProcessRayCast(raycastHit, hitInfo);
        }
    }
}
