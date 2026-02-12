using System.Collections;
using System.Collections.Generic;
using OutcastMayor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : MonoBehaviour, IMovement
{
    float runningSpeed = 5.0f;
    float walkingSpeed = 3.0f;

    [SerializeField]
    private float minRunningDistance = 10.0f;

    NavMeshAgent navMeshAgent;
    public UnityEvent OnPathComplete;

    bool startedPath;

    CharacterAnimation characterAnimation;
    Character character;

    bool movementLocked = false;

    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    bool isGrounded;
    [SerializeField]
    LayerMask groundLayerMask;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterAnimation = GetComponent<CharacterAnimation>();
        character = GetComponent<Character>();
        OnPathComplete = new UnityEvent();
    }

    void Start()
    {
        NavMeshBaker.SetNavMeshUpdate(transform.position);
    }

    public bool CheckPositionReachable(Vector3 _targetPosition, out NavMeshPath _navMeshPath)
    {
        _navMeshPath = new NavMeshPath();
        NavMeshHit sampledPosition;
        if(NavMesh.SamplePosition(_targetPosition, out sampledPosition, 4.0f, NavMesh.AllAreas))
        {
            if(navMeshAgent.CalculatePath(sampledPosition.position, _navMeshPath) && _navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }
        return false;
    }

    public void TryMoveTo(Vector3 position, bool running, UnityAction callback)
    {
        OnPathComplete.RemoveAllListeners();
        
        navMeshAgent.agentTypeID = character.WeightedDown? AgentTypeID.GetAgenTypeIDByName("Large") : AgentTypeID.GetAgenTypeIDByName("Medium");

        if(CheckPositionReachable(position, out NavMeshPath navMeshPath))
        {
            OnPathComplete.AddListener(callback);
            if(navMeshAgent.SetPath(navMeshPath))
            {
                running = !character.WeightedDown && Vector3.Distance(navMeshAgent.destination, navMeshAgent.transform.position) > minRunningDistance;
                navMeshAgent.speed = running ? runningSpeed : walkingSpeed;
                startedPath = true;
                print($"[{name}->Movement]: Path started");
            }
            else
            {
                print($"[{name}->Movement]: Path not found");
            }
            
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position +Vector3.down *.2f, .2f, groundLayerMask);
        characterAnimation.SetGrounded(isGrounded);
        float velocity = navMeshAgent.velocity.magnitude;
        if (startedPath && velocity > 0)
        {
            characterAnimation.SetRunning(true);
            characterAnimation.SetSpeedForward(velocity/runningSpeed);
        }
        else
        {
            characterAnimation.SetRunning(false);
            characterAnimation.SetSpeedForward(0);
        }

        if (startedPath)
        {
            Debug.DrawLine(navMeshAgent.transform.position, navMeshAgent.destination);
            if (IsPathComplete())
            {
                startedPath = false;
                print("Path Completed!");
                OnPathComplete?.Invoke();
            }
        }
    }

    public bool IsAlreadyArrived(Vector3 worldposition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(worldposition, out hit, 3.0f, navMeshAgent.areaMask))
        {
            if (Vector3.Distance(hit.position, navMeshAgent.transform.position) <= navMeshAgent.stoppingDistance)
            {
                return true;
            }

            return false;
        }
        return false;
    }

    public bool IsPathComplete()
    {
        if (Vector3.Distance(navMeshAgent.destination, navMeshAgent.transform.position) <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsMoving()
    {
        return startedPath && !IsPathComplete();
    }

    public void TeleportTo(Vector3 position)
    {
        navMeshAgent.Warp(position);
    }

    public void SnapYRotation(Quaternion rotation)
    {
       navMeshAgent.transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
    public void LockMovement(bool locked)
    {
        movementLocked = locked;
    }

     public static class AgentTypeID
 {
     public static int GetAgenTypeIDByName(string agentTypeName)
     {
         int count = NavMesh.GetSettingsCount();
         string[] agentTypeNames = new string[count + 2];
         for (var i = 0; i < count; i++)
         {
             int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
             string name = NavMesh.GetSettingsNameFromID(id);
             if(name == agentTypeName)
             {
                 return id;
             }
         }
         return -1;
     }
 }
}
