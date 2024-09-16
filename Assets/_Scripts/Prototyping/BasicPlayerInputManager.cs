using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace OutcastMayor
{
    public class BasicPlayerInputManager : MonoBehaviour
    {
        public bool IsPointerOverUI
        {
            get
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            }
        }

        InputActions inputActions;

        Vector2 moveInput;
        public Vector2 MoveInput => moveInput;
        Vector2 lookInput;
        public Vector2 LookIput => lookInput;

        public bool primaryDown = false;
        public Action onPrimaryPerformed;
        public bool secondaryDown = false;
        public Action onSecondaryPerformed;

        private bool interactDown = false;
        public Action onInteractPerformed;

        private bool inventoryPressed = false;
        public Action onInventoryPressed;
        private bool demolishPressed = false;
        public Action onDemolishPressed;

        public Action on1Pressed;
        public Action on2Pressed;
        public Action on3Pressed;
        public Action on4Pressed;

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

            inputActions.Player.Enable();

            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

            inputActions.Player.Look.performed += OnLook;
            inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

            inputActions.Player.Interact.performed += OnInteract;
            inputActions.Player.Interact.canceled += OnInteractReleased;

            inputActions.Player.Primary.performed += OnPrimary;
            inputActions.Player.Primary.canceled += OnPrimaryRelease;
            
            inputActions.Player.Secondary.performed += OnSecondary;
            inputActions.Player.Secondary.canceled += OnSecondaryRelease;

            inputActions.Player.Position.performed += OnPosition;

            inputActions.Player.Item1.performed += OnItem1Key;
            inputActions.Player.Item2.performed += OnItem2Key;
            inputActions.Player.Item3.performed += OnItem3Key;
            inputActions.Player.Item4.performed += OnItem4Key;
        }

        public void OnMove(CallbackContext c)
        {
            moveInput = c.ReadValue<Vector2>();
        }
        public void OnLook(CallbackContext c)
        {
            lookInput = c.ReadValue<Vector2>();
        }

        public void OnInteract(CallbackContext value)
        {
            interactDown = true;
            onInteractPerformed?.Invoke();
        }
        public void OnInteractReleased(CallbackContext value)
        {
            interactDown = false;
        }

        public void OnInventory(CallbackContext value)
        {
            inventoryPressed = value.performed;
        }

        public void OnPrimary(CallbackContext value)
        {
            primaryDown = true;
            onPrimaryPerformed?.Invoke();
        }
        public void OnPrimaryRelease(CallbackContext value)
        {
            primaryDown = false;
        }

        public void OnSecondary(CallbackContext value)
        {
            secondaryDown = true;
            onSecondaryPerformed?.Invoke();
        }
        public void OnSecondaryRelease(CallbackContext value)
        {
            secondaryDown = false;
        }

        public void OnItem1Key(CallbackContext value)
        {
            on1Pressed?.Invoke();
        }
        public void OnItem2Key(CallbackContext value)
        {
            on2Pressed?.Invoke();
        }
        public void OnItem3Key(CallbackContext value)
        {
            on3Pressed?.Invoke();
        }
        public void OnItem4Key(CallbackContext value)
        {
            on4Pressed?.Invoke();
        }

        public void OnBuildMode(CallbackContext value)
        {
            if (value.performed)
            {
                
            }
        }

        public void OnZoningMode(CallbackContext value)
        {
            if (value.performed)
            {
                
            }
        }

        float rotateValue;
        public void OnRotate(CallbackContext value)
        {
            float v = value.ReadValue<float>();
            if (v != 0 && !value.performed)
                return;
            rotateValue = v;
        }

        float alternateValue;
        public void OnAlternate(CallbackContext value)
        {
            float v = value.ReadValue<float>();
            if (v != 0 && !value.performed)
                return;
            alternateValue = v;
        }
        public void OnDemolish(CallbackContext value)
        {
            demolishPressed = value.performed;
        }

        Vector2 mousePosition;
        public Vector2 MousePosition => mousePosition;
        public void OnPosition(CallbackContext c)
        {
            mousePosition = c.ReadValue<Vector2>();
        }

        public void Update()
        {
            Ray ray;
            ray = Camera.main.ScreenPointToRay(mousePosition);
            raycastHit = Physics.Raycast(ray, out hitInfo, 100.0f, buildRaycastLayerMask);
        }
    }
}
