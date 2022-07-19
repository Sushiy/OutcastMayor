using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIKController : MonoBehaviour
{
    [SerializeField]
    Transform footBone;

    [SerializeField]
    Transform ikTarget;

    RaycastHit raycastHit;
    bool hitThisFrame = false;

    [Range(0, 1f)]
    public float distanceToGround;

    [SerializeField]
    LayerMask layerMask;

    void Update()
    {
        //Raycast down from foot
        Ray ray = new Ray(footBone.position + Vector3.up, Vector3.down);

        hitThisFrame = false;

        if (Physics.Raycast(ray, out raycastHit, distanceToGround + 1f, layerMask))
        {
            ikTarget.position = raycastHit.point;
            ikTarget.rotation = footBone.rotation;
            hitThisFrame = true;
        }
        else
        {
            ikTarget.position = footBone.position;
            ikTarget.rotation = footBone.rotation;
        }

    }

    private void OnDrawGizmosSelected()
    {
        if(hitThisFrame)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(footBone.position, raycastHit.point);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(footBone.position, Vector3.down * .5f);
        }
    }
}
