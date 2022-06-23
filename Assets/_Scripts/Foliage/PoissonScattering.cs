using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Mesh surface Poisson from https://github.com/zewt/maya-implicit-skinning/blob/master/src/meshes/vcg_lib/utils_sampling.cpp
/// and 
/// </summary>

public class PoissonScattering : MonoBehaviour
{
    [Header("Poisson Sampling")]
    public bool showPoissonPoints = false;
    public float radius = 1;

    //public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;

    public float displayRadius = 1;

    List<Vector2> points;

    public int sampleCount = 30;
    List<PoissonSample> poissonPoints;
    List<SamplePoint> rawSamples;


    MeshFilter meshFilter;
    [SerializeField]Mesh originalMesh;
    Mesh newMesh;

    [ReadOnly]
    public int samplePointCount;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        poissonPoints = new List<PoissonSample>();
    }

    private void OnDrawGizmos()
    {
        if (showPoissonPoints && poissonPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (PoissonSample sample in poissonPoints)
            {
                Gizmos.DrawSphere(transform.TransformPoint(sample.position), displayRadius);
                Gizmos.DrawRay(transform.TransformPoint(sample.position), transform.TransformDirection(sample.normal) * .1f);
            }
        }
    }
    /*
    #region PoissonSampling 2D
    [Button]
    public void GenerateGrid()
    {
        points = PoissonSamplingFlat(radius, regionSize, rejectionSamples);
    }
    public static List<Vector2> PoissonSamplingFlat(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        //Create the grid
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
        Debug.Log(grid.GetLength(0));
        List<Vector2> points = new List<Vector2>();

        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(sampleRegionSize / 2);

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];

            bool candidateAccepted = false;
            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);
                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }


        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        //is the point in the region?
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x < searchEndX; x++)
            {
                for (int y = searchStartY; y < searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float distance = (candidate - points[pointIndex]).sqrMagnitude;
                        if (distance < radius * radius)
                        {
                            return false;
                        }
                    }

                }
            }
            return true;
        }
        return false;
    }
    #endregion
    */
    #region PoissonSampling On Meshsurface
    [Button]
    public void GenerateGridOnMesh()
    {
        if(meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        if(poissonPoints == null)
        {
            poissonPoints = new List<PoissonSample>();
        }
        if(rawSamples == null)
        {
            rawSamples = new List<SamplePoint>();
        }
        if(meshFilter != null && meshFilter.sharedMesh != null)
        {
            rawSamples.Clear();
            poissonPoints.Clear();
            meshFilter.mesh = originalMesh;
            CreateRawSamples(sampleCount, meshFilter.sharedMesh.vertices, meshFilter.sharedMesh.normals, meshFilter.sharedMesh.GetIndices(0), rawSamples);

            PoissonDiskFromSamples(radius, rawSamples, poissonPoints);

            samplePointCount = poissonPoints.Count;
        }
    }


    /// <summary>
    /// Estimate the geodesic distance between two points using their position and normals.
    /// See c95-f95_199-a16-paperfinal-v5 "3.2 Sampling with Geodesic Distance".  This estimation
    /// assumes that we have a smooth gradient between the two points, since it doesn't have any
    /// information about complex surface changes between the points.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    /// <returns></returns>
    static float ApproximateGeodesicDistance(Vector3 p1, Vector3 p2, Vector3 n1, Vector3 n2)
    {
        float result = (p2 - p1).sqrMagnitude;

        n1.Normalize();
        n2.Normalize();

        Vector3 v = (p2 - p1).normalized;

        float c1 = Vector3.Dot(n1, v);
        float c2 = Vector3.Dot(n2, v);

        //Check for division by zero
        if(Mathf.Abs(c1-c2) > 0.0001f)
        {
            result *= (Mathf.Asin(c1) - Mathf.Asin(c2)) / (c1 - c2);
        }
        return result;
    }

    public struct SamplePoint
    {
        public int triangleIndex;
        public int cellIndex;
        public Vector3 position;
        public Vector3 normal;

        public override string ToString()
        {
            return "Tri: " + triangleIndex + " cell " + cellIndex + " pos: " + position;
        }
    };

    // Do a simple random sampling of the triangles.  Return the total surface area of the triangles.
    static float CreateRawSamples(int numberOfSamples, Vector3[] vertices, Vector3[] normals, int[] vertexIndices, List<SamplePoint> samples)
    {
        samples.Clear();
        // Calculate the area of each triangle.  We'll use this to randomly select triangles with probability proportional
        // to their area.
        List<float> triangleAreas = new List<float>();
        for(int tri = 0; tri < vertexIndices.Length; tri+=3)
        {
            int i0 = vertexIndices[tri + 0];
            int i1 = vertexIndices[tri + 1];
            int i2 = vertexIndices[tri + 2];

            triangleAreas.Add(AreaOfTriangle(vertices[i0], vertices[i1], vertices[i2]));
        }
        // Map the sums of the areas to a triangle index.  For example, if we have triangles with areas
        // 2, 7, 1 and 4, create:
        //
        // 0: 0
        // 2: 1
        // 9: 2
        // 10: 3
        //
        // A random number in 0...14 can then be mapped to an index.
        //A bigger triangle covers more numbers and is therefore more likely to be picked.
        Dictionary<float, int> areaSumToIndex = new Dictionary<float, int>();
        float areaSum = 0;
        for(int i = 0; i < triangleAreas.Count; i++)
        {
            if(triangleAreas[i] == 0)
            {
                Debug.LogError("There's a zero-sized triangle");
                continue;
            }
            areaSumToIndex.Add(areaSum, i);
            areaSum += triangleAreas[i];
        }

        for(int i = 0;  i < numberOfSamples; i++)
        {
            //Select a random triangle
            int triangleIndex = FindRandomTriangleFromWeightedArea(areaSum, areaSumToIndex);

            //VertexIndices
            int i0 = vertexIndices[triangleIndex * 3 + 0];
            int i1 = vertexIndices[triangleIndex * 3 + 1];
            int i2 = vertexIndices[triangleIndex * 3 + 2];
            //Vertices
            Vector3 v0 = vertices[i0];
            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            //Normals
            Vector3 n0 = normals[i0];
            Vector3 n1 = normals[i1];
            Vector3 n2 = normals[i2];

            //Select a random point on the triangle
            float u = Random.Range(0f, 1f);
            float v = Random.Range(0f, 1f);

            Vector3 point =     v0 * (1 - Mathf.Sqrt(u)) +
                                v1 * (Mathf.Sqrt(u) * (1 - v)) +
                                v2 * (v * Mathf.Sqrt(u));

            //Calculate normal for pos (CHECK!!! Correct?)
            Vector3 normal =    n0 * (1 - Mathf.Sqrt(u)) +
                                n1 * (Mathf.Sqrt(u) * (1 - v)) +
                                n2 * (v * Mathf.Sqrt(u));
            normal.Normalize();

            SamplePoint p;
            p.cellIndex = -1;
            p.position = point;
            p.normal = normal;
            p.triangleIndex = triangleIndex;
            samples.Add(p);
        }

        Debug.Log("Generated " + samples.Count + " raw samples");

        return areaSum;
    }

    public struct PoissonSample
    {
        public Vector3 position;
        public Vector3 normal;

        public PoissonSample(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }
    }

    struct HashData
    {
        // Resulting output sample points for this cell:
        public List<PoissonSample> poisson_samples;

        // Index into raw_samples:
        public int firstSampleIndex;
        public int sampleCount;

        public HashData(int firstSampleIndex, int sampleCount)
        {
            poisson_samples = new List<PoissonSample>();
            this.firstSampleIndex = firstSampleIndex;
            this.sampleCount = sampleCount;
        }
    }

    public static void PoissonDiskFromSamples(float radius, List<SamplePoint> rawSamples, List<PoissonSample> result)
    {
        //Get the boundingBox of the samples
        Bounds bounds = new Bounds();
        foreach(SamplePoint p in rawSamples)
        {
            bounds.Encapsulate(p.position);
        }

        Vector3 boundsSize = bounds.size;

        float radiusSquared = radius * radius;

        float cellSize = radius / Mathf.Sqrt(2);

        Vector3 gridSize = boundsSize / radius;
        Vector3 gridSizeInt = new Vector3(Mathf.Max((int)gridSize.x, 1), Mathf.Max((int)gridSize.y, 1), Mathf.Max((int)gridSize.z, 1));

        Debug.Log("BoundsSize: " + boundsSize + " GridSize: " + gridSizeInt);
        //Assign a cellID for each sample
        for (int i = 0; i < rawSamples.Count; i++)
        {
            SamplePoint p = rawSamples[i];
            Vector3 cellIndexVector = GetCellIndex(cellSize, p.position);
            p.cellIndex = CellIndexVectorToLinear(gridSizeInt, cellIndexVector);
            rawSamples[i] = p;
        }

        rawSamples.Sort((s1, s2) => s1.cellIndex.CompareTo(s2.cellIndex));

        string s = "";
        foreach(var sample in rawSamples)
        {
            s += sample.ToString() + " \n";
        }
        Debug.Log(s);


        // Map from cell IDs to hash_data.  Each hash_data points to the range in raw_samples corresponding to that cell.
        // (We could just store the samples in hash_data.  This implementation is an artifact of the reference paper, which
        // is optimizing for GPU acceleration that we haven't implemented currently.)

        Dictionary<int, HashData> cells = new Dictionary<int, HashData>();

        int lastID = -1;
        HashData lastHashData = new HashData();
        for (int i = 0; i < rawSamples.Count; i++)
        {
            SamplePoint samplePoint = rawSamples[i];
            if (samplePoint.cellIndex == lastID)
            {
                // This sample is in the same cell as the previous, so just increase the count.  Cells are
                // always contiguous, since we've sorted raw_samples by cell ID.
                lastHashData.sampleCount++;
                cells[lastID] = lastHashData;
                continue;
            }

            //This is a new Cell
            lastHashData = new HashData(i, 1);
            cells.Add(samplePoint.cellIndex, lastHashData);
            lastID = samplePoint.cellIndex;
        }

        Debug.Log(cells.Keys.Count + " Cells have samples");

        //Make a list of offsets to neighboring cell indexes, and to ourself(0)
        List<int> neighborCellIndices = new List<int>();
        
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                for(int z = -1; z <= 1; z++)
                {
                    neighborCellIndices.Add(CellIndexVectorToLinear(gridSizeInt, new Vector3(x, y, z)));
                }
            }
        }

        int maxTrials = 5;

        for (int trial = 0; trial < maxTrials; trial++)
        {
            //Create samplePoints for each entry in cells
            foreach (KeyValuePair<int, HashData> pair in cells)
            {
                int cellID = pair.Key;
                HashData data = pair.Value;

                // This cell's raw sample points start at firstSampleIndex.  On trial 0, try the first one.
                // On trial 1, try firstSampleIndex + 1.
                int nextSampleIndex = data.firstSampleIndex + trial;

                if(trial >= data.sampleCount)
                {
                    //There are no more points to try for this cell
                    continue;
                }

                var candidate = rawSamples[nextSampleIndex];

                // See if this point conflicts with any other points in this cell, or with any points in
                // neighboring cells.  Note that it's possible to have more than one point in the same cell.
                bool conflict = false;
                foreach(int neighborOffset in neighborCellIndices)
                {
                    int neighborCellID = cellID + neighborOffset;
                    HashData neighborData;
                    if (!cells.TryGetValue(neighborCellID, out neighborData))
                        continue;
                    foreach(var sample in neighborData.poisson_samples)
                    {
                        float distance = ApproximateGeodesicDistance(sample.position, candidate.position, sample.normal, candidate.normal);
                        if(distance < radiusSquared)
                        {
                            // The candidate is too close to this existing sample.
                            conflict = true;
                            break;
                        }
                    }
                    if (conflict)
                        break;
                }
                if (conflict)
                    continue;

                //If it didn't conflict, Store the new sample
                data.poisson_samples.Add(new PoissonSample(candidate.position, candidate.normal));
            }
        }

        //Copy the results to the output
        foreach(var cell in cells)
        {
            foreach(var sample in cell.Value.poisson_samples)
            {
                result.Add(new PoissonSample(sample.position, sample.normal));
            }
        }
    }

    static int CellIndexVectorToLinear(Vector3 gridSize, Vector3 cellIndexVector)
    {
        return (int)(cellIndexVector.x + gridSize.x * (cellIndexVector.y + gridSize.y * cellIndexVector.z));
    }

    static Vector3 GetCellIndex(float cellSize, Vector3 position)
    {
        return new Vector3((int)(position.x / cellSize), (int)(position.y / cellSize), (int)(position.z / cellSize));
    }

    static float AreaOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float a = (p1 - p2).magnitude;
        float b = (p2 - p3).magnitude;
        float c = (p3 - p1).magnitude;
        float s = (a + b + c) / 2;
        return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
    }

    static int FindRandomTriangleFromWeightedArea(float areaSum, Dictionary<float, int> areaSumToIndex)
    {
        float r = Random.Range(0f, areaSum);
        float bestDistance = Mathf.Infinity;
        float bestKey = 0;
        foreach(float key in areaSumToIndex.Keys)
        {
            float distance = r - key;
            if(distance >= 0 && distance < bestDistance)
            {
                bestKey = key;
                if (distance == 0)
                    break;
            }
        }

        return areaSumToIndex[bestKey];
    }
    #endregion

    #region MeshGeneration

    [Header("MeshGeneration")]
    public float quadWidth = .1f;
    public float quadHeight = .1f;

    [Button]
    public void GenerateMesh()
    {
        newMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>() ;
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        for(int i = 0; i < poissonPoints.Count; i++)
        {
            PoissonSample sample = poissonPoints[i];
            vertices.AddRange(new Vector3[4]
            {
                sample.position + new Vector3(-quadWidth, -quadHeight, 0),
                sample.position + new Vector3(quadWidth, -quadHeight, 0),
                sample.position + new Vector3(-quadWidth, quadHeight, 0),
                sample.position + new Vector3(quadWidth, quadHeight, 0)
            });

            tris.AddRange(new int[6]
            {
                // lower left triangle
                i*4+0, i*4+2, i*4+1,
                // upper right triangle
                i*4+2, i*4+3, i*4+1
            });

            normals.AddRange(new Vector3[4]
            {
                sample.normal,
                sample.normal,
                sample.normal,
                sample.normal
            });
            uv.AddRange( new Vector2[4]
            {
                  new Vector2(0, 0),
                  new Vector2(1, 0),
                  new Vector2(0, 1),
                  new Vector2(1, 1)
            });
        }
        newMesh.vertices = vertices.ToArray();
        newMesh.triangles = tris.ToArray();
        newMesh.normals = normals.ToArray();
        newMesh.uv = uv.ToArray();

        meshFilter.mesh = newMesh;
    }

    #endregion

}
