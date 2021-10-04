using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SnappingPoint : MonoBehaviour
{
    public enum SnapType
    {
        Vertical,
        Horizontal
    }

    public SnapType snapType;
    Buildable buildable;
    private void Awake()
    {
        buildable = GetComponentInParent<Buildable>();
        SphereCollider c = GetComponent<SphereCollider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            SnappingPoint p = other.GetComponent<SnappingPoint>();
            if(p!= null && p.snapType == snapType)
            {
                buildable.SnapPoints(transform, other.transform);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            SnappingPoint p = other.GetComponent<SnappingPoint>();
            if (p != null && p.snapType == snapType)
            {
                buildable.StopSnap(transform, other.transform);
            }
        }
    }
}
