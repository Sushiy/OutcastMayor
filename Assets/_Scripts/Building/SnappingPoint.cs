using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SnappingPoint : MonoBehaviour
{
    public enum SnapType
    {
        Vertical = 0,
        Horizontal = 1
    }

    public int status = 0;

    private Color[] gizmoColors =
    {
        Color.red,
        Color.yellow,
        Color.blue,
        Color.green
    };

    public SnapType snapType;
    public Buildable buildable
    {
        private set;
        get;
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

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
            UpgradeStatus(1);
            SnappingPoint p = other.GetComponent<SnappingPoint>();
            if(p!= null)
            {
                UpgradeStatus(2);
                if (p.snapType == snapType)
                {
                    UpgradeStatus(3);
                    buildable.StartSnapping(this, p);
                }
            }
        }
    }

    void UpgradeStatus(int newStatus)
    {
        if(newStatus > status)
        {
            status = newStatus;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            SnappingPoint p = other.GetComponent<SnappingPoint>();
            if (p != null && p.snapType == snapType)
            {
                buildable.StopSnapping(this, p);
            }
            status = 0;
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = gizmoColors[status];
        //Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
