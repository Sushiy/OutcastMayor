using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OutcastMayor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ControlElementManager : MonoBehaviour
{
    List<ControlPoint> controlPoints;
    [SerializeField]
    protected ControlPoint controlPointPrefab; 
    List<ControlEdge> controlEdges;
    [SerializeField]
    protected ControlEdge controlEdgePrefab; 

    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 50;

    public VectorBuilding vectorBuilding;

    void Awake()
    {
        controlPoints = new List<ControlPoint>();
        controlEdges = new List<ControlEdge>();
        vectorBuilding.onUpdateGraph += GetCheckPointsForGraph;
    }

    void GetCheckPointsForGraph(VectorPointGraph _graph)
    {
        //Check points
        List<VectorPoint> _graphPoints = _graph.points;
        //Adjust the size of the controlpointlist
        if(controlPoints.Count > _graphPoints.Count)
        {
            for(int i = controlPoints.Count-1; i >= _graphPoints.Count; i--)
            {
                ControlPoint c = controlPoints[i];
                PointPool.Release(c);
                controlPoints.Remove(c);
            }
        }

        //Get all the points that are already accounted for
        for(int i = 0; i < _graphPoints.Count; i++)
        {
            if(controlPoints.Count > i)
            {
                if(controlPoints[i].vectorPoint == _graphPoints[i])
                {
                    continue;
                }
                else
                {
                    controlPoints[i].SetData(_graphPoints[i], vectorBuilding);
                }
            }
            else
            {
                ControlPoint c = PointPool.Get();
                c.SetData(_graphPoints[i], vectorBuilding);
                controlPoints.Add(c);
            }
        }

        //Check edges
        List<VectorEdge> _graphEdges = _graph.edges;
        //Adjust the size of the controlpointlist
        if(controlEdges.Count > _graphEdges.Count)
        {
            for(int i = controlEdges.Count-1; i >= _graphEdges.Count; i--)
            {
                ControlEdge c = controlEdges[i];
                EdgePool.Release(c);
                controlEdges.Remove(c);
            }
        }

        //Get all the points that are already accounted for
        for(int i = 0; i < _graphEdges.Count; i++)
        {
            if(controlEdges.Count > i)
            {
                if(controlEdges[i].vectorEdge == _graphEdges[i])
                {
                    continue;
                }
                else
                {
                    controlEdges[i].SetData(_graphEdges[i], vectorBuilding);
                }
            }
            else
            {
                ControlEdge c = EdgePool.Get();
                c.SetData(_graphEdges[i], vectorBuilding);
                controlEdges.Add(c);
            }
        }
        
    }

#region PointPool
    IObjectPool<ControlPoint> pointPool;
    public IObjectPool<ControlPoint> PointPool
    {
        get
        {
            if (pointPool == null)
            {
                pointPool = new ObjectPool<ControlPoint>(CreatePooledPointItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 20, maxPoolSize);
            }
            return pointPool;
        }
    }

    ControlPoint CreatePooledPointItem()
    {
        ControlPoint point = Instantiate<ControlPoint>(controlPointPrefab, transform);
       return point;
    }

    void OnReturnedToPool(ControlPoint _point)
    {
        _point.gameObject.SetActive(false);
    } 

    void OnTakeFromPool(ControlPoint _point)
    {
        _point.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(ControlPoint _point)
    {
        Destroy(_point.gameObject);
    }
#endregion
#region EdgePool
    IObjectPool<ControlEdge> edgePool;
    public IObjectPool<ControlEdge> EdgePool
    {
        get
        {
            if (edgePool == null)
            {
                edgePool = new ObjectPool<ControlEdge>(CreatePooledEdgeItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 20, maxPoolSize);
            }
            return edgePool;
        }
    }

    ControlEdge CreatePooledEdgeItem()
    {
        ControlEdge edge = Instantiate<ControlEdge>(controlEdgePrefab, transform);
        return edge;
    }

    void OnReturnedToPool(ControlEdge _edge)
    {
        _edge.gameObject.SetActive(false);
    } 

    void OnTakeFromPool(ControlEdge _edge)
    {
        _edge.gameObject.SetActive(true);
    }

    void OnDestroyPoolObject(ControlEdge _edge)
    {
        Destroy(_edge.gameObject);
    }
#endregion
}
