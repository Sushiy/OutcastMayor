using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float rotationSpeed = 3.0f;
    public float maxVertical = 340;
    public float minVertical = 40;

    public float adjustSpeed = 2.0f;

    Vector3 movementDelta;
    public Transform rotationTarget;

    Animator animator;
    int hashRunning = Animator.StringToHash("bIsRunning");
    int hashSpeed = Animator.StringToHash("fSpeed");

    CharacterController characterController;

    private float verticalSpeed;

    #region Input
    Vector2 moveInput;
    Vector2 lookInput;

    public Transform followTransform;

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
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(moveInput.sqrMagnitude > 0.0f)
        {
            animator.SetBool(hashRunning, true);
            animator.SetFloat(hashSpeed, moveInput.magnitude);
        }
        else
        {
            animator.SetBool(hashRunning, false);
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
        if (angle > 180 && angle < maxVertical)
        {
            angles.x = maxVertical;
        }
        else if (angle < 180 && angle > minVertical)
        {
            angles.x = minVertical;
        }
        followTransform.localEulerAngles = angles;
        #endregion
        
        //Player gravity
        if (characterController.isGrounded)
        {
            verticalSpeed = 0.0f;
        }
        float frameSpeed = moveSpeed * Time.deltaTime;
        verticalSpeed -= 9.8f * Time.deltaTime * Time.deltaTime;

        //If the player is not moving, orbit the camera by rotating the player
        if (moveInput.x != 0 || moveInput.y != 0)
        {
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
                float adjustAngle = Vector3.Angle(followTransform.forward, rotationTarget.forward) * Time.deltaTime * adjustSpeed * moveInput.x;
                followTransform.rotation *= Quaternion.Euler(0, adjustAngle, 0);
            }
        }
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
}
