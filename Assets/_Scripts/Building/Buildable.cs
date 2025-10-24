using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace OutcastMayor.Building
{

    [RequireComponent(typeof(Rigidbody))]
    public class Buildable : MonoBehaviour
    {
        [Header("Placement System")]
        [SerializeField]
        private MeshRenderer[] meshRenderers;
        [SerializeField]
        public BoxCollider buildCollider;
        public SnappingPoint[] snappingPoints
        {
            private set;
            get;
        }

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

        public Room room
        {
            protected set;
            get;
        }

        [Header("Room Recognition")]
        public Buildable anchorParent;
        public UnityEvent OnChecked;
        public bool isBlueprint = false;
        protected static LayerMask buildLayer = 1 << 7;

        Collider[] allColliders;
        private void Awake()
        {
            snappingPoints = GetComponentsInChildren<SnappingPoint>();
            
            allColliders = GetComponentsInChildren<Collider>();
        }

        public void SetGhostMode(Material ghostMaterial)
        {
            SetMaterials(ghostMaterial);
            gameObject.name = "Ghost";
            SetLayerForAllColliders(LayerConstants.Ghost);
        }

        public void SetSensorMode(Material sensorMaterial)
        {
            SetMaterials(sensorMaterial);
            SetInvisible();
            gameObject.name = "Sensor";
            SetLayerForAllColliders(LayerConstants.Snapping);
        }

        public void SetBlueprintMode(Material ghostMaterial)
        {
            SetBlueprintLayer();
            SetMaterials(ghostMaterial);
        }

        private void SetRendererLayers(int i)
        {
            for (int m = 0; m < meshRenderers.Length; m++)
            {
                meshRenderers[m].gameObject.layer = i;
            }
        }

        public void SetRenderColor(Color c)
        {
            for (int m = 0; m < meshRenderers.Length; m++)
            {
                meshRenderers[m].material.color = c;
            }
        }

        public void SetDefaultLayer()
        {
            SetRendererLayers(LayerConstants.Default);
            isBlueprint = false;
            CheckForRoom();
        }

        public void SetBlueprintLayer()
        {
            //SetRendererLayers(LayerConstants.BuildingOnly);
            isBlueprint = true;
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
            for (int i = 0; i < allColliders.Length; i++)
            {
                allColliders[i].gameObject.layer = layer;
            }
        }

        public void StartSnapping(SnappingPoint own, SnappingPoint other)
        {
            snappedPointSelf = own;
            snappedPointOther = other;
        }

        public void StopSnapping(SnappingPoint own, SnappingPoint other)
        {
            if (snappedPointSelf == own && snappedPointOther == other)
            {
                snappedPointSelf = null;
                snappedPointOther = null;
            }
            else
            {
                //Debug.LogWarning("We tried to stop snapping the wrong points.");
            }
        }

        /// <summary>
        /// This method tries to sort Buildables into rooms. Rooms should eventually be used to validate buildings.
        /// For now it just keeps the scene hiearchy a little cleaner
        /// </summary>
        public virtual void CheckForRoom()
        {
            if (isBlueprint) return;
            //Proposed Function:
            //1. Check the buildcollider "as trigger" for all overlapping objects
            OverlapResult o = CheckRoomOverlap();
            for (int i = 0; i < o.touchedRooms.Count; i++)
            {
                RoomManager.CheckRoomIntegrity(o.touchedRooms[i]);
                RoomManager.CheckRoomValidity(o.touchedRooms[i]);
            }
        }

        public virtual void OnSetPosition()
        {
            
        }

        public virtual OverlapResult CheckRoomOverlap()
        {
            OverlapResult result;
            result.touchedFloors = new List<Floor>();
            result.touchedRooms = new List<Room>();

            Collider[] colliders = Physics.OverlapBox(transform.position, buildCollider.size / 2.0f, transform.rotation, buildLayer);
            //print("Touched: " + colliders.Length + " buildables");
            //2. Step through all of the objects, find floors and tell them to check their rooms!
            for (int i = 0; i < colliders.Length; i++)
            {
                Floor f = colliders[i].GetComponentInParent<Floor>();
                if (f == this) continue;
                if (f && !f.isBlueprint)
                {
                    result.touchedFloors.Add(f);
                    if (!result.touchedRooms.Contains(f.room))
                    {
                        result.touchedRooms.Add(f.room);
                    }
                }
            }
            //print("Touched: " + result.touchedFloors.Count + " floors & " + result.touchedRooms.Count + " rooms");
            return result;
        }


        /// <summary>
        /// Get the snapping point with the lowest local z coordinate
        /// </summary>
        /// <returns>snapping point with the lowest local z coordinate</returns>
        Transform startPoint = null;
        public Vector3 StartPoint
        {
            get
            {
                if (startPoint == null)
                {
                    float localZ = Mathf.Infinity;
                    for (int i = 0; i < snappingPoints.Length; i++)
                    {
                        if (snappingPoints[i].transform.localPosition.z < localZ)
                        {
                            localZ = snappingPoints[i].transform.localPosition.z;
                            startPoint = snappingPoints[i].transform;
                        }
                    }
                }
                return startPoint.position;
            }
        }

        /// <summary>
        /// Get the snapping point with the highest local z coordinate
        /// </summary>
        /// <returns>snapping point with the highest local z coordinate</returns>
        Transform endPoint = null;
        public Vector3 EndPoint
        {
            get
            {
                if (endPoint == null)
                {
                    float localZ = -Mathf.Infinity;
                    for (int i = 0; i < snappingPoints.Length; i++)
                    {
                        if (snappingPoints[i].transform.localPosition.z > localZ)
                        {
                            localZ = snappingPoints[i].transform.localPosition.z;
                            endPoint = snappingPoints[i].transform;
                        }
                    }
                }
                return endPoint.position;
            }
        }

        public struct OverlapResult
        {
            public List<Floor> touchedFloors;
            public List<Room> touchedRooms;
        }
    }


}