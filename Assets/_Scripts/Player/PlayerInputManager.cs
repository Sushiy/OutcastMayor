using OutcastMayor.UI;
using System;
using UnityEngine;
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

        [SerializeField]
        private InputActions inputActions;

        Vector2 moveInput;
        Vector2 lookInput;
        public PlayerMovement movement;

        private bool interactPressed = false;
        [SerializeField]
        private Interactor interactor;

        private bool inventoryPressed = false;
        private bool secondaryPressed = false;
        private bool tertiaryPressed = false;

        private bool controlDown = false;
        private bool alternateDown = false;
        private bool primaryPressed = false;

        public Action onPrimaryPressed;
        public Action onSecondaryPressed;
        public Action onTertiaryPressed;
        public Action onInteractPressed;
        public Action onInventoryPressed;
        public Action onRotationModePressed;
        public Action onBuildModePressed;

        [SerializeField]
        private Player player;
        [SerializeField]
        private bool topDownBuilding = false;

        [Header("Raycast")]
        [SerializeField]
        private LayerMask interactRaycastLayerMask;
        [SerializeField]
        private Transform raycastOrigin;
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

            inputActions.Player.Look.performed += OnLookPerformed;
            inputActions.Player.Look.canceled += OnLookCanceled;

            inputActions.Player.Jump.performed += OnJump;

            inputActions.Player.Interact.performed += OnInteract;

            inputActions.Player.Secondary.performed += OnSecondary;

            inputActions.Player.Inventory.performed += OnInventory;

            inputActions.Player.Primary.performed += OnPrimary;

            inputActions.Player.Item1.performed += OnItem1Key;

            inputActions.Player.Item2.performed += OnItem2Key;

            inputActions.Player.Item3.performed += OnItem3Key;

            inputActions.Player.Item4.performed += OnItem4Key;

            inputActions.Player.Position.performed += OnPosition;
            inputActions.Player.Position.canceled += OnPositionCanceled;

            inputActions.Player.Tertiary.performed += OnTertiary;

            inputActions.Player.Ctrl.performed += OnControlPerformed;
            inputActions.Player.Ctrl.canceled += OnControlCanceled;  

            inputActions.Player.ToolMenu.performed += OnToolMenu;

            //BuildMode specific stuff
            inputActions.BuildMode.Rotate.performed += OnRotate; 

            inputActions.BuildMode.RotationMode.performed += OnRotationMode; 

            inputActions.BuildMode.Alternate.performed += OnAlternatePerformed;
            inputActions.BuildMode.Alternate.canceled += OnAlternateCanceled;
            
            print("[PlayerInputManager] Awake completed");

        }

        void OnDestroy()
        {

            inputActions = new InputActions();
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;

            inputActions.Player.Look.performed -= OnLookPerformed;
            inputActions.Player.Look.canceled -= OnLookCanceled;

            inputActions.Player.Interact.performed -= OnInteract;

            inputActions.Player.Jump.performed -= OnJump;
            
            inputActions.Player.Secondary.performed -= OnSecondary;

            inputActions.Player.Inventory.performed -= OnInventory;

            inputActions.Player.Primary.performed -= OnPrimary;

            inputActions.Player.Item1.performed -= OnItem1Key;

            inputActions.Player.Item2.performed -= OnItem2Key;
            
            inputActions.Player.Item3.performed -= OnItem3Key;
            
            inputActions.Player.Item4.performed -= OnItem4Key;

            inputActions.Player.Position.performed -= OnPosition;
            inputActions.Player.Position.canceled -= OnPositionCanceled;

            inputActions.Player.Tertiary.performed -= OnTertiary;

            inputActions.Player.Ctrl.performed -= OnControlPerformed;
            inputActions.Player.Ctrl.canceled -= OnControlCanceled;

            inputActions.Player.ToolMenu.performed -= OnToolMenu;

            //BuildMode specific stuff
            inputActions.BuildMode.Rotate.performed += OnRotate;
            inputActions.BuildMode.Rotate.canceled += OnRotate;  

            inputActions.BuildMode.RotationMode.performed -= OnRotationMode;

            inputActions.BuildMode.Alternate.performed += OnAlternatePerformed;
            inputActions.BuildMode.Alternate.canceled += OnAlternateCanceled;
        }

        void Update()
        {
            if(!player.PlayerToolManager)
            {
                print("NO playertoolmanager");
                return;
            }
            if(player.PlayerToolManager.OnToolRaycast(raycastOrigin.position, raycastOrigin.forward))
            {
                //If this tool handles its own raycast, do that
            }
            else
            {
                //Otherwise pass the raycast on to the interactor
                raycastHit = Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hitInfo, 10.0f, interactRaycastLayerMask);
                interactor.ProcessRayCast(raycastHit, hitInfo);
            }
            //Debug.DrawLine(rayCastOrigin.position, hitInfo.point);

        }

        public void SetBuildMode(bool _active)
        {
            if(_active)
                inputActions.BuildMode.Enable();
            else
                inputActions.BuildMode.Disable();
        }

