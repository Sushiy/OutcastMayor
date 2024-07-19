using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer)), ExecuteAlways]
public class ProceduralWallMesh : MonoBehaviour
{
    public List<Transform> positions;

    public float wallThickness;
    public float wallheight;
    MeshFilter meshFilter;

    public bool closed = false;

    public Mesh originalCenterMesh;
    float originalMeshCenterLength;
    Vector3[] originalCenterVertices1;
    Vector3[] originalCenterVertices2;
    int[] segmentTriangles;

    Vector3[] segmentVertices;

    public bool windowChanged = false;
    public void Update()
    {
        if(windowChanged)
        {
            windowPosition.hasChanged = false;
            GenerateRectWindowSegment();
        }
        windowChanged = windowPosition.hasChanged;
    }

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
            float minZ = float.PositiveInfinity;
            float maxZ = float.NegativeInfinity;
            for(int i = 0; i < originalCenterMesh.vertexCount; i++)
            {
                Vector3 v = originalCenterMesh.vertices[i];
                if(v.z < minZ)
                {
                    minZ = v.z;
                }
                else if(v.z > maxZ)
                {
                    maxZ = v.z;
                }
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
            originalMeshCenterLength = Mathf.Abs(maxZ - minZ)/2;
            print("[GetOriginalMeshData] centerLength: " + originalMeshCenterLength);
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
        if(reusableMesh == null)
        reusableMesh = new Mesh {
			name = "Procedural Wall Mesh"
		};

        //Make vertices
        if(positions.Count > 1)
        {
            GetOriginalMeshData();

            int segmentVertexCount = originalCenterMesh.vertexCount;

            Vector3 previousTangent = Vector3.right;
            //Vector3[] vertices = new Vector3[(positions.Count-1) * segmentVertexCount];
            List<Vector3> vertices = new List<Vector3>();
            int totalVertexCount = 0;
            int previousTotalVertexCount = 0;
            int totalSubsegments = 0;
            for(int i = 0; i < positions.Count-1; i++)
            {
                Vector3 localPosition = positions[i].localPosition;
                Vector3 localEndPosition = positions[i+1].localPosition;
                Vector3 direction = localEndPosition - localPosition;
                float segmentLen = Vector3.Distance(localPosition, localEndPosition);
                direction.Normalize();
                Vector3 segmentTangent = Vector3.Cross(Vector3.up, direction);

                //calculate how many subsegments you would need
                float subSegmentCount = segmentLen/originalMeshCenterLength;
                int roundSubSegmentCount = Mathf.Max(1, Mathf.RoundToInt(subSegmentCount));
                float subSegmentLen = segmentLen/roundSubSegmentCount;
                print("SegmentLen:" + segmentLen + " subsegmentCount:" + subSegmentCount + " flooredCount:" + roundSubSegmentCount + " subgsementLen:" + subSegmentLen);

                for(int subSegment = 0; subSegment < (roundSubSegmentCount); subSegment++)
                {
                    totalSubsegments++;
                    totalVertexCount += segmentVertexCount;
                    vertices.AddRange(new Vector3[segmentVertexCount]);
                    Vector3 subsegmentStartPosition = localPosition +  direction * subSegment * subSegmentLen;
                    Vector3 subsegmentEndPosition = localPosition + direction * (subSegment+1) * subSegmentLen ;
                    print("Subsegment: " + subSegment + " subsegmentStartPosition:" + subsegmentStartPosition + " subsegmentEndPosition:" + subsegmentEndPosition);
                    for(int v = 0; v < segmentVertexCount; v++)
                    {
                        int index = previousTotalVertexCount + v;      
                        Vector3 vertex = originalCenterMesh.vertices[v];
                        if(vertex.z < 0)
                        {
                            vertex.z = 0;
                            if(subSegment == 0)
                            {
                                vertex.x /= Vector3.Dot(segmentTangent, GetTangent(i));
                                vertex = Quaternion.FromToRotation(Vector3.right, GetTangent(i)) * vertex + subsegmentStartPosition;
                            }
                            else
                            {
                                vertex = Quaternion.FromToRotation(Vector3.right, segmentTangent) * vertex + subsegmentStartPosition;
                            }

                        }
                        else
                        {
                            vertex.z = 0;
                            if(subSegment == (roundSubSegmentCount - 1))
                            {
                                vertex.x /= Vector3.Dot(segmentTangent, GetTangent(i+1));
                                vertex = Quaternion.FromToRotation(Vector3.right, GetTangent(i+1)) * vertex + localEndPosition;
                            }
                            else
                            {
                                vertex = Quaternion.FromToRotation(Vector3.right, segmentTangent) * vertex + subsegmentEndPosition;
                            }
                        }
                        vertices[index] = vertex;
                        Debug.DrawRay(transform.TransformPoint(vertices[index]), Vector3.up*.25f, Color.red, 1.0f);
                    }
                    previousTotalVertexCount = totalVertexCount;
                }
            }
            print("TotalSubsegments: " + totalSubsegments);
            reusableMesh.vertices = vertices.ToArray();


            //TRIANGLES
            int[] triangles;

            int segmentTriangleCount = segmentTriangles.Length;
            
            if(closed)
            {
                return;
                triangles = new int[positions.Count * segmentTriangleCount];

                for(int i = 0; i < positions.Count; i++)
                {
                    Vector3 localPosition = positions[i].localPosition;
                    Vector3 localEndPosition = (i == positions.Count-1) ? positions[0].localPosition : positions[i+1].localPosition;

                    float segmentLen = Vector3.Distance(localPosition, localEndPosition);
                    float subSegmentCount = segmentLen/originalMeshCenterLength;
                    int flooredSubSegmentCount = Mathf.FloorToInt(subSegmentCount);

                    for(int t = 0; t < segmentTriangleCount; t++)
                    {
                        triangles[segmentTriangleCount * i + t] = segmentTriangleCount * i + segmentTriangles[t];
                    }
                }
            }
            else
            {
                triangles = new int[totalSubsegments * segmentTriangleCount];
                //StartCap

                //center
                for(int i = 0; i < totalSubsegments; i++)
                {
                    for(int t = 0; t < segmentTriangleCount; t++)
                    {
                        triangles[segmentTriangleCount * i + t] = segmentVertexCount * i + segmentTriangles[t];
                    }
                }
                
                //EndCap
            }
            reusableMesh.triangles = triangles;
        }
        else
        {          
            reusableMesh.vertices = new Vector3[] {Vector3.zero, Vector3.up, Vector3.forward, Vector3.up + Vector3.forward};
            reusableMesh.triangles = new int[] {0,1,2,1,3,2};
        }
        
		meshFilter.sharedMesh = reusableMesh;
    }
    Mesh reusableMesh; 
    public Transform windowPosition;

