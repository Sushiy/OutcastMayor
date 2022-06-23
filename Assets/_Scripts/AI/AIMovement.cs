using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour, IMovement
{
    float runningSpeed = 5.0f;
    float walkingSpeed = 3.0f;

    [SerializeField]
    private float minRunningDistance = 10.0f;

    NavMeshAgent navMeshAgent;
    public UnityEvent OnPathComplete;

    bool startedPath;

    CharacterAnimation characterAnimation;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterAnimation = GetComponent<CharacterAnimation>();
        OnPathComplete = new UnityEvent();
    }

    public void MoveTo(Vector3 position, bool running, UnityAction callback)
    {
        OnPathComplete.RemoveAllListeners();
        OnPathComplete.AddListener(callback);
        if(navMeshAgent.SetDestination(position))
        {
            running = Vector3.Distance(navMeshAgent.destination, navMeshAgent.transform.position) > minRunningDistance;
            navMeshAgent.speed = running ? runningSpeed : walkingSpeed;
            startedPath = true;
            print("Path started");
        }
        else
        {
            print("Destination could not be set.");
        }
    }

    private void Update()
    {
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
}
