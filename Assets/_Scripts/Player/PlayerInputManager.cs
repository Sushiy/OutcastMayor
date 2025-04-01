using OutcastMayor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace OutcastMayor
{
    public class PlayerInputManager : MonoBehaviour
    {
        public bool IsPointerOverUI
        {
            get
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            }
        }

        public InputActions inputActions;

        Vector2 moveInput;
        Vector2 lookInput;
        public PlayerMovement movement;

        private bool interactPressed = false;
        [SerializeField]
        private Interactor interactor;

        private bool inventoryPressed = false;
        private bool secondaryPressed = false;
        private bool demolishPressed = false;

        private bool keyPressed = false;

        [SerializeField]
        private Player player;
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

        void Awake()
        {

            inputActions = new InputActions();
            //Activate Input Maps
            inputActions.Player.Enable();
            //Player input map
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            inputActions.Player.Look.performed += OnLook;
            inputActions.Player.Look.canceled += OnLookCanceled;

            inputActions.Player.Jump.performed += OnJump;
            inputActions.Player.Jump.canceled += OnJumpCanceled;

            inputActions.Player.Interact.performed += OnInteract;
            inputActions.Player.Interact.canceled += OnInteract;
            
            inputActions.Player.Secondary.performed += OnSecondary;
            inputActions.Player.Secondary.canceled += OnSecondary;

            inputActions.Player.Inventory.performed += OnInventory;
            inputActions.Player.Inventory.canceled += OnInventory;

            inputActions.Player.Primary.performed += OnPrimary;
            inputActions.Player.Primary.canceled += OnPrimary;

            inputActions.Player.Item1.performed += OnItem1Key;
            inputActions.Player.Item1.canceled += OnItem1Key;

            inputActions.Player.Item2.performed += OnItem2Key;
            inputActions.Player.Item2.canceled += OnItem2Key;
            
            inputActions.Player.Item3.performed += OnItem3Key;
            inputActions.Player.Item3.canceled += OnItem3Key;
            
            inputActions.Player.Item4.performed += OnItem4Key;
            inputActions.Player.Item4.canceled += OnItem4Key;

            inputActions.Player.EnterBuildMode.performed += OnBuildMode;
            inputActions.Player.EnterBuildMode.canceled += OnBuildMode;

            inputActions.Player.EnterZoneMode.performed += OnZoningMode;
            inputActions.Player.EnterZoneMode.canceled += OnZoningMode;

            inputActions.Player.Position.performed += OnPosition;
            inputActions.Player.Position.canceled += OnPositionCanceled;

            
            //BuildMode
            inputActions.Buildmode.Rotate.performed += OnRotate;
            inputActions.Buildmode.Rotate.canceled += OnRotate;

            inputActions.Buildmode.ChangeVariant.performed += OnChangeVariant;
            inputActions.Buildmode.ChangeVariant.canceled += OnChangeVariant;

            inputActions.Buildmode.Destroy.performed += OnDemolish;
            inputActions.Buildmode.Destroy.canceled += OnDemolish;

            inputActions.Buildmode.OpenBuildmenu.performed += OnSecondary;
            inputActions.Buildmode.OpenBuildmenu.canceled += OnSecondary;

        }

        void OnDestroy()
        {

            inputActions = new InputActions();
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.Player.Look.performed -= OnLook;
            inputActions.Player.Look.canceled -= OnLookCanceled;
            inputActions.Player.Interact.performed -= OnInteract;
            inputActions.Player.Interact.canceled -= OnInteract;
            inputActions.Player.Jump.performed -= OnJump;
            inputActions.Player.Jump.canceled -= OnJumpCanceled;
            
            inputActions.Player.Secondary.performed -= OnSecondary;
            inputActions.Player.Secondary.canceled -= OnSecondary;

            inputActions.Player.Inventory.performed -= OnInventory;
            inputActions.Player.Inventory.canceled -= OnInventory;

            inputActions.Player.Primary.performed -= OnPrimary;
            inputActions.Player.Primary.canceled -= OnPrimary;

            inputActions.Player.Item1.performed -= OnItem1Key;
            inputActions.Player.Item1.canceled -= OnItem1Key;

            inputActions.Player.Item2.performed -= OnItem2Key;
            inputActions.Player.Item2.canceled -= OnItem2Key;
            
            inputActions.Player.Item3.performed -= OnItem3Key;
            inputActions.Player.Item3.canceled -= OnItem3Key;
            
            inputActions.Player.Item4.performed -= OnItem4Key;
            inputActions.Player.Item4.canceled -= OnItem4Key;

            inputActions.Player.EnterBuildMode.performed -= OnBuildMode;
            inputActions.Player.EnterBuildMode.canceled -= OnBuildMode;

            inputActions.Player.EnterZoneMode.performed -= OnZoningMode;
            inputActions.Player.EnterZoneMode.canceled -= OnZoningMode;

            inputActions.Player.Position.performed -= OnPosition;
            inputActions.Player.Position.canceled -= OnPositionCanceled;

            
            //BuildMode
            inputActions.Buildmode.Rotate.performed -= OnRotate;
            inputActions.Buildmode.Rotate.canceled -= OnRotate;

            inputActions.Buildmode.ChangeVariant.performed -= OnChangeVariant;
            inputActions.Buildmode.ChangeVariant.canceled -= OnChangeVariant;

            inputActions.Buildmode.Destroy.performed -= OnDemolish;
            inputActions.Buildmode.Destroy.canceled -= OnDemolish;

            inputActions.Buildmode.OpenBuildmenu.performed -= OnSecondary;
            inputActions.Buildmode.OpenBuildmenu.canceled -= OnSecondary;

        }


        public void OnMovePerformed(CallbackContext c)
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
        public void OnMoveCanceled(CallbackContext c)
        {
            moveInput = Vector2.zero;
            movement.Move(moveInput);
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

        public void OnLookCanceled(CallbackContext c)
        {
            lookInput = Vector2.zero;
            movement.Look(lookInput);
        }

        void OnJump(CallbackContext c)
        {
            if(c.performed)
            {
                print("jump");
                movement.Jump();
            }
        }

        void OnJumpCanceled(CallbackContext c)
        {

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
            if (secondaryPressed && player.BuildingMode.isActive)
            {
                UIManager.Instance.ToggleBuildingView();
            }
        }

        public void OnInventory(CallbackContext value)
        {
            inventoryPressed = value.performed;
            if (inventoryPressed)
            {
                UIManager.Instance.ToggleInventory();
            }
        }

        public void OnPrimary(CallbackContext value)
        {
            keyPressed = value.performed;
            if (keyPressed && (!UIManager.IsUIOpen || (player.BuildingMode.isActive && topDownBuilding && !IsPointerOverUI)))
            {
                if (player.BuildingMode.isActive)
                {
                    player.BuildingMode.Build();
                }
                else if (player.ZoningMode.isActive)
                {
                    player.ZoningMode.PlaceZone();
                }
                else
                {
                    Player.Instance.PlayerToolManager.SwingTool();
                }
            }
        }

        public void OnItem1Key(CallbackContext value)
        {
            if (value.performed && !player.BuildingMode.isActive && !player.ZoningMode.isActive)
            {
                player.HoldItem(0);
            }
        }
        public void OnItem2Key(CallbackContext value)
        {
            if (value.performed && !player.BuildingMode.isActive && !player.ZoningMode.isActive)
            {
                player.HoldItem(1);
            }
        }
        public void OnItem3Key(CallbackContext value)
        {
            if (value.performed && !player.BuildingMode.isActive && !player.ZoningMode.isActive)
            {
                player.HoldItem(2);
            }
        }
        public void OnItem4Key(CallbackContext value)
        {
            if (value.performed && !player.BuildingMode.isActive && !player.ZoningMode.isActive)
            {
                player.HoldItem(3);
            }
        }

        public void OnBuildMode(CallbackContext value)
        {
            if (value.performed)
            {
                if (!player.BuildingMode.isActive)
                {
                    player.BuildingMode.EnterBuildMode();
                    if (topDownBuilding)
                    {
                        UIManager.forceCursor = true;
                        UIManager.ShowCursor();
                        CameraController.ChangeToTopDownCamera();
                    }
                }
                else
                {
                    player.BuildingMode.ExitBuildMode();
                    if (topDownBuilding)
                    {
                        UIManager.forceCursor = false;
                        UIManager.HideCursor();
                        CameraController.ChangeToStandardCamera();
                    }
                }
            }
        }

        public void OnZoningMode(CallbackContext value)
        {
            if (value.performed)
            {
                Debug.Log("ZONINGMODE!");
                if (!player.ZoningMode.isActive)
                {
                    player.ZoningMode.EnterZoningMode();
                }
                else
                {
                    player.ZoningMode.ExitZoningMode();
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
            if (player.BuildingMode.isActive)
            {
                player.BuildingMode.Rotate(rotateValue);
            }
            if (player.ZoningMode.isActive)
            {
                player.ZoningMode.Rotate(rotateValue);
            }
        }

        float alternateValue;
        public void OnChangeVariant(CallbackContext value)
        {
            float v = value.ReadValue<float>();
            if (v != 0 && !value.performed)
                return;
            alternateValue = v;
            //print(alternateValue + ";" + value.performed.ToString());
            if (player.BuildingMode.isActive)
            {
                player.BuildingMode.Alternate(alternateValue);
            }
        }
        public void OnDemolish(CallbackContext value)
        {
            demolishPressed = value.performed;
            if (demolishPressed)
            {
                if (interactor.hoveredInteractable is Building.Construction)
                {
                    ((Building.Construction)interactor.hoveredInteractable).Destroy();
                    interactor.hoveredInteractable.OnEndHover(interactor);
                }
            }
        }

        Vector2 mousePosition;
        public void OnPosition(CallbackContext c)
        {
            mousePosition = c.ReadValue<Vector2>();
        }

        public void OnPositionCanceled(CallbackContext c)
        {
            mousePosition = Vector2.zero;
        }

        public void Update()
        {
            if (player.BuildingMode.isActive)
            {
                Ray ray;
                if (topDownBuilding)
                {
                    ray = Camera.main.ScreenPointToRay(mousePosition);
                }
                else
                {
                    ray = new Ray(rayCastOrigin.position, rayCastOrigin.forward);
                }
                player.BuildingMode.ProcessRayCast(raycastHit, ray, hitInfo);
                raycastHit = Physics.Raycast(ray, out hitInfo, 10.0f, buildRaycastLayerMask);
            }
            else if (player.ZoningMode.isActive)
            {
                Ray ray;

                ray = new Ray(rayCastOrigin.position, rayCastOrigin.forward);
                player.ZoningMode.ProcessRayCast(raycastHit, ray, hitInfo);
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

}