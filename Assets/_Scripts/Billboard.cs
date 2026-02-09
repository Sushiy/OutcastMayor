using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    Camera mainCamera;

    public Vector3 positionOffset;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(mainCamera.transform.position + positionOffset, Vector3.up);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
    }
}
