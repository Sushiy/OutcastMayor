using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SnappingPoint : MonoBehaviour
{
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
            buildable.SnapPoints(transform, other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
            buildable.StopSnap(transform, other.transform);
    }
}
