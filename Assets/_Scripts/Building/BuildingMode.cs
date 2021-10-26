using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMode : MonoBehaviour
{
    public bool isActive
    {
        private set;
        get;
    }
    [Header("References")]
    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private Material ghostMaterial;
    [SerializeField]
    private Material sensorMaterial;
    [SerializeField]
    private Transform buildingParent;


    [Header("BuildMode Settings")]
    [SerializeField]
    public BuildRecipe[] recipes;
    private BuildRecipe selectedRecipe;

    private Vector3 buildPosition;
    private Vector3 rayCastPosition;
    private Quaternion buildRotation;
    [SerializeField]
    private float rotateSpeed = 10.0f;
    private float buildAngle = 0;


    /// <summary>
    /// This copy snaps to where the player wants to build
    /// </summary>
    private Buildable ghostBuilding;
    /// <summary>
    /// This Copy stays exactly where the mouse is aimed, but is invisible
    /// </summary>
    private Buildable sensorBuilding;

    [SerializeField]
    private CurrentBuildPanel currentBuildPanel;


    public void ChooseBuildRecipe(BuildRecipe buildRecipe)
    {
        if(ghostBuilding != null)
        {
            Destroy(ghostBuilding.gameObject);
            Destroy(sensorBuilding.gameObject);
        }

        selectedRecipe = buildRecipe;
        ghostBuilding = GameObject.Instantiate(buildRecipe.prefab, buildPosition, buildRotation);
        ghostBuilding.SetGhostMode(ghostMaterial);

        sensorBuilding = GameObject.Instantiate(buildRecipe.prefab, buildPosition, buildRotation);
        sensorBuilding.SetSensorMode(sensorMaterial);

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

    public void ExitBuildMode()
    {
        isActive = false;
        currentBuildPanel.Hide();
        Destroy(ghostBuilding.gameObject);
        Destroy(sensorBuilding.gameObject);
    }

    public void Build()
    {
        if (selectedRecipe.IsValid(inventory))
        {
            Buildable snappedBuilding = null;
            if (sensorBuilding.snappedPointOther != null)
            {
                snappedBuilding = sensorBuilding.snappedPointOther.buildable;
            }
            for (int i = 0; i < selectedRecipe.materials.Length; i++)
            {      
                inventory.Delete(selectedRecipe.materials[i]);
            }
            Buildable b = GameObject.Instantiate(selectedRecipe.prefab, buildPosition, buildRotation, buildingParent);
            b.SetBuildMode(snappedBuilding);
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

            Vector3 offset = Vector3.zero;
            if (raycastHit)
            {
                #region Placement Calculation
                //Calculate where to put the mesh, so that it doesnt poke through the other mesh

                //First place the sensor to the exact raycast position
                sensorBuilding.transform.SetPositionAndRotation(rayCastPosition, buildRotation);

                //Calculate the 8 corner of the sensors box collider
                Vector3 c = sensorBuilding.buildCollider.center;
                Vector3 s = sensorBuilding.buildCollider.size / 2.0f;
                float sX = s.x;
                float sY = s.y;
                float sZ = s.z;

                //Eight Corner Vertices in worldspace
                Vector3[] v = new Vector3[8];
                v[0] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(-sX, sY, -sZ));
                v[1] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(-sX, -sY, -sZ));
                v[2] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(-sX, sY, sZ));
                v[3] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(-sX, -sY, sZ));
                v[4] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(sX, sY, sZ));
                v[5] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(sX, -sY, sZ));
                v[6] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(sX, -sY, -sZ));
                v[7] = sensorBuilding.buildCollider.transform.TransformPoint(c + new Vector3(sX, sY, -sZ));

                //Project the corner vertices onto the surfacenormal of the raycasted point
                Vector3[] p = new Vector3[8];
                p[0] = Vector3.Project(v[0] - surfaceNormal.origin, surfaceNormal.direction);
                p[1] = Vector3.Project(v[1] - surfaceNormal.origin, surfaceNormal.direction);
                p[2] = Vector3.Project(v[2] - surfaceNormal.origin, surfaceNormal.direction);
                p[3] = Vector3.Project(v[3] - surfaceNormal.origin, surfaceNormal.direction);
                p[4] = Vector3.Project(v[4] - surfaceNormal.origin, surfaceNormal.direction);
                p[5] = Vector3.Project(v[5] - surfaceNormal.origin, surfaceNormal.direction);
                p[6] = Vector3.Project(v[6] - surfaceNormal.origin, surfaceNormal.direction);
                p[7] = Vector3.Project(v[7] - surfaceNormal.origin, surfaceNormal.direction);

                //Get the signed length of each of the projected vectors
                float[] d = new float[8];
                d[0] = p[0].magnitude * Vector3.Dot(p[0], surfaceNormal.direction);
                d[1] = p[1].magnitude * Vector3.Dot(p[1], surfaceNormal.direction);
                d[2] = p[2].magnitude * Vector3.Dot(p[2], surfaceNormal.direction);
                d[3] = p[3].magnitude * Vector3.Dot(p[3], surfaceNormal.direction);
                d[4] = p[4].magnitude * Vector3.Dot(p[4], surfaceNormal.direction);
                d[5] = p[5].magnitude * Vector3.Dot(p[5], surfaceNormal.direction);
                d[6] = p[6].magnitude * Vector3.Dot(p[6], surfaceNormal.direction);
                d[7] = p[7].magnitude * Vector3.Dot(p[7], surfaceNormal.direction);

                //Find the maximum of d and save all indices with that value
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

                //Avearage the maximum points we found
                Vector3 averagePoint = Vector3.zero;
                Vector3 averageProjection = Vector3.zero;
                for (int i = 0; i < maxIndices.Count; i++)
                {
                    averagePoint += v[maxIndices[i]];
                    averageProjection += p[maxIndices[i]];
                    Debug.DrawRay(v[maxIndices[i]], -p[maxIndices[i]], Color.cyan);
                }
                averagePoint /= maxIndices.Count;
                averageProjection /= maxIndices.Count;
                //Debug.DrawRay(averagePoint, averageProjection, Color.yellow);
                #endregion
                offset = averagePoint - surfaceNormal.origin;
                Debug.DrawRay(rayCastPosition, offset, Color.green);
            }

            //Offset the sensor, this is to trigger snapping from the correct position
            sensorBuilding.transform.SetPositionAndRotation(rayCastPosition + offset, buildRotation);

            //Offset the buildposition
            buildPosition = rayCastPosition + offset;

            if (sensorBuilding.snappedPointSelf != null)
            {
                Debug.DrawLine(sensorBuilding.snappedPointSelf.position, sensorBuilding.snappedPointOther.position, Color.red);
                buildPosition += sensorBuilding.snappedPointOther.position - sensorBuilding.snappedPointSelf.position;
            }
            ghostBuilding.transform.SetPositionAndRotation(buildPosition, buildRotation);
        }
    }

    Ray surfaceNormal;
    bool raycastHit;

    public void ProcessRayCast(bool raycastHit, RaycastHit hitInfo)
    {
        this.raycastHit = raycastHit;
        if(raycastHit)
        {
            rayCastPosition = hitInfo.point;
            surfaceNormal.origin = hitInfo.point;
            surfaceNormal.direction = hitInfo.normal;
        }
    }

    public void Rotate(float rotateInput)
    {
        if(rotateInput != 0)
        {
            //print("Rotate:" + Mathf.Sign(rotateInput));
            buildAngle += Mathf.Sign(rotateInput) * rotateSpeed;
            buildRotation = Quaternion.Euler(0, buildAngle, 0);
        }
    }
}
