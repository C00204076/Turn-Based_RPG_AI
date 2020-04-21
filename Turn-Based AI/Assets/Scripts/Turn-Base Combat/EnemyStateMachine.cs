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
    private GameObject m_enemyObject;
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
    public GameObject m_heroTarget;
    private float m_animationSpeed;

    private bool m_alive = true;

    private bool m_firstAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        //
        m_attackTime = 0.0f;
        //
        m_currentState = TurnState.PROCESSING;
        //
        m_bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>();
        m_enemyObject = GameObject.Find("Enemy");
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
                if(m_bsm.m_heroes.Count > 0)
                {
                    chooseAction();
                }
                m_currentState = TurnState.ACTION;
                break;
            case TurnState.WAITING:

                break;
            case TurnState.ACTION:
                //StartCoroutine(actionTime());
                actionTime();
                //m_currentState = TurnState.PROCESSING;
                break;
            case TurnState.DEAD:
                if(!m_alive)
                {
                    return;
                }

                else
                {
                    // Change Enemy tag
                    this.gameObject.tag = "DeadEnemy";
                    // Remove from State Machine
                    m_bsm.m_enemies.Remove(this.gameObject);
                    // Remove all inputs of Hero Atacks

                    if (m_bsm.m_enemies.Count > 0)
                    {
                        for (int i = 0; i < m_bsm.m_performList.Count; i++)
                        {
                            if (m_bsm.m_performList[i].AttackingObject == this.gameObject)
                            {
                                m_bsm.m_performList.Remove(m_bsm.m_performList[i]);
                                m_bsm.m_performEnemy.Remove(m_bsm.m_performEnemy[i]);
                            }
                        }
                    }
                    //
                    //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);
                    //
                    m_alive = false;
                    m_bsm.m_battleStates = BattleStateMachine1.PerformAction.CHECKALIVE;
                }
                break;
        }
    }

    //
    void updateTurnBarProgress()
    {

        if (m_attackTime < 1.0f)
        {
            if(m_firstAttack == false)
            {
                //
                m_attackTime += 0.1f;
            }
            else
            {
                //
                m_attackTime += 0.001f;
            }
            
        }


        //
        else if (m_attackTime >= 1.0f)
        {
            m_currentState = TurnState.CHOOSEACTION;
            m_attackTime = 0.0f;
            m_firstAttack = true;
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
        setHeroTarget(attack.Target);

        //
        int num = Random.Range(0, m_enemy.m_attacks.Count);
        attack.m_choosenAttack = m_enemy.m_attacks[num];
        Debug.Log(this.gameObject.name + " has choosen " +
                  attack.m_choosenAttack.m_attackName + " and does " +
                  attack.m_choosenAttack.m_attackDamage + " damage!");

        //
        if(m_bsm.m_aiState == BattleStateMachine1.AIState.RANDOM)
        {

        }
        //
        else if (m_bsm.m_aiState == BattleStateMachine1.AIState.DT)
        {

        }
        //
        else if (m_bsm.m_aiState == BattleStateMachine1.AIState.BT)
        {

        }

        //
        m_bsm.collectActions(attack);
    }

    //IEnumerator
    void actionTime()
    {
        // Break IEnumerator if action has already started
        /*if (m_startedAct)
        {
            yield break;
        }*/

        m_startedAct = true;

        // Animate enemy attacking hero, when near
        Vector2 heroPos = new Vector2(m_heroTarget.transform.position.x,
                                      m_heroTarget.transform.position.y);
        // Return null if true
        /*while (moveTowardsHero(heroPos))
        {
            yield return null;
        }//*/

        // Wait
        //yield return new WaitForSeconds(0.5f);
        // Do damage
        doDamage();
        // Animate back to sart position
        Vector3 firstPos = m_startPos;
        /*while (moveTowardsStart(firstPos))
        {
            yield return null;
        }//*/

        // Wait
        //yield return new WaitForSeconds(4.5f);
        // Remove performer from BSM list
        m_bsm.m_performList.RemoveAt(0);
        // Reset BSM -> Wait
        m_bsm.m_battleStates = BattleStateMachine1.PerformAction.WAIT;
        // End of Coroutine
        m_startedAct = false;
        // Reset this enemy state
        m_attackTime = 0.0f;
        m_currentState = TurnState.PROCESSING;
    }

    private bool moveTowardsHero(Vector3 target)
    {
        return target != (m_enemy.transform.position = Vector2.MoveTowards(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(target.x, target.y, target.z),
            m_animationSpeed * Time.deltaTime));
    }

    private bool moveTowardsStart(Vector3 target)
    {
        return target != (m_enemy.transform.position = Vector2.MoveTowards(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(target.x, target.y, target.z),
            m_animationSpeed * Time.deltaTime));
    }

    public void setHeroTarget(GameObject target)
    {
        m_heroTarget = target;
    }

    //
    void doDamage()
    {
        float calDamage = m_enemy.m_currentATK + m_bsm.m_performList[0].m_choosenAttack.m_attackDamage;

        m_heroTarget.GetComponent<HeroStateMachine>().takeDamage(calDamage);
    }

    // 
    public void takeDamage(float damage)
    {
        m_enemy.m_currentHP -= damage;

        if (m_enemy.m_currentHP <= 0)
        {
            m_enemy.m_currentHP = 0;
            m_currentState = TurnState.DEAD;
        }
    }

}
