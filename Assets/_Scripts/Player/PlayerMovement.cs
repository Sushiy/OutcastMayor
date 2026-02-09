using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace OutcastMayor
{
    /// <summary>
    /// This class controls both player movement and camera direction (in normal camera mode)
    /// </summary>
    public class PlayerMovement : MonoBehaviour, IMovement
    {
        [Header("Movement Settings")]
        [SerializeField]
        private float moveSpeed = 1.0f;

        [Header("View Settings")]
        [SerializeField]
        private float rotationSpeed = 3.0f;
        [SerializeField]
        private float maxVerticalCamAngle = 340;
        [SerializeField]
        private float minVerticalCamAngle = 40;

        [SerializeField]
        private float camToMoveAdjustSpeed = 2.0f;

        [SerializeField]
        float jumpHeight = 1.0f;

        const float GRAVITY = -9.81f;
        [SerializeField]
        LayerMask groundLayerMask;

        Vector3 movementDelta;
        [SerializeField]
        private Transform rotationTarget;

        private CharacterController characterController;

        private float verticalSpeed;

        private bool isFlying;
        [SerializeField, Sirenix.OdinInspector.ReadOnly]
        private bool isGrounded;
        bool jumpedThisFrame;
        bool jumpPressed;

        #region Input
        Vector2 moveInput;
        Vector2 lookInput;

        public Transform followTransform;

        bool movementLocked = false;

        public void Move(Vector2 move)
        {
            moveInput = move;
        }
        public void Look(Vector2 look)
        {
            lookInput = look;
        }
        #endregion

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.CheckSphere(transform.position +Vector3.down *.2f, .2f, groundLayerMask);
            Player.Instance.CharacterAnimation.SetGrounded(isGrounded);

            if (movementLocked)
            {
                moveInput = Vector2.zero;
                lookInput = Vector2.zero;
            }    
            if (moveInput.sqrMagnitude > 0.0f)
            {
                Player.Instance.CharacterAnimation.SetRunning(true);
                Player.Instance.CharacterAnimation.SetSpeedForward(moveInput.y);
                Player.Instance.CharacterAnimation.SetSpeedSide(moveInput.x);
            }
            else
            {
                Player.Instance.CharacterAnimation.SetRunning(false);
                Player.Instance.CharacterAnimation.SetSpeedForward(0.0f);
                Player.Instance.CharacterAnimation.SetSpeedSide(0.0f);
            }


            //Rotate the Follow Target transform based on the input
            followTransform.rotation *= Quaternion.AngleAxis(lookInput.x * rotationSpeed, Vector3.up);

            #region Vertical Camera Rotation
            followTransform.rotation *= Quaternion.AngleAxis(-lookInput.y * rotationSpeed, Vector3.right);

            //Clamp the Vertical Angles to logical boundaries
            var angles = followTransform.localEulerAngles;
            angles.z = 0;

            var angle = followTransform.localEulerAngles.x;

            //Clamp the Up/Down rotation
            if (angle > 180 && angle < maxVerticalCamAngle)
            {
                angles.x = maxVerticalCamAngle;
            }
            else if (angle < 180 && angle > minVerticalCamAngle)
            {
                angles.x = minVerticalCamAngle;
            }
            followTransform.localEulerAngles = angles;
            #endregion

            //Player gravity     
            if (characterController.isGrounded)
            {
                verticalSpeed = 0.0f;
            }
            if (jumpedThisFrame)
            {
                jumpedThisFrame = false;
                verticalSpeed += Mathf.Sqrt(jumpHeight * -2.0f * GRAVITY) * Time.deltaTime;
            }
            float frameSpeed = moveSpeed * Time.deltaTime;
            verticalSpeed += GRAVITY * Time.deltaTime * Time.deltaTime;

            //If the player is moving, adjust the camera to the movement
            if (IsMoving())
            {
                Player.Instance.WakeUp();
                
                //1.Set the player rotation based on the look transform
                transform.rotation = Quaternion.Euler(0, followTransform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
                followTransform.localEulerAngles = new Vector3(angles.x, 0, 0);

                //Calculate the movementDelta based 
                movementDelta = transform.forward * (moveInput.y * frameSpeed) + transform.right * (moveInput.x * frameSpeed);

                //Rotate the player model to match the movement direction
                Quaternion lookRot = Quaternion.LookRotation(movementDelta, Vector3.up);
                rotationTarget.rotation = Quaternion.Euler(0, lookRot.eulerAngles.y, 0);

                if (moveInput.x != 0)
                {
                    //Slowly adjust the camera to match the players travel direction
                    float adjustAngle = Vector3.Angle(followTransform.forward, rotationTarget.forward) * Time.deltaTime * camToMoveAdjustSpeed * moveInput.x;
                    followTransform.rotation *= Quaternion.Euler(0, adjustAngle, 0);
                }
            }
            //If the player is not moving, allow free orbiting of the camera
            else
            {
                movementDelta = Vector3.zero;
            }
            movementDelta.y = verticalSpeed;
            characterController.Move(movementDelta);

            //Debug Rays
            Debug.DrawRay(transform.position, rotationTarget.forward, Color.blue);
            Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            Debug.DrawRay(transform.position, movementDelta.normalized, Color.green);
        }

        public void ActualJump()
        {
            jumpedThisFrame = true;
        }

        public void Jump()
        {
            if (characterController.isGrounded)
            {
                print("[PlayerMovement] Jump!");
                Player.Instance.WakeUp();
                Player.Instance.CharacterAnimation.SetJump();
            }
            else
                print("[PlayerMovement] Can't Jump: Not grounded");
        }

        public void TeleportTo(Vector3 position)
        {
            characterController.enabled = false;
            transform.position = position;
            characterController.enabled = true;
        }

        public void SnapYRotation(Quaternion rotation)
        {
            rotationTarget.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }

        public void LockMovement(bool locked)
        {
            movementLocked = locked;
        }

        public bool IsMoving()
        {
            return moveInput.x != 0 || moveInput.y != 0;
        }
    }


}