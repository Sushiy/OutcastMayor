using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralWallMesh : MonoBehaviour
{
    public List<Transform> positions;

    public float wallThickness;
    public float wallheight;
    MeshFilter meshFilter;

    public bool closed = false;

    public Mesh originalCenterMesh;
    Vector3[] originalCenterVertices1;
    Vector3[] originalCenterVertices2;
    int[] segmentTriangles;

    Vector3[] segmentVertices;

    void GetReferences()
    {
        if(meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        tangents = new Vector3[positions.Count];
    }

    [Button]
    public void GetOriginalMeshData()
    {
        if(originalCenterMesh != null)
        {
            originalCenterVertices1 = new Vector3[originalCenterMesh.vertexCount/2];
            originalCenterVertices2 = new Vector3[originalCenterMesh.vertexCount/2];

            int v1 = 0;
            int v2 = 0;
            for(int i = 0; i < originalCenterMesh.vertexCount; i++)
            {
                Vector3 v = originalCenterMesh.vertices[i];
                if(v.z < 0)
                {
                    originalCenterVertices1[v1] = v;
                    originalCenterVertices1[v1].z = 0;
                    v1++;
                }
                else
                {
                    originalCenterVertices2[v2] = v;
                    originalCenterVertices2[v2].z = 0;
                    v2++;
                }
            }
            segmentVertices = originalCenterVertices1;

            segmentTriangles = originalCenterMesh.triangles;
        }
    }
    [Button]
    public void GenerateTestMeshSegment()
    {
        GetReferences();
        var mesh = new Mesh {
			name = "Procedural Wall Mesh"
		};
        GetOriginalMeshData();

        int segmentVertexCount = originalCenterMesh.vertexCount;
        Vector3[] vertices = new Vector3[segmentVertexCount];
        for(int v = 0; v < segmentVertexCount; v++)
        {
            vertices[v] = originalCenterMesh.vertices[v];
            if(vertices[v].z < 0)
            {
                vertices[v].z = 0;
                Vector3 tangent = Vector3.Cross(Vector3.up, positions[1].localPosition - positions[positions.Count-1].localPosition);
                vertices[v] = Quaternion.FromToRotation(Vector3.right, tangent) * vertices[v] + positions[0].localPosition;
            }
            else
            {
                vertices[v].z = 0;
                Vector3 tangent = Vector3.Cross(Vector3.up, positions[2].localPosition - positions[0].localPosition);
                vertices[v] = Quaternion.FromToRotation(Vector3.right, tangent) * vertices[v] + positions[1].localPosition;
            }
        }
        mesh.vertices = vertices;

        int[] triangles;
        triangles = segmentTriangles;
        mesh.triangles = triangles;
		meshFilter.mesh = mesh;
    }

    Vector3[] tangents;
    Vector3 GetTangent(int index)
    {
        if(index >= positions.Count)
            index -= positions.Count;
        print(index);
        if(tangents[index] == Vector3.zero)
        {
            Vector3 localPosition = positions[index].localPosition;
            Vector3 direction = positions[index].forward;
            if(index == 0)
            {
                direction = (positions[index+1].localPosition - localPosition).normalized;
                if(closed)
                    direction += (localPosition - positions[positions.Count-1].localPosition).normalized;
            }
            else if(index == positions.Count-1)
            {
                direction = (localPosition - positions[index-1].localPosition).normalized;
                if(closed)
                    direction += (positions[0].localPosition - localPosition).normalized;
            }
            else
            {
                direction = (positions[index+1].localPosition - localPosition).normalized;
                direction += (localPosition - positions[index-1].localPosition).normalized;
            }
            direction.Normalize();
            tangents[index] = Vector3.Cross(Vector3.up, direction);
        }
        return tangents[index];
    }

    [Button]
    public void GenerateTestMesh()
    {
        GetReferences();
        var mesh = new Mesh {
			name = "Procedural Wall Mesh"
		};

        //Make vertices
        if(positions.Count > 1)
        {
            GetOriginalMeshData();

            int segmentVertexCount = originalCenterMesh.vertexCount;

            Vector3 previousTangent = Vector3.right;
            Vector3[] vertices = new Vector3[(positions.Count-1) * segmentVertexCount];
            for(int i = 0; i < positions.Count-1; i++)
            {
                Vector3 localPosition = positions[i].localPosition;

                for(int v = 0; v < segmentVertexCount; v++)
                {
                    int index = segmentVertexCount * i + v;
                    vertices[index] = originalCenterMesh.vertices[v];
                    if(vertices[index].z < 0)
                    {
                        vertices[index].z = 0;
                        vertices[index] = Quaternion.FromToRotation(Vector3.right, GetTangent(i)) * vertices[index] + positions[i].localPosition;
                    }
                    else
                    {
                        vertices[index].z = 0;
                        vertices[index] = Quaternion.FromToRotation(Vector3.right, GetTangent(i+1)) * vertices[index] + positions[i+1].localPosition;
                    }
                }
            }
            mesh.vertices = vertices;
            int[] triangles;

            int segmentTriangleCount = segmentTriangles.Length;
            
            if(closed)
            {
                triangles = new int[positions.Count * segmentTriangleCount];

                for(int i = 0; i < positions.Count; i++)
                {
                    for(int t = 0; t < segmentTriangleCount; t++)
                    {
                        triangles[segmentTriangleCount * i + t] = segmentTriangleCount * i + segmentTriangles[t];
                    }
                }
            }
            else
            {
                triangles = new int[(positions.Count - 1) * segmentTriangleCount];
                //StartCap

                //center
                for(int i = 0; i < positions.Count-1; i++)
                {
                    for(int t = 0; t < segmentTriangleCount; t++)
                    {
                        triangles[segmentTriangleCount * i + t] = segmentVertexCount * i + segmentTriangles[t];
                    }
                }
                
                //EndCap
            }
            mesh.triangles = triangles;
        }
        else
        {          
            mesh.vertices = new Vector3[] {Vector3.zero, Vector3.up, Vector3.forward, Vector3.up + Vector3.forward};
            mesh.triangles = new int[] {0,1,2,1,3,2};
        }
        
		meshFilter.mesh = mesh;
    }


    [Button]
    public void GenerateRectMesh()
    {
        GetReferences();
        var mesh = new Mesh {
			name = "Procedural Wall Mesh"
		};

        //Make vertices
        if(positions.Count > 1)
        {
            GetOriginalMeshData();

            Vector3[] vertices = new Vector3[positions.Count * 4];
            for(int i = 0; i < positions.Count; i++)
            {
                Vector3 localPosition = positions[i].localPosition;
                Vector3 direction = positions[i].forward;
                if(i == 0)
                {
                    direction = (positions[i+1].localPosition - localPosition).normalized;
                    if(closed)
                        direction += (localPosition - positions[positions.Count-1].localPosition).normalized;
                }
                else if(i == positions.Count-1)
                {
                    direction = (localPosition - positions[i-1].localPosition).normalized;
                    if(closed)
                        direction += (positions[0].localPosition - localPosition).normalized;
                }
                else
                {
                    direction = (positions[i+1].localPosition - localPosition).normalized;
                    direction += (localPosition - positions[i-1].localPosition).normalized;
                }
                direction.Normalize();
                Vector3 tangent = Vector3.Cross(Vector3.up, direction);
                float halfWallThickness = wallThickness/2;
                vertices[4*i + 0] = localPosition + tangent * halfWallThickness;
                vertices[4*i + 1] = localPosition - tangent * halfWallThickness;
                vertices[4*i + 2] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
                vertices[4*i + 3] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;
            }
            mesh.vertices = vertices;
            int[] triangles;

            
            if(closed)
            {
                triangles = new int[positions.Count * 24];
                for(int i = 1; i < positions.Count; i++)
                {
                    //Side Right
                    triangles[24*i + 0] = 4*(i-1) + 0;
                    triangles[24*i + 1] = 4*(i-1) + 3;
                    triangles[24*i + 2] = 4* i + 0;
                    triangles[24*i + 3] = 4* i + 0;
                    triangles[24*i + 4] = 4* (i-1) + 3; 
                    triangles[24*i + 5] = 4* i + 3;

                    //Side Up
                    triangles[24*i + 6] = 4*(i-1) + 3;
                    triangles[24*i + 7] = 4*(i-1) + 2;
                    triangles[24*i + 8] = 4* i + 3;
                    triangles[24*i + 9] = 4* i + 3;
                    triangles[24*i + 10] = 4* (i-1) + 2; 
                    triangles[24*i + 11] = 4* i + 2;

                    //Side Left
                    triangles[24*i + 12] = 4* i + 2;
                    triangles[24*i + 13] = 4* (i-1) + 2;
                    triangles[24*i + 14] = 4* i + 1;
                    triangles[24*i + 15] = 4* i + 1;
                    triangles[24*i + 16] = 4* (i-1) + 2; 
                    triangles[24*i + 17] = 4* (i-1) + 1;

                    //Side Down
                    triangles[24*i + 18] = 4* (i-1) + 1;
                    triangles[24*i + 19] = 4* i + 0;
                    triangles[24*i + 20] = 4* i + 1;
                    triangles[24*i + 21] = 4* i + 0;
                    triangles[24*i + 22] = 4* (i-1) + 1; 
                    triangles[24*i + 23] = 4* (i-1) + 0;
                }

                int last = positions.Count-1;
                //Side Right
                triangles[0] = 4*last + 0;
                triangles[1] = 4*last + 3;
                triangles[2] = 4* 0 + 0;
                triangles[3] = 4* 0 + 0;
                triangles[4] = 4* last + 3; 
                triangles[5] = 4* 0 + 3;

                //Side Up
                triangles[6] = 4*last + 3;
                triangles[7] = 4*last + 2;
                triangles[8] = 4* 0 + 3;
                triangles[9] = 4* 0 + 3;
                triangles[10] = 4* last + 2; 
                triangles[11] = 4* 0 + 2;

                //Side Left
                triangles[12] = 4* 0 + 2;
                triangles[13] = 4* last + 2;
                triangles[14] = 4* 0 + 1;
                triangles[15] = 4* 0 + 1;
                triangles[16] = 4* last + 2; 
                triangles[17] = 4* last + 1;

                //Side Down
                triangles[18] = 4* last + 1;
                triangles[19] = 4* 0 + 0;
                triangles[20] = 4* 0 + 1;
                triangles[21] = 4* 0 + 0;
                triangles[22] = 4* last + 1; 
                triangles[23] = 4* last + 0;
            }
            else
            {
                triangles = new int[2 * 6 + (positions.Count - 1) * 24];
                //StartCap
                triangles[0] = 0;
                triangles[1] = 1;
                triangles[2] = 2;
                triangles[3] = 2;
                triangles[4] = 3;
                triangles[5] = 0;

                for(int i = 1; i < positions.Count; i++)
                {
                    //Side Right
                    triangles[6+24*(i-1) + 0] = 4*(i-1) + 0;
                    triangles[6+24*(i-1) + 1] = 4*(i-1) + 3;
                    triangles[6+24*(i-1) + 2] = 4* i + 0;
                    triangles[6+24*(i-1) + 3] = 4* i + 0;
                    triangles[6+24*(i-1) + 4] = 4* (i-1) + 3; 
                    triangles[6+24*(i-1) + 5] = 4* i + 3;

                    //Side Up
                    triangles[6+24*(i-1) + 6] = 4*(i-1) + 3;
                    triangles[6+24*(i-1) + 7] = 4*(i-1) + 2;
                    triangles[6+24*(i-1) + 8] = 4* i + 3;
                    triangles[6+24*(i-1) + 9] = 4* i + 3;
                    triangles[6+24*(i-1) + 10] = 4* (i-1) + 2; 
                    triangles[6+24*(i-1) + 11] = 4* i + 2;

                    //Side Left
                    triangles[6+24*(i-1) + 12] = 4* i + 2;
                    triangles[6+24*(i-1) + 13] = 4* (i-1) + 2;
                    triangles[6+24*(i-1) + 14] = 4* i + 1;
                    triangles[6+24*(i-1) + 15] = 4* i + 1;
                    triangles[6+24*(i-1) + 16] = 4* (i-1) + 2; 
                    triangles[6+24*(i-1) + 17] = 4* (i-1) + 1;

                    //Side Down
                    triangles[6+24*(i-1) + 18] = 4* (i-1) + 1;
                    triangles[6+24*(i-1) + 19] = 4* i + 0;
                    triangles[6+24*(i-1) + 20] = 4* i + 1;
                    triangles[6+24*(i-1) + 21] = 4* i + 0;
                    triangles[6+24*(i-1) + 22] = 4* (i-1) + 1; 
                    triangles[6+24*(i-1) + 23] = 4* (i-1) + 0;
                }
                
                //EndCap
                triangles[triangles.Length - 6] = vertices.Length - 2;
                triangles[triangles.Length - 5] = vertices.Length - 3;
                triangles[triangles.Length - 4] = vertices.Length - 4;
                triangles[triangles.Length - 3] = vertices.Length - 4;
                triangles[triangles.Length - 2] = vertices.Length - 1;
                triangles[triangles.Length - 1] = vertices.Length - 2;
            }
            mesh.triangles = triangles;
        }
        else
        {          
            mesh.vertices = new Vector3[] {Vector3.zero, Vector3.up, Vector3.forward, Vector3.up + Vector3.forward};
            mesh.triangles = new int[] {0,1,2,1,3,2};
        }
        
		meshFilter.mesh = mesh;
    }

    [Button]
    public void ClearMesh()
    {
        GetReferences();
        Destroy(meshFilter.mesh);
        meshFilter.mesh = null;
    }
}
