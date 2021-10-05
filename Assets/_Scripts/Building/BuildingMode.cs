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

    public CurrentBuildPanel currentBuildPanel;

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
        sensorBuilding.SetInvisible();
        sensorBuilding.gameObject.name = "Sensor";
        sensorBuilding.SetLayerForAllColliders(6);

        currentBuildPanel.Show();
        currentBuildPanel.SetData(buildRecipe);
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
        if(sensorBuilding.otherSnapReference != null)
        {
            Buildable snappedBuilding = sensorBuilding.otherSnapReference.buildable;
        }
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
            sensorBuilding.transform.SetPositionAndRotation(rayCastPosition, buildRotation);
            //Calculate where to put the mesh, so that it doesnt poke through the other mesh
            Vector3 c = sensorBuilding.mainCollider.center;
            Vector3 cw = sensorBuilding.mainCollider.transform.TransformPoint(c);
            float sX = sensorBuilding.mainCollider.size.x / 2.0f;
            float sY = sensorBuilding.mainCollider.size.y / 2.0f;
            float sZ = sensorBuilding.mainCollider.size.z / 2.0f;

            Vector3[] v = new Vector3[8];
            v[0] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(-sX, sY, -sZ));
            v[1] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(-sX, -sY, -sZ));
            v[2] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(-sX, sY, sZ));
            v[3] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(-sX, -sY, sZ));
            v[4] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(sX, sY, sZ));
            v[5] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(sX, -sY, sZ));
            v[6] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(sX, -sY, -sZ));
            v[7] = sensorBuilding.mainCollider.transform.TransformPoint(c + new Vector3(sX, sY, -sZ));
            Vector3[] p = new Vector3[8];

            p[0] = Vector3.Project(v[0] - surfaceNormal.origin, surfaceNormal.direction);
            p[1] = Vector3.Project(v[1] - surfaceNormal.origin, surfaceNormal.direction);
            p[2] = Vector3.Project(v[2] - surfaceNormal.origin, surfaceNormal.direction);
            p[3] = Vector3.Project(v[3] - surfaceNormal.origin, surfaceNormal.direction);
            p[4] = Vector3.Project(v[4] - surfaceNormal.origin, surfaceNormal.direction);
            p[5] = Vector3.Project(v[5] - surfaceNormal.origin, surfaceNormal.direction);
            p[6] = Vector3.Project(v[6] - surfaceNormal.origin, surfaceNormal.direction);
            p[7] = Vector3.Project(v[7] - surfaceNormal.origin, surfaceNormal.direction);

            float[] d = new float[8];

            d[0] = p[0].magnitude * Vector3.Dot(p[0], surfaceNormal.direction);
            d[1] = p[1].magnitude * Vector3.Dot(p[1], surfaceNormal.direction);
            d[2] = p[2].magnitude * Vector3.Dot(p[2], surfaceNormal.direction);
            d[3] = p[3].magnitude * Vector3.Dot(p[3], surfaceNormal.direction);
            d[4] = p[4].magnitude * Vector3.Dot(p[4], surfaceNormal.direction);
            d[5] = p[5].magnitude * Vector3.Dot(p[5], surfaceNormal.direction);
            d[6] = p[6].magnitude * Vector3.Dot(p[6], surfaceNormal.direction);
            d[7] = p[7].magnitude * Vector3.Dot(p[7], surfaceNormal.direction);

            float max = 0;
            List<int> maxIndices = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                if (Mathf.Abs(d[i] - max) < 0.01f)
                {
                    maxIndices.Add(i);
                }
                else if (d[i] > max)
                {
                    max = d[i];
                    maxIndices.Clear();
                    maxIndices.Add(i);
                }
                Debug.DrawRay(v[i], -p[i], Color.blue);
            }

            Vector3 averagePoint = Vector3.zero;
            Vector3 averageProjection = Vector3.zero;
            for(int i = 0; i < maxIndices.Count; i++)
            {
                averagePoint += v[maxIndices[i]];
                averageProjection += p[maxIndices[i]];
                Debug.DrawRay(v[maxIndices[i]], -p[maxIndices[i]], Color.cyan);
            }
            averagePoint /= maxIndices.Count;
            averageProjection /= maxIndices.Count;
            Debug.DrawRay(averagePoint, averageProjection, Color.green);
            Vector3 offset = averagePoint - surfaceNormal.origin;

            sensorBuilding.transform.SetPositionAndRotation(rayCastPosition + offset, buildRotation);

            buildPosition = rayCastPosition + offset;

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
            surfaceNormal.origin = hitInfo.point;
            surfaceNormal.direction = hitInfo.normal;
        }
    }

    public void EndBuildMode()
    {
        isActive = false;
        currentBuildPanel.Hide();
        Destroy(ghostBuilding.gameObject);
        Destroy(sensorBuilding.gameObject);
    }

    float rotateInput;
    public void Rotate(float rotateInput)
    {
        this.rotateInput = rotateInput;
        if(rotateInput != 0)
        {
            print("Rotate:" + Mathf.Sign(rotateInput));
            buildAngle += Mathf.Sign(rotateInput) * rotateSpeed;
            buildRotation = Quaternion.Euler(0, buildAngle, 0);
        }
    }
}
