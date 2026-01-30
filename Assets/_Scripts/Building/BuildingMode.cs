using OutcastMayor.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OutcastMayor.Building
{
    public class BuildingMode : MonoBehaviour
    {
        public bool isActive
        {
            private set;
            get;
        }
        [Header("References")]
        private Player player;
        [SerializeField]
        private Material ghostMaterial;
        [SerializeField]
        private Material sensorMaterial;
        [SerializeField]
        private Transform buildingParent;
        [SerializeField]
        private LayerMask buildModeCullingMask;
        private LayerMask defaultCullingMask;

        [Header("BuildMode Settings")]
        [SerializeField]
        public BuildRecipe[] recipes;
        private BuildRecipe selectedRecipe;

        private Vector3 buildPosition;
        private Vector3 rayCastPosition;
        [SerializeField]
        private float raycastMaxDistance = 5.0f;
        private Quaternion buildRotation;
        private Quaternion buildYRotation;
        private Quaternion buildXLocalRotation;
        [SerializeField]
        private float rotateSpeed = 10.0f;
        [SerializeField]
        private Vector2 buildAngle = Vector2.zero;
        
        [SerializeField]
        private float baseBuildAngleY = 0;

        public enum BuildRotationMode
        {
            fixedAngle = 0,
            viewAngle = 1,
            normalAngle
        }

        public BuildRotationMode buildRotationMode = BuildRotationMode.fixedAngle;

        /// <summary>
        /// This copy snaps to where the player wants to build
        /// </summary>
        private Buildable ghostBuilding;
        /// <summary>
        /// This Copy stays exactly where the mouse is aimed, but is invisible
        /// </summary>
        private Buildable sensorBuilding;

        [SerializeField]
        private PointerIndicator indicator;

        [SerializeField]
        bool autoCompleteBuilds = false;

        //Events
        public Action<BuildRotationMode> onBuildRotationModeChanged;

        public Action<Buildable> onBuildablePlaced;

        private void Start()
        {
            player = Player.Instance;
            player.PlayerInputManager.onRotationModePressed += NextRotationMode;
        }

        void OnDestroy()
        {
            player.PlayerInputManager.onRotationModePressed -= NextRotationMode;
        }

        public void ChooseBuildRecipe(BuildRecipe buildRecipe)
        {
            if (ghostBuilding != null)
            {
                Destroy(ghostBuilding.gameObject);
                Destroy(sensorBuilding.gameObject);
            }

            selectedRecipe = buildRecipe;
            ghostBuilding = GameObject.Instantiate(buildRecipe.BuildingPrefab, buildPosition, buildRotation);
            ghostBuilding.transform.localScale = selectedRecipe.BuildScale;
            ghostBuilding.SetGhostMode(ghostMaterial);

            sensorBuilding = GameObject.Instantiate(buildRecipe.BuildingPrefab, buildPosition, buildRotation);
            sensorBuilding.transform.localScale = selectedRecipe.BuildScale;
            sensorBuilding.SetSensorMode(sensorMaterial);
        }

        public void EnterBuildMode()
        {
            isActive = true;
            player.ChangeState(player.BuildingState);
            defaultCullingMask = Camera.main.cullingMask;
            Camera.main.cullingMask = buildModeCullingMask;
            if (selectedRecipe == null)
            {
                ChooseBuildRecipe(recipes[0]);
            }
            else
            {
                ChooseBuildRecipe(selectedRecipe);
            }
            UI.UIManager.Instance.ShowBuildingModeHud();
        }

        public void ExitBuildMode()
        {
            Camera.main.cullingMask = defaultCullingMask;
            isActive = false;
            if(indicator)
                indicator.SetVisible(false);
            UI.UIManager.Instance.HideBuildingView();
            UI.UIManager.Instance.HideBuildingModeHud();
            Destroy(ghostBuilding.gameObject);
            Destroy(sensorBuilding.gameObject);
        }
        public Construction Build(Vector3 position)
        {
            Buildable snappedBuilding = null;
            if (sensorBuilding.snappedPointOther != null)
            {
                snappedBuilding = sensorBuilding.snappedPointOther.buildable;
            }
            Buildable build = GameObject.Instantiate(selectedRecipe.BuildingPrefab, position, buildRotation, buildingParent);
            build.transform.localScale = selectedRecipe.BuildScale;
            build.OnBuild(snappedBuilding);
            build.SetBlueprintMode(ghostMaterial);
            onBuildablePlaced(build);
            
            Construction c = GameObject.Instantiate(selectedRecipe.ConstructionPrefab, position, buildRotation, build.transform);
            c.transform.localScale = selectedRecipe.BuildScale;
            c.SetConstruction(selectedRecipe, build, autoCompleteBuilds);
            return c;
        }

        public void Build()
        {
            Build(buildPosition);
        }

        private void Update()
        {
            if (isActive && ghostBuilding != null)
            {
                
                switch(buildRotationMode)
                {
                    case BuildRotationMode.fixedAngle:
                        baseBuildAngleY = 0;
                        break;
                    case BuildRotationMode.viewAngle:
                        Vector3 flatView = Camera.main.transform.forward;
                        flatView.y = 0;
                        baseBuildAngleY =  Quaternion.LookRotation(flatView).eulerAngles.y;
                        break;
                    case BuildRotationMode.normalAngle:
                        Vector3 flatNormal = surfaceNormal.direction;
                        flatNormal.y = 0;
                        if(flatNormal != Vector3.zero)
                        {
                            baseBuildAngleY = Quaternion.LookRotation(flatNormal).eulerAngles.y; 
                        }
                        break;
                }
                buildRotation = Quaternion.Euler(0, (baseBuildAngleY + buildAngle.y)%360, buildAngle.x);

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
                    float min = 0;
                    List<int> minIndices = new List<int>();
                    for (int i = 0; i < 8; i++)
                    {
                        if (Mathf.Abs(d[i] - min) <= 0.01f)
                        {
                            minIndices.Add(i);
                            if (d[i] < min)
                                min = d[i];
                        }
                        else if (d[i] < min)
                        {
                            min = d[i];
                            minIndices.Clear();
                            minIndices.Add(i);
                        }
                        Debug.DrawRay(v[i], -p[i], Color.blue);
                        Debug.DrawRay(v[i], Vector3.up, Color.blue);
                    }

                    //Average the maximum points we found
                    Vector3 averagePoint = Vector3.zero;
                    Vector3 averageProjection = Vector3.zero;
                    for (int i = 0; i < minIndices.Count; i++)
                    {
                        averagePoint += v[minIndices[i]];
                        averageProjection += p[minIndices[i]];
                        Debug.DrawRay(v[minIndices[i]], -p[minIndices[i]], Color.cyan);
                        Debug.DrawRay(v[minIndices[i]], Vector3.up, Color.cyan);
                    }
                    averagePoint /= minIndices.Count;
                    averageProjection /= minIndices.Count;
                    //Debug.DrawRay(averagePoint, averageProjection, Color.yellow);
                    #endregion
                    offset = surfaceNormal.origin - averagePoint;
                    Debug.DrawRay(rayCastPosition, offset, Color.green);
                }
                Vector3 sensorPosition = rayCastPosition + offset;
                
                if(selectedRecipe is BuildRecipeHeight)
                {
                    sensorPosition += (selectedRecipe as BuildRecipeHeight).GetHeightOffset();
                }

                //Offset the sensor, this is to trigger snapping from the correct position
                sensorBuilding.transform.SetPositionAndRotation(sensorPosition, buildRotation);

                //Offset the buildposition
                buildPosition = rayCastPosition + offset;
                
                if(selectedRecipe is BuildRecipeHeight)
                {
                    buildPosition += (selectedRecipe as BuildRecipeHeight).GetHeightOffset();
                }

                if (sensorBuilding.snappedPointSelf != null && sensorBuilding.snappedPointOther != null)
                {
                    Debug.DrawLine(sensorBuilding.snappedPointSelf.position, sensorBuilding.snappedPointOther.position, Color.red);
                    Vector3 snappedBuildPosition = buildPosition + sensorBuilding.snappedPointOther.position - sensorBuilding.snappedPointSelf.position;
                    if ((snappedBuildPosition - sensorBuilding.snappedPointOther.buildable.transform.position).sqrMagnitude > .01f)
                    {
                        //you can snap, these objects don't end up in the same location
                        buildPosition = snappedBuildPosition;
                    }
                }
                ghostBuilding.transform.SetPositionAndRotation(buildPosition, buildRotation);
                ghostBuilding.OnSetPosition();
             }
        }

        Ray surfaceNormal;
        bool raycastHit;

        public void ProcessRayCast(bool raycastHit, Ray ray, RaycastHit hitInfo)
        {
            this.raycastHit = raycastHit;
            if (raycastHit)
            {
                rayCastPosition = hitInfo.point;
                surfaceNormal.origin = hitInfo.point;
                surfaceNormal.direction = hitInfo.normal;
                if (indicator)
                {
                    indicator.transform.position = rayCastPosition;
                    indicator.transform.rotation = Quaternion.LookRotation(surfaceNormal.direction);
                    indicator.SetVisible(true);
                }
            }
            else
            {
                if (indicator)
                {
                    indicator.SetVisible(false);
                }
                rayCastPosition = ray.origin + ray.direction * raycastMaxDistance;
            }
        }

        public void NextRotationMode()
        {
            buildRotationMode = (BuildRotationMode)(((int)buildRotationMode +1)%3);
            onBuildRotationModeChanged?.Invoke(buildRotationMode);
        }

        public void Rotate(float rotateInput, bool _isModifierDown)
        {
            if (rotateInput != 0)
            {
                //print("Rotate:" + Mathf.Sign(rotateInput));
                float angleDelta = Mathf.Sign(rotateInput) * rotateSpeed;
                if (!_isModifierDown)
                {
                    buildAngle.y += angleDelta;
                    buildAngle.y %= 360;
                }
                else if(!(ghostBuilding is Foundation))
                {
                    buildAngle.x += angleDelta;
                    buildAngle.x %= 360;
                }
                buildRotation = Quaternion.Euler(0, (baseBuildAngleY + buildAngle.y)%360, buildAngle.x);
            }
        }
        public void Alternate(float alternateInput)
        {
            selectedRecipe.Alternate(alternateInput);
            ChooseBuildRecipe(selectedRecipe);
        }

        public void DestroyConstruction()
        {

        }
    }

}
