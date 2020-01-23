using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFSM : FiniteStateMachine<AIStates> { };

public class NewBehaviourScript : MonoBehaviour
{
    public TestFSM m_testfsm;

    // Start is called before the first frame update
    void Start()
    {
        m_testfsm = new TestFSM();

        // Move from IDLE to TARGETING
        m_testfsm.AddTransition(AIStates.IDLE, AIStates.TARGETING, Target);
        // Move from TARGETING to IDLE
        m_testfsm.AddTransition(AIStates.TARGETING, AIStates.IDLE, Idle);
        // Move from TARGETING to ATTACK
        m_testfsm.AddTransition(AIStates.TARGETING, AIStates.ATTACK, Attack);
        // Move from ATTACK to DONE
        m_testfsm.AddTransition(AIStates.ATTACK, AIStates.DONE, Done);
    }

    void Idle()
    {
        Debug.Log("Idling");
    }

    void Target()
    {
        Debug.Log("Targeting");
    }

    void Attack()
    {
        Debug.Log("Attacking");
    }

    void Done()
    {
        Debug.Log("Done");
    }

    void updateStates()
    {
        if (Input.GetKeyDown("up"))
        {
            m_testfsm.Advance(AIStates.TARGETING);
        }
        if (Input.GetKeyDown("down"))
        {
            m_testfsm.Advance(AIStates.IDLE);
        }
        if (Input.GetKeyDown("left"))
        {
            m_testfsm.Advance(AIStates.ATTACK);
        }
        if (Input.GetKeyDown("right"))
        {
            m_testfsm.Advance(AIStates.DONE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateStates();
    }
}
