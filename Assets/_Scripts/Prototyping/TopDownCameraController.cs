using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutcastMayor
{
    public class TopDownCameraController : MonoBehaviour
    {
        BasicPlayerInputManager inputManager;
        //ApartmentInputSettings inputActions;

        [SerializeField]
        Transform mouseSelectionIndicator;

        [SerializeField]
        float moveSpeed = 1f;
        [SerializeField]
        float horizontalRotationSpeed = 1f;
        [SerializeField]
        float verticalRotationSpeed = 1f;

        Vector3 flatForward;

        void Awake()
        {
            inputManager = GetComponent<BasicPlayerInputManager>();
            Cursor.visible = false;
            mouseSelectionIndicator.parent = null;
        }

        void LateUpdate()
        {           

            if(inputManager.secondaryDown)
            {
                transform.Rotate(Vector3.up, horizontalRotationSpeed * Time.deltaTime * inputManager.LookIput.x, Space.World);

                transform.Rotate(Vector3.right, verticalRotationSpeed * Time.deltaTime * -inputManager.LookIput.y, Space.Self);
                Vector3 euler = transform.rotation.eulerAngles;
                euler.x = Mathf.Clamp(euler.x, 25, 70);
                transform.rotation = Quaternion.Euler(euler);
            }
            else
            {
                // Move cursor
                if(inputManager.raycastHit)
                    mouseSelectionIndicator.position = inputManager.HitInfo.point;
            }

            // Move Camera
            flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            transform.position += flatForward * moveSpeed * Time.deltaTime * inputManager.MoveInput.y + transform.right * moveSpeed * Time.deltaTime * inputManager.MoveInput.x;
        }
    }

}
