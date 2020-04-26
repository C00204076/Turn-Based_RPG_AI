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
    public GameObject m_btAttack;
    public GameObject m_btTarget;
    public GameObject m_dtAttack;
    public GameObject m_dtTarget;

    //
    public List<BTAttack> m_attackBTData = new List<BTAttack>();
    //
    public List<BTTarget> m_targetBTData = new List<BTTarget>();
    //
    public List<DTAttack> m_attackDTData = new List<DTAttack>();
    //
    public List<DTTarget> m_targetDTData = new List<DTTarget>();

    //
    public TurnState m_currentState;
    //
    private float m_attackTime;
    //
    private Vector3 m_startPos;
    //
    public HandleTurn attack;
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

        //
        setDTAttackData();
        //Debug.Log(m_attackDTData[0].GetComponent<DTAttack>().m_dtDamage);

        switch (m_currentState)
        {
            case TurnState.PROCESSING:
                updateTurnBarProgress();

                if (m_enemy.m_currentMP <= 0)
                {
                    m_enemy.m_currentMP = 0;
                }
                else if (m_enemy.m_currentMP >= m_enemy.m_baseMP)
                {
                    m_enemy.m_currentMP = m_enemy.m_baseMP;
                }

                break;
            case TurnState.CHOOSEACTION:
                if(m_bsm.m_heroes.Count > 0)
                {
                    chooseAction();
                }
                m_enemy.m_currentMP += 2;
                m_currentState = TurnState.ACTION;
                break;
            case TurnState.WAITING:
                // Idling
                break;
            case TurnState.ACTION:
                actionTime();
                // Reset attack data
                attack = null;
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
        if(m_bsm.m_aiState == BattleStateMachine1.AIState.RANDOM)
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
        m_startedAct = true;

        // Animate enemy attacking hero, when near
        Vector2 heroPos = new Vector2(m_heroTarget.transform.position.x,
                                      m_heroTarget.transform.position.y);

        // Do damage
        doDamage();
        // Animate back to sart position
        Vector3 firstPos = m_startPos;

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

        manaCost(m_bsm.m_performList[0].m_choosenAttack.m_attackCost);

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

    //
    public void manaCost(float cost)
    {
        m_enemy.m_currentMP -= cost;

        if (m_enemy.m_currentMP <= 0)
        {
            m_enemy.m_currentMP = 0;
        }
    }



    // Decision Tree
    //

    //




    // Behaviour Tree
    //
    // Hero targeting
    private NodeStates isAlive()
    {
        for (int i = 0; i < m_bsm.m_heroes.Count; i++)
        {

        }

        //
        if (m_heroTarget == null)
        {

        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates lowestHP()
    {
        float lowHP = m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_hero.m_currentHP;

        if (m_heroTarget == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                foreach (GameObject a in m_bsm.m_heroes)
                {
                    if (a.GetComponent<HeroStateMachine>().m_hero.m_currentHP <= lowHP)
                    {
                        lowHP = a.GetComponent<HeroStateMachine>().m_hero.m_currentHP;
                    }
                }

                // If lowest equal to base HP(maximum) of any of the heroes
                if (lowHP == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseHP)
                {
                    return NodeStates.FAILURE;
                }
            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates lowestMP()
    {
        float lowMP = m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_hero.m_currentMP;


        if (m_heroTarget == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                foreach (GameObject a in m_bsm.m_heroes)
                {
                    if (a.GetComponent<HeroStateMachine>().m_hero.m_currentHP <= lowMP)
                    {
                        lowMP = a.GetComponent<HeroStateMachine>().m_hero.m_currentMP;
                    }
                }

                // If lowest equal to base HP(maximum) of any of the heroes
                if (lowMP == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseMP)
                {
                    return NodeStates.FAILURE;
                }
            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates canHeal()
    {
        //
        if (m_heroTarget == null)
        {
            
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates selectTarget()
    {
        //
        if (m_heroTarget == null)
        {

        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }


    // Attack decision
    private NodeStates currentMP()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_enemy.m_attacks.Count; i++)
            {

            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates whatWeakness()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                //
                if(m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_bluntWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_slashWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_pierceWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_fireWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_windWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_earthWeak == true)
                {

                }

                //
                else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_waterWeak == true)
                {

                }
            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates powerVHP()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for(int j =0; j < m_enemy.m_attacks.Count; j++)
                {

                }
            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    private NodeStates selectAttack()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for (int j = 0; j < m_enemy.m_attacks.Count; j++)
                {

                }
            }
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.SUCCESS;
    }

    //

    //
    //
    void setDTAttackData()
    {
        if (m_attackDTData.Count < 7)
        {
            for (int i = 0; i < 8; i++)
            {
                //
                m_dtAttack.GetComponent<DTAttack>().m_dtName = m_enemy.m_attacks[i].m_attackName;
                //
                m_dtAttack.GetComponent<DTAttack>().m_dtBlunt = m_enemy.m_attacks[i].m_blunt;
                m_dtAttack.GetComponent<DTAttack>().m_dtSlash = m_enemy.m_attacks[i].m_slash;
                m_dtAttack.GetComponent<DTAttack>().m_dtPierce = m_enemy.m_attacks[i].m_pierce;
                m_dtAttack.GetComponent<DTAttack>().m_dtFire = m_enemy.m_attacks[i].m_fire;
                m_dtAttack.GetComponent<DTAttack>().m_dtWind = m_enemy.m_attacks[i].m_wind;
                m_dtAttack.GetComponent<DTAttack>().m_dtEarth = m_enemy.m_attacks[i].m_earth;
                m_dtAttack.GetComponent<DTAttack>().m_dtWater = m_enemy.m_attacks[i].m_water;
                //
                m_dtAttack.GetComponent<DTAttack>().m_dtDamage = m_enemy.m_attacks[i].m_attackDamage;
                m_dtAttack.GetComponent<DTAttack>().m_dtMP = m_enemy.m_attacks[i].m_attackCost;
                m_dtAttack.GetComponent<DTAttack>().m_dtAttackPriority = 0;

                m_attackDTData.Add(m_dtAttack.GetComponent<DTAttack>());
            }
        }
    }

    //
    void setBTAttackData()
    {
        for (int i = 0; i < m_enemy.m_attacks.Count - 1; i++)
        {

        }
    }

    //
}
