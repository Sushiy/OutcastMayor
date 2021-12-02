using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovement : MonoBehaviour
{
    float runningSpeed = 5.0f;
    float walkingSpeed = 3.0f;


    NavMeshAgent navMeshAgent;
    UnityEvent OnPathComplete;

    bool startedPath;

    Animator animator;
    //Hash values for animator control
    private int hashRunning = Animator.StringToHash("bIsRunning");
    private int hashSpeedForward = Animator.StringToHash("fSpeedForward");
    private int hashSpeedSide = Animator.StringToHash("fSpeedSide");

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        OnPathComplete = new UnityEvent();
    }

    public void MoveTo(Vector3 position, bool running, UnityAction callback)
    {
        OnPathComplete.RemoveAllListeners();
        if(navMeshAgent.SetDestination(position))
        {
            navMeshAgent.speed = running ? runningSpeed : walkingSpeed;
            startedPath = true;
            OnPathComplete.AddListener(callback);
            OnPathComplete.AddListener(() => OnPathComplete.RemoveListener(callback));
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
            animator.SetBool(hashRunning, true);
            animator.SetFloat(hashSpeedForward, velocity/navMeshAgent.speed);
            //animator.SetFloat(hashSpeedSide, moveInput.y);
        }
        else
        {
            animator.SetBool(hashRunning, false);
        }

        if (startedPath && IsPathComplete())
        {
            startedPath = false;
            print("Path Completed!");
            OnPathComplete?.Invoke();
        }
    }

    protected bool IsPathComplete()
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
}
