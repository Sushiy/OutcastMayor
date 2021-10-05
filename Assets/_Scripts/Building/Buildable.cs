using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buildable : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;

    SnappingPoint[] snappingPoints;
    public BoxCollider buildCollider;

    public Room room;

    private void Awake()
    {
        snappingPoints = GetComponentsInChildren<SnappingPoint>();
    }

    public void SetGhostMode(Material ghostMaterial)
    {
        SetMaterials(ghostMaterial);
        gameObject.name = "Ghost";
        SetLayerForAllColliders(2);
    }

    public void SetSensorMode(Material sensorMaterial)
    {
        SetMaterials(sensorMaterial);
        SetInvisible();
        gameObject.name = "Sensor";
        SetLayerForAllColliders(6);
    }

    public void SetBuildMode(Buildable snappedToObject)
    {
        CheckForRoom(snappedToObject);
    }

    public void SetInvisible()
    {
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            meshRenderers[m].enabled = false;
        }
    }

    public void SetMaterials(Material material)
    {
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            Material[] materials = meshRenderers[m].materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            meshRenderers[m].materials = materials;
        }
    }

    public void SetLayerForAllColliders(int layer)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = layer;
        }
    }

    public SnappingPoint ownSnapReference;
    public SnappingPoint otherSnapReference;

    public void SnapPoints(SnappingPoint own, SnappingPoint other)
    {
        ownSnapReference = own;
        otherSnapReference = other;
    }

    public void StopSnap(SnappingPoint own, SnappingPoint other)
    {
        if(ownSnapReference == own && otherSnapReference == other)
        {
            ownSnapReference = null;
            otherSnapReference = null;
        }
    }

    public virtual void CheckForRoom(Buildable snappedTo)
    {

    }
}
