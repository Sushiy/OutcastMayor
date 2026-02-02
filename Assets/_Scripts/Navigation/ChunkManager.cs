using Sirenix.OdinInspector;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{   
    public int tileSize = 64;

    NavMeshBaker navMeshBaker;

    void Awake()
    {
        navMeshBaker = GetComponent<NavMeshBaker>();

        navMeshBaker.SetNavMeshTileSize(tileSize);
    }

    void OnDrawGizmos()
    {
        //Try to draw the same grid as the navmesh
    }
}