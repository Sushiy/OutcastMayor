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

    private bool interactPressed = false;
    [SerializeField]
    private Interactor interactor;

    private bool inventoryPressed = false;
    private bool secondaryPressed = false;
    private bool demolishPressed = false;

    private bool firePressed = false;

    [SerializeField]
    private BuildingMode buildingMode;
    [SerializeField]
    private bool topDownBuilding = false;

    [Header("Raycast")]
    [SerializeField]
    private LayerMask buildRaycastLayerMask;
    [SerializeField]
    private LayerMask interactRaycastLayerMask;
    [SerializeField]
    private Transform rayCastOrigin;
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

    public void OnMove(CallbackContext c)
    {
        moveInput = c.ReadValue<Vector2>();
        if (!UIManager.IsUIOpen && CameraController.ActiveCamera != CameraController.CameraType.Dialogue)
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
        if (UIManager.IsUIOpen || CameraController.ActiveCamera != CameraController.CameraType.Standard)
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
        print("Interact");
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
            UIManager.Instance.ToggleBuildingView();
        }
    }

    public void OnInventory(CallbackContext value)
    {
        inventoryPressed = value.performed;
        if(inventoryPressed)
        {
            UIManager.Instance.ToggleInventory();
        }
    }

    public void OnFire(CallbackContext value)
    {
        firePressed = value.performed;
        if(firePressed && (!UIManager.IsUIOpen || (buildingMode.isActive && topDownBuilding && !IsPointerOverUI)))
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
            {
                buildingMode.EnterBuildMode();
                if(topDownBuilding)
                {
                    UIManager.forceCursor = true;
                    UIManager.ShowCursor();
                    CameraController.ChangeToTopDownCamera();
                }
            }
            else
            {
                buildingMode.ExitBuildMode();
                if(topDownBuilding)
                {
                    UIManager.forceCursor = false;
                    UIManager.HideCursor();
                    CameraController.ChangeToStandardCamera();
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

    float alternateValue;
    public void OnAlternate(CallbackContext value)
    {
        float v = value.ReadValue<float>();
        if (v != 0 && !value.performed)
            return;
        alternateValue = v;
        //print(alternateValue + ";" + value.performed.ToString());
        if (buildingMode.isActive)
        {
            buildingMode.Alternate(alternateValue);
        }
    }
    public void OnDemolish(CallbackContext value)
    {
        demolishPressed = value.performed;
        if (demolishPressed)
        {
            if(interactor.hoveredInteractable is Construction)
            {
                ((Construction)interactor.hoveredInteractable).Destroy();
                interactor.hoveredInteractable.OnEndHover(interactor);
            }
        }
    }

    Vector2 mousePosition;
    public void OnPosition(CallbackContext c)
    {
        mousePosition = c.ReadValue<Vector2>();
    }

    public void Update()
    {
        if(buildingMode.isActive)
        {
            Ray ray;
            if(topDownBuilding)
            {
                ray = Camera.main.ScreenPointToRay(mousePosition);
            }
            else
            {
                ray = new Ray(rayCastOrigin.position, rayCastOrigin.forward);
            }
            buildingMode.ProcessRayCast(raycastHit, ray, hitInfo);
            raycastHit = Physics.Raycast(ray, out hitInfo, 10.0f, buildRaycastLayerMask);
        }
        else
        {
            raycastHit = Physics.Raycast(rayCastOrigin.position, rayCastOrigin.forward, out hitInfo, 10.0f, interactRaycastLayerMask);
            interactor.ProcessRayCast(raycastHit, hitInfo);
        }
        //Debug.DrawLine(rayCastOrigin.position, hitInfo.point);

    }
}
