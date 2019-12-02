using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 16:26 14 November 2019
// Finished at
// Time taken:
// Known bugs:

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine1 m_bsm;
    public BaseEnemy m_enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    //
    public TurnState m_currentState;
    //
    private float m_attackTime;
    //
    private Vector3 m_startPos;
    //
    private HandleTurn attack;
    //actionTime
    private bool m_startedAct = false;
    private GameObject m_heroTarget;
    private float m_animationSpeed;



    // Start is called before the first frame update
    void Start()
    {
        //
        m_attackTime = 0.0f;
        //
        m_currentState = TurnState.PROCESSING;
        //
        m_bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>();
        m_startPos = transform.position;
        //
        attack = new HandleTurn();
        //
        m_animationSpeed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_currentState)
        {
            case TurnState.PROCESSING:
                updateTurnBarProgress();
                break;
            case TurnState.CHOOSEACTION:
                chooseAction();
                m_currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:

                break;
            case TurnState.ACTION:
                StartCoroutine(actionTime());
                break;
            case TurnState.DEAD:

                break;
        }
    }

    //
    void updateTurnBarProgress()
    {

        if (m_attackTime < 1.0f)
        {
            //
            m_attackTime++;
        }


        //
        else
        {
            m_currentState = TurnState.CHOOSEACTION;
        }
    }

    //
    void chooseAction()
    {
        //
        attack.Attacker = m_enemy.name;
        attack.Type = "Enemy";
        attack.AttackingObject = GameObject.Find("Enemy");
        attack.Target = m_bsm.m_heroes[Random.Range(0, m_bsm.m_heroes.Count)];
        //
        m_bsm.collectActions(attack);
    }

    //
    private IEnumerator actionTime()
    {
        // Break IEnumerator if action has already started
        if(m_startedAct)
        {
            yield break;
        }

        m_startedAct = true;

        // Animate enemy attacking hero, when near
        Vector3 heroPos = new Vector3(m_heroTarget.transform.position.x - 1.5f, 
                                      m_heroTarget.transform.position.y, 
                                      m_heroTarget.transform.position.z);
        // Return null if true
        while(moveTowardsHero(heroPos))
        {
            yield return null;
        }

        // Wait
        // Do damage

        // Remove performer from BSM list

        // Reset BSM -> Wait

        m_startedAct = false;
        // Reset this enemy state
        m_attackTime = 0.0f;
        m_currentState = TurnState.PROCESSING;
    }

    private bool moveTowardsHero(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, m_animationSpeed * Time.deltaTime));
    }
}
