using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabWallPlacer : MonoBehaviour
{
    [SerializeField]
    VectorBuilding vectorBuilding;

    [SerializeField]
    GameObject wallPrefab;
    [SerializeField]
    float wallLength = 2;
    [SerializeField]
    float wallHeight = 2;

    List<GameObject> walls;

    [Button]
    public void PlacePrefabs()
    { 
        walls = new List<GameObject>();
        float halfHeight = wallHeight/2;
        foreach(VectorEdge edge in vectorBuilding.currentVectorPointGraph.edges)
        {
            if(edge.isInside) continue;
            Vector3 normalized = edge.Vector.normalized;
            float len = edge.Length;
            int wallCount = Mathf.FloorToInt(edge.Length/wallLength);
            float rest = len % wallLength;
            for(int i = 0; i < wallCount; i++)
            {
                Transform wallT = Instantiate(wallPrefab, transform).transform;
                wallT.position = edge.p1.worldPosition + normalized * wallLength * (i+0.5f) + Vector3.up * halfHeight;
                wallT.rotation = Quaternion.LookRotation(edge.Vector);
                walls.Add(wallT.gameObject);
                if(i == wallCount-1 && rest > 0)
                {             
                    wallT.transform.localScale = new Vector3(1,1, 1 + rest/wallLength);
                    wallT.position = edge.p1.worldPosition + normalized * wallLength * (i+0.5f) + normalized * rest/2 + Vector3.up * halfHeight;
                    /*
                    if(rest < wallLength/2)
                    {
                    }
                    else
                    {
                        Transform wallT2 = Instantiate(wallPrefab, transform).transform;
                        wallT2.position = edge.p1.worldPosition + normalized * wallLength * (i+0.5f + 1) + Vector3.up * halfHeight;
                        wallT2.rotation = Quaternion.LookRotation(edge.Vector);
                        wallT2.transform.localScale = new Vector3(1,1, rest/wallLength);
                        walls.Add(wallT2.gameObject);
                    }*/
                }
            }
        }
    }
}
