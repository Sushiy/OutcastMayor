using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Statemachine : MonoBehaviour
{

    public class State
    {
        protected UnityAction start;
        protected UnityAction update;
        protected UnityAction<State> exit;
        protected Statemachine statemachine;
        public string stateName;

        public State(UnityAction startRef, UnityAction updateRef, UnityAction<State> exitRef, Statemachine statemachine, string name)
        {
            start += startRef;
            update += updateRef;
            exit += exitRef;
            this.statemachine = statemachine;
            this.stateName = name;
        }

        public void Start()
        {
            start?.Invoke();
        }

        public void Update()
        {
            update?.Invoke();
        }

        public void Exit(State nextState)
        {
            exit?.Invoke(nextState);
            statemachine.CompleteChangeState();
        }

    }

    public State currentState;
    public State nextState;

    public string currentStateName;
    public void ChangeState(State newState)
    {
        if(currentState == null)
        {
            currentState = newState;
            currentStateName = currentState.stateName;
            return;
        }
        if (newState == currentState)
        {
            Debug.LogWarning("Changing to the same gamestate: " + newState);
            return;
        }

        nextState = newState;
        currentState.Exit(newState);
    }
    public void CompleteChangeState()
    {
        if (nextState == null)
        {
            Debug.LogError("There is no next state to change into");
        }
        currentState = nextState;
        currentStateName = currentState.stateName;
        nextState = null;
        currentState.Start();
    }
}