#region CallbackMethdods
        void OnMovePerformed(CallbackContext c)
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
        void OnMoveCanceled(CallbackContext c)
        {
            moveInput = Vector2.zero;
            movement.Move(moveInput);
        }

        void OnLookPerformed(CallbackContext c)
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

        void OnLookCanceled(CallbackContext c)
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

        void OnInteract(CallbackContext value)
        {
            if (!UIManager.IsUIOpen)
            {
                print("Interact performed");  
                interactor.Interact();
            }
        }
        void OnSecondary(CallbackContext value)
        {
            secondaryPressed = value.performed;
            if (secondaryPressed)
            {
                player.PlayerToolManager.ToolSecondary();
            }
        }

        void OnInventory(CallbackContext value)
        {
            inventoryPressed = value.performed;
            if (inventoryPressed)
            {
                UIManager.Instance.ToggleInventory();
            }
        }

        void OnPrimary(CallbackContext value)
        {
            primaryPressed = value.performed;
            if (primaryPressed && (!UIManager.IsUIOpen))
            {
                player.PlayerToolManager.ToolPrimary();
            }
        }

        void OnItem1Key(CallbackContext value)
        {
            if (value.performed)
            {
                player.HoldItem(0);
            }
        }
        void OnItem2Key(CallbackContext value)
        {
            if (value.performed)
            {
                player.HoldItem(1);
            }
        }
        void OnItem3Key(CallbackContext value)
        {
            if (value.performed)
            {
                player.HoldItem(2);
            }
        }
        void OnItem4Key(CallbackContext value)
        {
            if (value.performed)
            {
                player.HoldItem(3);
            }
        }

        void OnToolMenu(CallbackContext value)
        {
            if(value.performed)
            {
                player.PlayerToolManager.ToolMenu();
            }
        }

        float rotateValue;
        void OnRotate(CallbackContext value)
        {
            float v = value.ReadValue<float>();
            if ((v != 0 && !value.performed) || UIManager.IsUIOpen)
                return;
            rotateValue = v;
            if (alternateDown)
                player.PlayerToolManager.ToolAlternate(rotateValue);
            else
                player.PlayerToolManager.ToolRotate(rotateValue, controlDown);
        }

         void OnRotationMode(CallbackContext value)
        {
            if (value.performed)
            {
                onRotationModePressed?.Invoke();
            }
        }

        
        void OnTertiary(CallbackContext value)
        {
            tertiaryPressed = value.performed;
            if (tertiaryPressed)
            {
                player.PlayerToolManager.ToolTertiary();
            }
        }

        void OnControlPerformed(CallbackContext value)
        {
            if (!controlDown && value.performed)
                controlDown = true;
        }
        void OnControlCanceled(CallbackContext value)
        {
            if (controlDown && value.canceled)
                controlDown = false;
        }

        void OnAlternatePerformed(CallbackContext value)
        {
            if (!alternateDown)
                alternateDown = true;
        }
        void OnAlternateCanceled(CallbackContext value)
        {
            if (alternateDown)
                alternateDown = false;
        }
        Vector2 mousePosition;
        void OnPosition(CallbackContext c)
        {
            mousePosition = c.ReadValue<Vector2>();
        }

        void OnPositionCanceled(CallbackContext c)
        {
            mousePosition = Vector2.zero;
        }
#endregion

    
    }

}