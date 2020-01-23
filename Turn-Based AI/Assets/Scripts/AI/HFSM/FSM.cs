using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum AIStates
{
    IDLE,
    TARGETING,
    ATTACK,
    DONE
}

public delegate void Callback();


/// <summary>
/// StateTransition class
/// </summary>
/// <typeparam name="S">State enum</typeparam>
public class StateTransition<S> : System.IEquatable<StateTransition<S>>
{
    // Public variables
    // ----------------------------------------

    // Protected variables
    // ----------------------------------------
    protected S mInitState;
    protected S mEndState;

    // Public functions
    // ----------------------------------------
    public StateTransition() 
    { 

    }
    public StateTransition(S init, S end) 
    { 
        mInitState = init; mEndState = end; 
    }

    public bool Equals(StateTransition<S> other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return mInitState.Equals(other.GetInitState()) && mEndState.Equals(other.GetEndState());
    }

    public override int GetHashCode()
    {
        if ((mInitState == null || mEndState == null))
            return 0;

        unchecked
        {
            int hash = 17;
            hash = hash * 23 + mInitState.GetHashCode();
            hash = hash * 23 + mEndState.GetHashCode();
            return hash;
        }
    }

    public S GetInitState() 
    { 
        return mInitState; 
    }
    public S GetEndState() 
    { 
        return mEndState; 
    }
}


/// <summary>
/// A generic Finite state machine
/// </summary>
/// <typeparam name="S"></typeparam>
public class FiniteStateMachine<S> 
{
    // Public variables
    // ----------------------------------------

    // Protected variables
    // ----------------------------------------
    protected S mState;
    protected S mPrevState;
    protected bool mbLocked = false;

    protected Dictionary<StateTransition<S>, System.Delegate>
      mTransitions;


    // Public functions
    // ----------------------------------------
    public FiniteStateMachine()
    {
        mTransitions = new Dictionary<StateTransition<S>, System.Delegate>();
    }

    public void Initialise(S state) { mState = state; }

    public void Advance(S nextState)
    {
        if (mbLocked)
            return;

        StateTransition<S> transition = new StateTransition<S>(mState, nextState);

        // Check if the transition is valid
        System.Delegate d;
        if (mTransitions.TryGetValue(transition, out d)) // new StateTransition(mState, nextState)
        {
            //Debug.Log("[FMS] Advancing to " + nextState + " state...");

            if (d != null)
            {
                Callback c = d as Callback;
                c();
            }

            mPrevState = mState;
            mState = nextState;
        }
        else
        {
            //Debug.Log("[FMS] Cannot advance to " + nextState + " state");
        }
    }

    public void AddTransition(S init, S end, Callback c)
    {
        StateTransition<S> tr = new StateTransition<S>(init, end);

        if (mTransitions.ContainsKey(tr))
        {
            Debug.Log("[FSM] Transition: " + tr.GetInitState() + " - " + tr.GetEndState() + " exists already.");
            return;
        }

        mTransitions.Add(tr, c);

        Debug.Log("[FSM] Added transition " + mTransitions.Count + ": " + tr.GetInitState() + " - " + tr.GetEndState()
          + ", Callback: " + c);
    }

    // Call this to prevent the state machine from leaving this state
    public void Lock() { mbLocked = true; }

    public void Unlock()
    {
        mbLocked = false;
        Advance(mPrevState);
    }

    public S GetState() 
    { 
        return mState; 
    }
    public S GetPrevState() 
    { 
        return mPrevState; 
    }
}
