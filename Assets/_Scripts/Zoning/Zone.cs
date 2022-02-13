using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Zone : MonoBehaviour
{
    [SerializeField]
    bool xInverted = false;
    [SerializeField]
    int sizeX = 1;
    [SerializeField]
    bool zInverted = false;
    [SerializeField]
    int sizeZ = 1;

    [SerializeField]
    TMP_Text sizeXText;

    [SerializeField]
    TMP_Text sizeZText;

    [SerializeField]
    Shapes.Line outsideLineX1;
    [SerializeField]
    Shapes.Line outsideLineX2;
    [SerializeField]
    Shapes.Line outsideLineZ1;
    [SerializeField]
    Shapes.Line outsideLineZ2;

    [SerializeField]
    Transform dragHandle;
    [SerializeField]
    Transform snappedHandle;

    [SerializeField]
    Shapes.Line gridLinePrefab;
    [SerializeField]
    List<Shapes.Line> gridLines;
    [SerializeField]
    Transform gridLineParent;

    [SerializeField]
    Shapes.Polyline advancedGridLinePrefab;
    [SerializeField]
    List<Shapes.Polyline> advancedGridLines;
    [SerializeField]
    Transform advancedGridLineParent;

    /// <summary>
    /// List of grid points, mapped onto the terrain
    /// </summary>
    [Header("Terrain Mapping")]
    [SerializeField]
    List<List<Vector3>> gridPoints;

    List<List<Vector3>> originalGridPoints;
    [SerializeField]
    Shapes.Polyline mappedOutsideLine;

    [SerializeField]
    float verticalGridOffset = .5f;

    void Update()
    {
        UpdateGrid();
    }

    void UpdateGrid()
    {
        int newX = Mathf.RoundToInt(dragHandle.localPosition.x);
        if(newX < 0)
        {
            xInverted = true;
        }
        else
        {
            xInverted = false;
        }
        int newZ = Mathf.RoundToInt(dragHandle.localPosition.z);
        if (newZ < 0)
        {
            zInverted = true;
        }
        else
        {
            zInverted = false;
        }
        if (newX != sizeX || newZ != sizeZ)
        {
            int oldSizeX = sizeX;
            int oldSizeZ = sizeZ;
            sizeX = Mathf.Abs(newX);
            sizeZ = Mathf.Abs(newZ);

            Vector3 xDir = xInverted ? -transform.right : transform.right;
            Vector3 zDir = zInverted ? -transform.forward : transform.forward;


            outsideLineX1.End = xDir * sizeX;
            
            outsideLineZ1.End = zDir * sizeZ;

            snappedHandle.position = xDir * sizeX + zDir * sizeZ;

            outsideLineX2.Start = outsideLineZ1.End;
            outsideLineX2.End = snappedHandle.position;

            outsideLineZ2.Start = outsideLineX1.End;
            outsideLineZ2.End = snappedHandle.position;

            Vector3 pos = transform.position + outsideLineX1.End / 2.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos) - verticalGridOffset;
            sizeXText.transform.position = pos;
            sizeXText.text = sizeX.ToString();
            pos = transform.position + outsideLineX1.End + outsideLineZ1.End / 2.0f;
            pos.y = Terrain.activeTerrain.SampleHeight(pos) - verticalGridOffset;
            sizeZText.transform.position = pos;
            sizeZText.text = sizeZ.ToString();

            //********GRID********

            if(gridLines == null)
            {
                gridLines = new List<Shapes.Line>();
            }

            int gridLineCount = (Mathf.Max(0,(sizeX - 1)) + Mathf.Max(0, (sizeZ - 1)));
            int gridLineDiff = gridLineCount - gridLines.Count;
            if (gridLineDiff > 0)
            {
                //There are not enough gridlines, so spawn more!
                for (int i = 0; i < gridLineCount; i++)
                {
                    gridLines.Add(Instantiate<Shapes.Line>(gridLinePrefab, gridLineParent));
                }
            }
            else if (gridLineDiff < 0)
            {
                //There are too many gridlines, so let's deactivate those
                for (int i = gridLineCount; i < gridLines.Count; i++)
                {
                    gridLines[i].gameObject.SetActive(false);
                }
            }

            if(sizeX > 0)
            {
                //Create the gridlines along the xBorder
                for (int i = 0; i < sizeX; i++)
                {
                    gridLines[i].gameObject.SetActive(true);
                    gridLines[i].gameObject.name = "GridLineX" + i;
                    gridLines[i].Start = xDir * (i+1);
                    gridLines[i].End = gridLines[i].Start + outsideLineZ1.End;
                }

            }
            if (sizeZ > 0)
            {
                //Create the gridlines along the zBorder
                for (int i = 0; i < sizeZ-1; i++)
                {
                    gridLines[sizeX + i].gameObject.SetActive(true);
                    gridLines[sizeX + i].gameObject.name = "GridLineZ" + i;
                    gridLines[sizeX + i].Start = zDir * (i+1);
                    gridLines[sizeX + i].End = gridLines[sizeX + i].Start + outsideLineX1.End;
                }
            }

            //*****TERRAIN MAPPING*****

            //1. DONE Build the required number of gridpoints
            //2. DONE Raycast up and down from every gridpoint to find the hegiht of the terrain at that point
            //3. DONE Adjust the gridpoint
            //4. DOING Adjust the gridDrawing
            //5. Mark Cells that are "too steep" or "blocked"

            int gridX = sizeX + 1;
            int gridZ = sizeZ + 1;

            if (originalGridPoints == null)
            {
                originalGridPoints = new List<List<Vector3>>(gridX);
            }

            if(gridPoints == null)
            {
                gridPoints = new List<List<Vector3>>(gridX);
            }

            //Clear old Gridpoints TODO: Optimize this!
            mappedOutsideLine.points.Clear();

            for (int x = 0; x < gridX; x++)
            {
                if(originalGridPoints.Count <= x)
                {
                    originalGridPoints.Add(new List<Vector3>(gridZ));
                }
                if (gridPoints.Count <= x)
                {
                    gridPoints.Add(new List<Vector3>(gridZ));
                }

                for (int z = 0; z < gridZ; z++)
                {
                    if(originalGridPoints[x].Count <= z)
                    {
                        originalGridPoints[x].Add(Vector3.zero);
                    }
                    if (gridPoints[x].Count <= z)
                    {
                        gridPoints[x].Add(Vector3.zero);
                    }
                    originalGridPoints[x][z] = transform.position + zDir * z + xDir * x;

                    Vector3 point = originalGridPoints[x][z];
                    point.y = Terrain.activeTerrain.SampleHeight(originalGridPoints[x][z]) - verticalGridOffset;
                    gridPoints[x][z] = point;
                }
            }

            //Draw outline
            for (int i = 0; i < gridX; i++)
            {
                mappedOutsideLine.AddPoint(new Shapes.PolylinePoint(transform.InverseTransformPoint(gridPoints[i][0])));
            }
            for (int i = 1; i < gridZ; i++)
            {
                mappedOutsideLine.AddPoint(new Shapes.PolylinePoint(transform.InverseTransformPoint(gridPoints[gridX - 1][i])));
            }
            for (int i = gridX-2; i >= 0; i--)
            {
                mappedOutsideLine.AddPoint(new Shapes.PolylinePoint(transform.InverseTransformPoint(gridPoints[i][gridZ-1])));
            }
            for (int i = gridZ-2; i > 0; i--)
            {
                mappedOutsideLine.AddPoint(new Shapes.PolylinePoint(transform.InverseTransformPoint(gridPoints[0][i])));
            }

            //Advanced Grid:
            int advancedGridLineCount = (Mathf.Max(0, (sizeX - 1)) + Mathf.Max(0, (sizeZ - 1)));
            int advancedGridLineDiff = advancedGridLineCount - advancedGridLines.Count;

            if (advancedGridLines == null)
            {
                advancedGridLines = new List<Shapes.Polyline>();
            }

            if (advancedGridLineDiff > 0)
            {
                //There are not enough gridlines, so spawn more!
                for (int i = 0; i < advancedGridLineCount; i++)
                {
                    advancedGridLines.Add(Instantiate<Shapes.Polyline>(advancedGridLinePrefab, advancedGridLineParent));
                }
            }
            else if (advancedGridLineDiff < 0)
            {
                //There are too many gridlines, so let's deactivate those
                for (int i = advancedGridLineCount; i < advancedGridLines.Count; i++)
                {
                    advancedGridLines[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < advancedGridLineCount; i++)
            {
                advancedGridLines[i].gameObject.SetActive(false);
            }

            if (sizeX > 0)
            {
                //Create the gridlines along the xBorder
                for (int i = 0; i <= sizeX; i++)
                {
                    advancedGridLines[i].gameObject.SetActive(true);
                    advancedGridLines[i].gameObject.name = "GridLineX" + i;
                    advancedGridLines[i].points.Clear();
                    for (int j = 0; j <= sizeZ; j++)
                    {
                        advancedGridLines[i].AddPoint(transform.InverseTransformPoint(gridPoints[i][j]));
                    }
                }

            }
            if (sizeZ > 0)
            {
                
                //Create the gridlines along the zBorder
                for (int i = 0; i <= sizeZ; i++)
                {
                    advancedGridLines[sizeX + i].gameObject.SetActive(true);
                    advancedGridLines[sizeX + i].gameObject.name = "GridLineZ" + i;
                    advancedGridLines[sizeX + i].points.Clear();
                    for (int j = 0; j <= sizeX; j++)
                    {
                        advancedGridLines[sizeX + i].AddPoint(transform.InverseTransformPoint(gridPoints[j][i]));
                    }
                }
            }

        }
    }

    public void UpdateDragHandlePosition(Vector3 position)
    {
        dragHandle.position = position;
    }

    private void OnDrawGizmos()
    {
        if(originalGridPoints != null)
        {
            for (int x = 0; x < sizeX+1; x++)
            {
                for (int z = 0; z < sizeZ+1; z++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(originalGridPoints[x][z], gridPoints[x][z]);
                }
            }

        }
    }
}
