using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class ZoningMode
{
    public bool isActive
    {
        private set;
        get;
    }

    [SerializeField]
    Player player;

    [SerializeField]
    ZoningController zonePrefab;

    [SerializeField]
    private Transform zoneParent;

    private Vector3 buildPosition;
    private Vector3 rayCastPosition;
    Ray surfaceNormal;
    bool raycastHit;
    [SerializeField]
    private float raycastMaxDistance = 5.0f;
    private Quaternion buildRotation;
    [SerializeField]
    private float rotateSpeed = 10.0f;
    public float buildAngle = 0;

    [SerializeField]
    private PointerIndicator indicator;

    private InputActionMap buildActionMap;

    ZoningController editingZone;

    public void EnterZoningMode()
    {
        isActive = true;
        buildActionMap = player.GetComponent<PlayerInput>().actions.FindActionMap("Buildmode");
        buildActionMap.Enable();
    }

    public void ExitZoningMode()
    {
        isActive = false;
        buildActionMap.Disable();
    }

    public void PlaceZone()
    {
        if(!editingZone)
        {
            editingZone = GameObject.Instantiate<ZoningController>(zonePrefab, rayCastPosition, buildRotation, zoneParent);
        }
        else
        {
            editingZone.UpdateDragHandlePosition(rayCastPosition);
            editingZone.InitStockpile(editingZone.GetComponent<StockpileZone>());
            editingZone.enabled = false;
            editingZone = null;
        }
    }
    public void ProcessRayCast(bool raycastHit, Ray ray, RaycastHit hitInfo)
    {
        this.raycastHit = raycastHit;
        if (raycastHit)
        {
            rayCastPosition = hitInfo.point;
            surfaceNormal.origin = hitInfo.point;
            surfaceNormal.direction = hitInfo.normal;
            if (indicator)
            {
                indicator.transform.position = rayCastPosition;
                indicator.transform.rotation = Quaternion.LookRotation(surfaceNormal.direction);
                indicator.SetVisible(true);
            }

            if (isActive && editingZone != null)
            {
                editingZone.UpdateDragHandlePosition(rayCastPosition);
            }
        }
        else
        {
            if (indicator)
            {
                indicator.SetVisible(false);
            }
            rayCastPosition = ray.origin + ray.direction * raycastMaxDistance;
        }
    }

    private void Update()
    {
    }
    public void Rotate(float rotateInput)
    {
        if (rotateInput != 0)
        {
            //Debug.Log("Rotate:" + Mathf.Sign(rotateInput));
            buildAngle += Mathf.Sign(rotateInput) * rotateSpeed;
            buildRotation = Quaternion.Euler(0, buildAngle, 0);
        }
    }
}
