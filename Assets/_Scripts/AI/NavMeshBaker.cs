using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] navMeshSurfaces;

    private void Awake()
    {
    }

    private void Start()
    {
        ShouldRebuild = true;
        StartCoroutine(ScheduleNavMesh());
    }

    public void BuildNavMesh()
    {
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }

    public static bool ShouldRebuild = false;

    IEnumerator ScheduleNavMesh()
    {
        while(true)
        {
            if (ShouldRebuild)
            {
                ShouldRebuild = false;
                BuildNavMesh();
            }
            yield return new WaitForSeconds(1.0f);
        }

    }
}
