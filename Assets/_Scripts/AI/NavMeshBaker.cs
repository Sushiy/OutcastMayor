using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] navMeshSurfaces;

    List<NavMeshBuildSource> sources;
    public static Queue<Vector3> boundPositions;
    public Vector3 rebuildBoundSize = new Vector3(30,30,30);

    public float updateInterval = 1.0f;

    void Awake()
    {
        boundPositions = new Queue<Vector3>();
        sources = new List<NavMeshBuildSource>();
    }

    void Start()
    {
        StartCoroutine(ScheduleNavMesh());
        ShouldRebuild = true;
    }

    public void SetNavMeshTileSize(int _tileSize)
    {
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].overrideTileSize = true;
            navMeshSurfaces[i].tileSize = _tileSize;
        }
    }

    private async Task RebuildWholeMesh()
    {
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }

    private async Task BuildNavMesh()
    {
        Bounds worldBounds = new Bounds(boundPositions.Dequeue(), rebuildBoundSize);
        print($"Building Mesh around position {worldBounds.center}");
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            NavMeshData meshData = new NavMeshData();
            await UpdateNavMeshInBounds(navMeshSurfaces[i], worldBounds, navMeshSurfaces[i].navMeshData);
            NavMesh.AddNavMeshData(meshData);
        }
    }
    private AsyncOperation UpdateNavMeshInBounds(NavMeshSurface _navMeshSurface, Bounds _worldBounds, NavMeshData navMeshData)
    {
        sources.Clear();
        NavMeshBuilder.CollectSources(_worldBounds, _navMeshSurface.layerMask, _navMeshSurface.useGeometry, _navMeshSurface.defaultArea, new List<NavMeshBuildMarkup>(), sources);
        foreach(NavMeshBuildSource s in sources)
            print($"Building Navmesh, source: {s.shape} {s.sourceObject}");
        var localBounds = new Bounds();
        localBounds.center = transform.InverseTransformPoint(_worldBounds.center);
        localBounds.size = transform.InverseTransformDirection(_worldBounds.size);

        NavMeshBuildSettings navMeshBuildSettings = _navMeshSurface.GetBuildSettings();

        navMeshBuildSettings.preserveTilesOutsideBounds = true;
        return NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshBuildSettings, sources, localBounds);
    }

    private static bool ShouldRebuild = false;

    public static void SetNavMeshUpdate(Vector3 worldPosition)
    {
        boundPositions.Enqueue(worldPosition);
    }

    IEnumerator ScheduleNavMesh()
    {
        while(true)
        {
            if (ShouldRebuild || boundPositions.Count > 0)
            {
                ShouldRebuild = false;
                _ = BuildNavMesh();
            }
            yield return new WaitForSeconds(updateInterval);
        }

    }
}
