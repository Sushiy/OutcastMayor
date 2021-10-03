using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMode : MonoBehaviour
{
    public bool isActive = false;

    public Inventory inventory;
    public BuildRecipe[] recipes;
    public Material ghostMaterial;
    public Material sensorMaterial;

    public BuildRecipe selectedRecipe;

    public Vector3 buildPosition;
    public Vector3 rayCastPosition;
    public Quaternion buildRotation;
    private float buildAngle = 0;

    /// <summary>
    /// This copy snaps to where the player wants to build
    /// </summary>
    public Buildable ghostBuilding;
    /// <summary>
    /// This Copy stays exactly where the mouse is aimed, but is invisible
    /// </summary>
    public Buildable sensorBuilding;

    public float rotateSpeed = 10.0f;

    public void ChooseBuildRecipe(BuildRecipe buildRecipe)
    {
        if(ghostBuilding != null)
        {
            Destroy(ghostBuilding.gameObject);
            Destroy(sensorBuilding.gameObject);
        }
        selectedRecipe = buildRecipe;
        ghostBuilding = GameObject.Instantiate(buildRecipe.prefab, buildPosition, buildRotation);
        ghostBuilding.SetMaterials(ghostMaterial);
        ghostBuilding.gameObject.name = "Ghost";
        ghostBuilding.SetLayerForAllColliders(2);

        sensorBuilding = GameObject.Instantiate(buildRecipe.prefab, buildPosition, buildRotation);
        sensorBuilding.SetMaterials(sensorMaterial);
        sensorBuilding.gameObject.name = "Sensor";
        sensorBuilding.SetLayerForAllColliders(6);
    }

    public void EnterBuildMode()
    {
        isActive = true;
        if(selectedRecipe == null)
        {
            ChooseBuildRecipe(recipes[0]);
        }
        else
        {
            ChooseBuildRecipe(selectedRecipe);
        }
    }

    public void Build()
    {
        if (selectedRecipe.IsValid(inventory))
        {
            for (int i = 0; i < selectedRecipe.materials.Length; i++)
            {      
                inventory.Delete(selectedRecipe.materials[i]);
            }
            GameObject.Instantiate(selectedRecipe.prefab, buildPosition, buildRotation);
        }
    }

    private void Update()
    {
        if(isActive && ghostBuilding != null)
        {
            if(!selectedRecipe.IsValid(inventory))
            {
                ghostMaterial.color = new Color(1, 0.071f,0.065f, 0.33f);
            }
            else
            {
                ghostMaterial.color = new Color(0.0627f, 0.6358f, 1.0f, 0.33f);
            }

            Debug.DrawRay(surfaceNormal.origin, surfaceNormal.direction, Color.yellow);


            sensorBuilding.transform.SetPositionAndRotation(rayCastPosition, buildRotation);

            if (sensorBuilding.ownSnapReference != null)
            {
                Debug.DrawLine(sensorBuilding.ownSnapReference.position, sensorBuilding.otherSnapReference.position, Color.red);
                buildPosition += sensorBuilding.otherSnapReference.position - sensorBuilding.ownSnapReference.position;
            }

            ghostBuilding.transform.SetPositionAndRotation(buildPosition, buildRotation);
        }
    }

    Ray surfaceNormal;

    public void ProcessRayCast(bool raycastHit, RaycastHit hitInfo)
    {
        if(raycastHit)
        {
            rayCastPosition = hitInfo.point;
            buildPosition = hitInfo.point;
            surfaceNormal.origin = hitInfo.point;
            surfaceNormal.direction = hitInfo.normal;
        }
    }

    public void EndBuildMode()
    {
        isActive = false;
        Destroy(ghostBuilding.gameObject);
        Destroy(sensorBuilding.gameObject);
    }

    float rotateInput;
    public void Rotate(float rotateInput)
    {
        this.rotateInput = rotateInput;
        if(rotateInput != 0)
        {
            buildAngle += Mathf.Sign(rotateInput) * rotateSpeed;
            buildRotation = Quaternion.Euler(0, buildAngle, 0);
        }
    }
}