    void AddFullSegment(ref Vector3[] vertices, ref int[] indices)
    {

    }

    [Button]
    public void GenerateRectWindowSegment()
    {
        GetReferences();
        if(reusableMesh == null)
        reusableMesh = new Mesh {
			name = "Procedural Wall Mesh"
		};

        float halfWallThickness = wallThickness/2;

        //Place window against segment
        Vector3 segmentTangent = GetTangent(0);
        Plane p = new Plane(segmentTangent,positions[0].position);
        windowPosition.position = p.ClosestPointOnPlane(windowPosition.position);
        windowPosition.LookAt(windowPosition.position + segmentTangent);

        int windowCount = 1;

        Vector3[] vertices = new Vector3[(positions.Count * 4) * (1 + windowCount) + (windowCount * 16)];

        //Build segment until window   
        Vector3 localPosition = positions[0].localPosition;
        Vector3 direction = (positions[1].localPosition - positions[0].localPosition).normalized;
        Vector3 tangent = GetTangent(0); 
        vertices[4*0 + 0] = localPosition + tangent * halfWallThickness;
        vertices[4*0 + 1] = localPosition - tangent * halfWallThickness;
        vertices[4*0 + 2] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*0 + 3] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;

        localPosition = windowPosition.localPosition - direction * windowPosition.localScale.x/2;
        localPosition.y = positions[0].localPosition.y;
        vertices[4*1 + 0] = localPosition + tangent * halfWallThickness;
        vertices[4*1 + 1] = localPosition - tangent * halfWallThickness;
        vertices[4*1 + 2] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*1 + 3] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;

        //Build segment of window
        Vector3 botToWindow = Vector3.up * (windowPosition.localPosition.y - positions[0].localPosition.y - windowPosition.localScale.y/2);
        Vector3 topToWindow = Vector3.up * (windowPosition.localPosition.y - positions[0].localPosition.y + windowPosition.localScale.y/2);
        
        localPosition = windowPosition.localPosition - direction * windowPosition.localScale.x/2;
        localPosition.y = positions[0].localPosition.y;
        vertices[4*2 + 0] = localPosition + tangent * halfWallThickness;
        vertices[4*2 + 1] = localPosition - tangent * halfWallThickness;
        vertices[4*2 + 2] = localPosition - tangent * halfWallThickness + botToWindow;
        vertices[4*2 + 3] = localPosition + tangent * halfWallThickness + botToWindow;

        localPosition = windowPosition.localPosition + direction * windowPosition.localScale.x/2;
        localPosition.y = positions[0].localPosition.y;
        vertices[4*2 + 4] = localPosition + tangent * halfWallThickness;
        vertices[4*2 + 5] = localPosition - tangent * halfWallThickness;
        vertices[4*2 + 6] = localPosition - tangent * halfWallThickness + botToWindow;
        vertices[4*2 + 7] = localPosition + tangent * halfWallThickness + botToWindow;
        
        localPosition = windowPosition.localPosition - direction * windowPosition.localScale.x/2;
        localPosition.y = positions[0].localPosition.y;
        vertices[4*2 + 8] = localPosition + tangent * halfWallThickness + topToWindow;
        vertices[4*2 + 9] = localPosition - tangent * halfWallThickness  + topToWindow;
        vertices[4*2 + 10] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*2 + 11] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;
        
        localPosition = windowPosition.localPosition + direction * windowPosition.localScale.x/2;
        localPosition.y = positions[0].localPosition.y;
        vertices[4*2 + 12] = localPosition + tangent * halfWallThickness + topToWindow;
        vertices[4*2 + 13] = localPosition - tangent * halfWallThickness + topToWindow;
        vertices[4*2 + 14] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*2 + 15] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;

        //Build segment after window
        vertices[4*2 + 16 + 0] = localPosition + tangent * halfWallThickness;
        vertices[4*2 + 16 + 1] = localPosition - tangent * halfWallThickness;
        vertices[4*2 + 16 + 2] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*2 + 16 + 3] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;

        localPosition = positions[1].localPosition;
        vertices[4*3 + 16 + 0] = localPosition + tangent * halfWallThickness;
        vertices[4*3 + 16 + 1] = localPosition - tangent * halfWallThickness;
        vertices[4*3 + 16 + 2] = localPosition - tangent * halfWallThickness + Vector3.up * wallheight;
        vertices[4*3 + 16 + 3] = localPosition + tangent * halfWallThickness + Vector3.up * wallheight;

        reusableMesh.vertices = vertices;
        int[] triangles;

         triangles = new int[2 * 6 + ((positions.Count - 1) * 24)*2 + 18*2];
        //StartCap
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        //Before Window Segment
        int fullSegmentCount = 1;
        int windowSegmentCount = 
        //Side Right
        triangles[6 + 24*0 + 0] = 4*(fullSegmentCount-1) + 0;
        triangles[6 + 24*0 + 1] = 4*(fullSegmentCount-1) + 3;
        triangles[6 + 24*0 + 2] = 4* fullSegmentCount + 0;
        triangles[6 + 24*0 + 3] = 4* fullSegmentCount + 0;
        triangles[6 + 24*0 + 4] = 4* (fullSegmentCount-1) + 3; 
        triangles[6 + 24*0 + 5] = 4* fullSegmentCount + 3;
  
        //Side Up  
        triangles[6 + 24*0 + 6] = 4*(fullSegmentCount-1) + 3;
        triangles[6 + 24*0 + 7] = 4*(fullSegmentCount-1) + 2;
        triangles[6 + 24*0 + 8] = 4* fullSegmentCount + 3;
        triangles[6 + 24*0 + 9] = 4* fullSegmentCount + 3;
        triangles[6 + 24*0 + 10] = 4* (fullSegmentCount-1) + 2; 
        triangles[6 + 24*0 + 11] = 4* fullSegmentCount + 2;
  
        //Side Left  
        triangles[6 + 24*0 + 12] = 4* fullSegmentCount + 2;
        triangles[6 + 24*0 + 13] = 4* (fullSegmentCount-1) + 2;
        triangles[6 + 24*0 + 14] = 4* fullSegmentCount + 1;
        triangles[6 + 24*0 + 15] = 4* fullSegmentCount + 1;
        triangles[6 + 24*0 + 16] = 4* (fullSegmentCount-1) + 2; 
        triangles[6 + 24*0 + 17] = 4* (fullSegmentCount-1) + 1;
  
        //Side Down  
        triangles[6 + 24*0 + 18] = 4* (fullSegmentCount-1) + 1;
        triangles[6 + 24*0 + 19] = 4* fullSegmentCount + 0;
        triangles[6 + 24*0 + 20] = 4* fullSegmentCount + 1;
        triangles[6 + 24*0 + 21] = 4* fullSegmentCount + 0;
        triangles[6 + 24*0 + 22] = 4* (fullSegmentCount-1) + 1; 
        triangles[6 + 24*0 + 23] = 4* (fullSegmentCount-1) + 0;

        
        //Window Segment
        //Side Right
        triangles[6 + 24*1 + 0] = 4* 2 + 0;
        triangles[6 + 24*1 + 1] = 4* 2 + 3;
        triangles[6 + 24*1 + 2] = 4* 3 + 0;
        triangles[6 + 24*1 + 3] = 4* 3 + 0;
        triangles[6 + 24*1 + 4] = 4* 2 + 3; 
        triangles[6 + 24*1 + 5] = 4* 3 + 3;
  
        //Side Down  
        triangles[6 + 24*1 + 6] = 4* 2 + 1;
        triangles[6 + 24*1 + 7] = 4* 3 + 0;
        triangles[6 + 24*1 + 8] = 4* 3 + 1;
        triangles[6 + 24*1 + 9] = 4* 3 + 0;
        triangles[6 + 24*1 + 10] = 4* 2 + 1; 
        triangles[6 + 24*1 + 11] = 4* 2 + 0;
  
        //Side Left  
        triangles[6 + 24*1 + 12] = 4* 3 + 2;
        triangles[6 + 24*1 + 13] = 4* 2 + 2;
        triangles[6 + 24*1 + 14] = 4* 3 + 1;
        triangles[6 + 24*1 + 15] = 4* 3 + 1;
        triangles[6 + 24*1 + 16] = 4* 2 + 2; 
        triangles[6 + 24*1 + 17] = 4* 2 + 1;

        
        //Side Right
        triangles[6 + 24*1+ 18 + 0] = 4* 4 + 0;
        triangles[6 + 24*1+ 18 + 1] = 4* 4 + 3;
        triangles[6 + 24*1+ 18 + 2] = 4* 5 + 0;
        triangles[6 + 24*1+ 18 + 3] = 4* 5 + 0;
        triangles[6 + 24*1+ 18 + 4] = 4* 4 + 3; 
        triangles[6 + 24*1+ 18 + 5] = 4* 5 + 3;

        //Side Up
        triangles[6 + 24*1+ 18 + 6] = 4* 4 + 3;
        triangles[6 + 24*1+ 18 + 7] = 4* 4 + 2;
        triangles[6 + 24*1+ 18 + 8] = 4* 5 + 3;
        triangles[6 + 24*1+ 18 + 9] = 4* 5 + 3;
        triangles[6 + 24*1+ 18 + 10] = 4* 4 + 2; 
        triangles[6 + 24*1+ 18 + 11] = 4* 5 + 2;

        //Side Left
        triangles[6 + 24*1+ 18 + 12] = 4* 5 + 2;
        triangles[6 + 24*1+ 18 + 13] = 4* 4 + 2;
        triangles[6 + 24*1+ 18 + 14] = 4* 5 + 1;
        triangles[6 + 24*1+ 18 + 15] = 4* 5 + 1;
        triangles[6 + 24*1+ 18 + 16] = 4* 4 + 2; 
        triangles[6 + 24*1+ 18 + 17] = 4* 4 + 1;

        //After Window Segment
        fullSegmentCount = 3;
        //Side Right
        triangles[36 + 6 + 24*1 + 0] = 16 + 4*(fullSegmentCount-1) + 0;
        triangles[36 + 6 + 24*1 + 1] = 16 + 4*(fullSegmentCount-1) + 3;
        triangles[36 + 6 + 24*1 + 2] = 16 + 4* fullSegmentCount + 0;
        triangles[36 + 6 + 24*1 + 3] = 16 + 4* fullSegmentCount + 0;
        triangles[36 + 6 + 24*1 + 4] = 16 + 4* (fullSegmentCount-1) + 3; 
        triangles[36 + 6 + 24*1 + 5] = 16 + 4* fullSegmentCount + 3;
  
        //Side Up  
        triangles[36 + 6 + 24*1 + 6] = 16 + 4*(fullSegmentCount-1) + 3;
        triangles[36 + 6 + 24*1 + 7] = 16 + 4*(fullSegmentCount-1) + 2;
        triangles[36 + 6 + 24*1 + 8] = 16 + 4* fullSegmentCount + 3;
        triangles[36 + 6 + 24*1 + 9] = 16 + 4* fullSegmentCount + 3;
        triangles[36 + 6 + 24*1 + 10] = 16 + 4* (fullSegmentCount-1) + 2; 
        triangles[36 + 6 + 24*1 + 11] = 16 + 4* fullSegmentCount + 2;
  
        //Side Left  
        triangles[36 + 6 + 24*1 + 12] = 16 + 4* fullSegmentCount + 2;
        triangles[36 + 6 + 24*1 + 13] = 16 + 4* (fullSegmentCount-1) + 2;
        triangles[36 + 6 + 24*1 + 14] = 16 + 4* fullSegmentCount + 1;
        triangles[36 + 6 + 24*1 + 15] = 16 + 4* fullSegmentCount + 1;
        triangles[36 + 6 + 24*1 + 16] = 16 + 4* (fullSegmentCount-1) + 2; 
        triangles[36 + 6 + 24*1 + 17] = 16 + 4* (fullSegmentCount-1) + 1;

        //Side Down
        triangles[36 + 6 + 24*1 + 18] = 16 + 4* (fullSegmentCount-1) + 1;
        triangles[36 + 6 + 24*1 + 19] = 16 + 4* fullSegmentCount + 0;
        triangles[36 + 6 + 24*1 + 20] = 16 + 4* fullSegmentCount + 1;
        triangles[36 + 6 + 24*1 + 21] = 16 + 4* fullSegmentCount + 0;
        triangles[36 + 6 + 24*1 + 22] = 16 + 4* (fullSegmentCount-1) + 1; 
        triangles[36 + 6 + 24*1 + 23] = 16 + 4* (fullSegmentCount-1) + 0;

        
        //EndCap
        triangles[triangles.Length - 6] = vertices.Length - 2;
        triangles[triangles.Length - 5] = vertices.Length - 3;
        triangles[triangles.Length - 4] = vertices.Length - 4;
        triangles[triangles.Length - 3] = vertices.Length - 4;
        triangles[triangles.Length - 2] = vertices.Length - 1;
        triangles[triangles.Length - 1] = vertices.Length - 2;
        reusableMesh.triangles = triangles;

		meshFilter.mesh = reusableMesh;
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
                Vector3 line = (i < positions.Count-1) ? positions[i+1].localPosition - localPosition : localPosition - positions[i-1].localPosition;
                line.Normalize();
                Vector3 normal = Vector3.Cross(Vector3.up, line).normalized;
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

                float halfWallThickness = (wallThickness/2)/Vector3.Dot(normal, tangent);
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
