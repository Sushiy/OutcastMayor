using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    Material[][] originalMaterials;

    SnappingPoint[] snappingPoints;
    public BoxCollider mainCollider;

    private void Awake()
    {
        /*
        originalMaterials = new Material[meshRenderers.Length][];
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            originalMaterials[m] = new Material[meshRenderers[m].materials.Length];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                originalMaterials[m][i] = meshRenderers[m].materials[i];
            }
        }*/

        snappingPoints = GetComponentsInChildren<SnappingPoint>();
    }

    public void ResetMaterials()
    {
        for (int m = 0; m < meshRenderers.Length; m++)
        {
            meshRenderers[m].materials = originalMaterials[m];
        }
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

    public Transform ownSnapReference;
    public Transform otherSnapReference;

    public void SnapPoints(Transform own, Transform other)
    {
        ownSnapReference = own;
        otherSnapReference = other;
    }

    public void StopSnap(Transform own, Transform other)
    {
        if(ownSnapReference == own && otherSnapReference == other)
        {
            ownSnapReference = null;
            otherSnapReference = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Draw bounds
        if(mainCollider)
        {
        }
    }
}
