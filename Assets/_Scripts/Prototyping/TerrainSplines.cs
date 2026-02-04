using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class TerrainAdjusterRuntime : MonoBehaviour
{
    public Terrain terrain;

    [Range(0f,1f)]
    public float brushFallOff = 0.3f;

    [Range(1f,10f)]
    public float brushSpacing = 1f;

    [HideInInspector]
    public SplineContainer splineContainer;
    Spline spline;

    private float[,] originalTerrainHeights;

    // the blur radius values being used for the various passes
    public int[] initialPassRadii = { 15, 7, 2 };

    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        spline = splineContainer.Spline;

        if( splineContainer == null)
            Debug.LogError("Script must be attached to a SplineContainer GameObject");

    }

    public void SaveOriginalTerrainHeights()
    {
        if (terrain == null || splineContainer == null)
            return;

        Debug.Log("Saving original terrain data");

        TerrainData terrainData = terrain.terrainData;

        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;

        originalTerrainHeights = terrainData.GetHeights(0, 0, w, h);

    }

    public void CleanUp()
    {
        originalTerrainHeights = null;
        Debug.Log("Deleting original terrain data");
    }

    public void ShapeTerrain()
    {
        if (terrain == null || splineContainer == null)
            return;

        // save original terrain in case the terrain got added later
        if (originalTerrainHeights == null)
            SaveOriginalTerrainHeights();

        Vector3 terrainPosition = terrain.gameObject.transform.position;
        TerrainData terrainData = terrain.terrainData;

        // both GetHeights and SetHeights use normalized height values, where 0.0 equals to terrain.transform.position.y in the world space and 1.0 equals to terrain.transform.position.y + terrain.terrainData.size.y in the world space
        // so when using GetHeight you have to manually divide the value by the Terrain.activeTerrain.terrainData.size.y which is the configured height ("Terrain Height") of the terrain.
        float terrainMin = terrain.transform.position.y + 0f;
        float terrainMax = terrain.transform.position.y + terrain.terrainData.size.y;
        float totalHeight = terrainMax - terrainMin;

        int w = terrainData.heightmapResolution;
        int h = terrainData.heightmapResolution;

        // clone the original data, the modifications along the path are based on them
        float[,] allHeights = originalTerrainHeights.Clone() as float[,];

        // the blur radius values being used for the various passes
        for (int pass = 0; pass < initialPassRadii.Length; pass++)
        {
            int radius = initialPassRadii[pass];

            // equi-distant points
            List<Vector3> distancePoints = new List<Vector3>();
            
            float t = 0; 
            int count = 0;
            while(t <= 1.0f || count< 10)
            {
                count++;
                Vector3 point = spline.GetPointAtLinearDistance(t, brushSpacing, out t);

                distancePoints.Add(point);
            }

            // sort by height reverse
            // sequential height raising would just lead to irregularities, ie when a higher point follows a lower point
            // we need to proceed from top to bottom height
            distancePoints.Sort((a, b) => -a.y.CompareTo(b.y));

            Vector3[] points = distancePoints.ToArray();

            foreach (var point in points)
            {

                float targetHeight = (point.y - terrainPosition.y) / totalHeight;

                int centerX = (int)(splineContainer.transform.position.z + point.z);
                int centerY = (int)(splineContainer.transform.position.x + point.x);

                AdjustTerrain(allHeights, radius, centerX, centerY, targetHeight);

            }
        }

        terrain.terrainData.SetHeights(0, 0, allHeights);
    }

    private void AdjustTerrain(float[,] heightMap, int radius, int centerX, int centerY, float targetHeight)
    {
        float deltaHeight = targetHeight - heightMap[centerX, centerY];
        int sqrRadius = radius * radius;

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        for (int offsetY = -radius; offsetY <= radius; offsetY++)
        {
            for (int offsetX = -radius; offsetX <= radius; offsetX++)
            {
                int sqrDstFromCenter = offsetX * offsetX + offsetY * offsetY;

                // check if point is inside brush radius
                if (sqrDstFromCenter <= sqrRadius)
                {
                    // calculate brush weight with exponential falloff from center
                    float dstFromCenter = Mathf.Sqrt(sqrDstFromCenter);
                    float t = dstFromCenter / radius;
                    float brushWeight = Mathf.Exp(-t * t / brushFallOff);

                    // raise terrain
                    int brushX = centerX + offsetX;
                    int brushY = centerY + offsetY;

                    if (brushX >= 0 && brushY >= 0 && brushX < width && brushY < height)
                    {
                        heightMap[brushX, brushY] += deltaHeight * brushWeight;

                        // clamp the height
                        if (heightMap[brushX, brushY] > targetHeight)
                        {
                            heightMap[brushX, brushY] = targetHeight;
                        }
                    }
                }
            }
        }
    }

}