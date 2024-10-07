using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OutcastMayor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ControlElementManager : MonoBehaviour
{
    List<ControlPoint> controlPoints;
    ControlPoint selectedControlPoint;
    [SerializeField]
    protected ControlPoint controlPointPrefab; 

    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 50;

    VectorBuilding vectorBuilding;

    void Awake()
    {
        vectorBuilding.onUpdatePoints += CheckPoints;
    }

    void CheckPoints(List<VectorPoint> _graphPoints)
    {
        foreach(VectorPoint p in _graphPoints)
        {
            if(controlPoints.Any(x => x.vectorPoint == p))
            {
                continue;
            }
            else
            {
                ControlPoint newPoint = Pool.Get();
                newPoint.vectorPoint = p;
                controlPoints.Add(newPoint);
            }
        }
    }

    IObjectPool<ControlPoint> pool;
    public IObjectPool<ControlPoint> Pool
    {
        get
        {
            if (pool == null)
            {
                pool = new ObjectPool<ControlPoint>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 20, maxPoolSize);
            }
            return pool;
        }
    }

    ControlPoint CreatePooledItem()
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
}
