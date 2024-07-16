using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingV2 : MonoBehaviour
{
    Vector3 raycastMousePosition;
    public Transform mouseIndicator;
    
    void Update()
    {
    }
    
    void LateUpdate()
    {
        mouseIndicator.position = raycastMousePosition;
    }
}
