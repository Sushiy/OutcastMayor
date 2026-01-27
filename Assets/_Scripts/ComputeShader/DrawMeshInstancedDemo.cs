using Unity.Mathematics;
using UnityEngine;

public class DrawMeshInstancedDemo : MonoBehaviour 
{
    [Header("ShingleSettings")]
    public Mesh mesh;
    // Material to use for drawing the meshes.
    public Material material;

    // How many meshes to draw.
    public Vector3 scale = new Vector3(.1f,.2f, 0.02f);
    public float shingleAngle = 5f; // rotation in deg
    // Range to draw meshes within.
    public Vector2 positionSpacing = new Vector2(.125f, .15f);

    [Header("RoofShape")]

    public Vector3 minPosition = new Vector3(-.5f,0,0);
    Vector3 minPosition1WS;
    Vector3 minPosition2WS;
    Vector3 minPositionCache;
    public Vector3 maxPosition = new Vector3(0.5f, 1f, -1f);
    Vector3 maxPosition1WS;
    Vector3 maxPosition2WS;
    Vector3 maxPositionCache;

    [Header("Instanced Rendering")]
    public bool RenderIndirect = false;
  
    private InstanceData[] instanceData;
    private MaterialPropertyBlock block;
    private ComputeBuffer positionBuffer;
    private GraphicsBuffer commandBuffer;
    GraphicsBuffer.IndirectDrawIndexedArgs[] commandData;

    int population;
    private void OnValidate()
    {
        if(minPositionCache != minPosition || maxPositionCache != maxPosition)
        {
            UpdatePoints();
            minPositionCache = minPosition;
            maxPositionCache = maxPosition;
        }
    }
    Vector3 tangentWS;
    public int heightCount, widthCount;
    public Vector2 realSpacing;

    RenderParams instancedRP;
    RenderParams indirectRP;

    public struct InstanceData
    {
        public Matrix4x4 objectToWorld;
    }

    void UpdatePoints()
    {
        //Calculate cornerPoints
        minPosition1WS = transform.TransformPoint(minPosition);
        maxPosition2WS = transform.TransformPoint(maxPosition);
        Vector3 direction = maxPosition2WS-minPosition1WS;
        Vector3 widthWS = Vector3.Project(direction, transform.right);
        minPosition2WS = minPosition1WS + widthWS;
        maxPosition1WS = maxPosition2WS - widthWS;
        Vector3 centerWS = minPosition1WS + direction/2.0f;

        Vector3 normalWS = Vector3.Cross(direction, transform.right);
        tangentWS = maxPosition1WS - minPosition1WS;
        float width = widthWS.magnitude;
        float height = tangentWS.magnitude;
        tangentWS.Normalize();

        widthCount = Mathf.CeilToInt(width/positionSpacing.x);
        heightCount = Mathf.CeilToInt(height/positionSpacing.y);
        realSpacing = new Vector2(width/widthCount, height/heightCount);
        population = widthCount * heightCount;

        Quaternion roofRotation = Quaternion.LookRotation(normalWS, tangentWS);
        Quaternion shingleRotation = roofRotation * Quaternion.AngleAxis(shingleAngle, transform.right);
        if(!RenderIndirect)
        {
            block = new MaterialPropertyBlock(); 
            //Setup arrays
            instanceData = new InstanceData[population];

            instancedRP = new RenderParams(material);
            instancedRP.worldBounds = new Bounds(centerWS, direction); // use tighter bounds for better FOV culling

            for(int x = 0; x < widthCount; x++)
            {
                for(int y = 0; y < heightCount; y++)
                {
                    int i = x + y*widthCount;
                    float offset = 0.5f * positionSpacing.x;
                    // Build matrix.
                    if(y % 2 != 0)
                        offset = 1f * positionSpacing.x;
                    Vector3 position = minPosition1WS + (x * realSpacing.x + offset) * transform.right + y * realSpacing.y * tangentWS;
                    Quaternion rotation = shingleRotation;
                    Vector3 scale = this.scale;

                    var mat = Matrix4x4.TRS(position, rotation, scale);

                    instanceData[i].objectToWorld = mat;
                }
            }            
        }
        else
        {
            //Create a buffer that will store the positions
            //positionBuffer = new ComputeBuffer(population, sizeof(float) * 3, ComputeBufferType.Default, ComputeBufferMode.Immutable); //These settings are just the default
            commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, GraphicsBuffer.IndirectDrawIndexedArgs.size);
            commandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

            indirectRP = new RenderParams(material);
            indirectRP.worldBounds = new Bounds(centerWS, direction); // use tighter bounds for better FOV culling
            indirectRP.matProps = new MaterialPropertyBlock();
            
            indirectRP.matProps.SetVector("_MinPositionWS", minPosition1WS);
            indirectRP.matProps.SetVector("_ShingleOffset", realSpacing);
            indirectRP.matProps.SetVector("_HeightVec", tangentWS);
            indirectRP.matProps.SetVector("_WidthVec", transform.right);
            indirectRP.matProps.SetInteger("_WidthCount", widthCount);
            indirectRP.matProps.SetInteger("_HeightCount", heightCount);
            indirectRP.matProps.SetFloat("_Offset", positionSpacing.x);    
            indirectRP.matProps.SetVector("_ShingleRotation", new Vector4(shingleRotation.x,shingleRotation.y, shingleRotation.z, shingleRotation.w));
            
            commandData[0].indexCountPerInstance = mesh.GetIndexCount(0);
            commandData[0].instanceCount = (uint)population;
            commandBuffer.SetData(commandData);        
        }
    }

    private void Start()
    {
        UpdatePoints();
    }

    private void Update()
    {
            // Draw a bunch of meshes each frame.
        if(!RenderIndirect)
            Graphics.RenderMeshInstanced(instancedRP,mesh, 0, instanceData);
        else
            Graphics.RenderMeshIndirect(indirectRP, mesh, commandBuffer, 1,0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(minPosition1WS, minPosition2WS);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(minPosition2WS, maxPosition2WS);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(maxPosition2WS, maxPosition1WS);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(maxPosition1WS, minPosition1WS);
    }

    void OnDestroy()
    {
        commandBuffer?.Release();
        commandBuffer = null;        
    }
}