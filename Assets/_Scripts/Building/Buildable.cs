using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buildable : MonoBehaviour
{
    [Header("Placement System")]
    [SerializeField]
    private MeshRenderer[] meshRenderers;
    [SerializeField]
    public BoxCollider buildCollider;
    SnappingPoint[] snappingPoints;

    Construction c;

    public SnappingPoint snappedPointSelf
    {
        private set;
        get;
    }
    public SnappingPoint snappedPointOther
    {
        private set;
        get;
    }

    [Header("Room Recognition")]
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
        SetBlueprintLayer();
        CheckForRoom(snappedToObject);
    }

    private void SetRendererLayers(int i)
    {
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            meshRenderers[m].gameObject.layer = i;
        }
    }

    public void SetDefaultLayer()
    {
        SetRendererLayers(0);
    }

    public void SetBlueprintLayer()
    {
        SetRendererLayers(7);
    }

    private void SetInvisible()
    {
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            meshRenderers[m].enabled = false;
        }
    }

    private void SetMaterials(Material material)
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

    private void SetLayerForAllColliders(int layer)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = layer;
        }
    }

    public void StartSnapping(SnappingPoint own, SnappingPoint other)
    {
        snappedPointSelf = own;
        snappedPointOther = other;
    }

    public void StopSnapping(SnappingPoint own, SnappingPoint other)
    {
        if(snappedPointSelf == own && snappedPointOther == other)
        {
            snappedPointSelf = null;
            snappedPointOther = null;
        }
        else
        {
            Debug.LogWarning("We tried to stop snapping the wrong points.");
        }
    }

    /// <summary>
    /// This method tries to sort Buildables into rooms. Rooms should eventually be used to validate buildings.
    /// For now it just keeps the scene hiearchy a little cleaner
    /// </summary>
    /// <param name="snappedTo"></param>
    public virtual void CheckForRoom(Buildable snappedTo)
    {
        //Proposed Function:
        //1. Check the buildcollider "as trigger" for all overlapping objects
        //2. Step through all of the objects and try to find out if they are from the same room/building
        //3. Add this Object to an existing room OR create a new one.
        //4. If this Object connects multiple rooms, consolidate them into one

        if (snappedTo == null || snappedTo.room == null)
        {
            GameObject g = new GameObject("Room");
            g.transform.parent = transform.parent;
            room = g.AddComponent<Room>();
            transform.parent = room.transform;
        }
        else
        {
            room = snappedTo.room;
            transform.parent = room.transform;
        }
    }
}
