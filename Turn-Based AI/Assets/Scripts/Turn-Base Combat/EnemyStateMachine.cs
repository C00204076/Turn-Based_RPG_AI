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
    public enum AIState
    {
        RANDOM,
        DT,
        BT
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
    //public GameObject m_btTargetObject = new GameObject();
    //
    public List<DTAttack> m_attackDTData = new List<DTAttack>();
    //
    public List<DTTarget> m_targetDTData = new List<DTTarget>();
    public DTTarget m_dtTargetObject = new DTTarget();
    //


    //
    public TurnState m_currentState;
    public AIState m_aiState;
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

    //
    public Sequence m_sequenceA;
    public Sequence m_sequenceB;
    public Selector m_rootNode;
    // Targeting
    public ActionNode m_lowestHPA;
    public ActionNode m_lowestMPB;
    public ActionNode m_canHealC;
    public ActionNode m_selectTargetD;
    // Attacking
    public ActionNode m_currentMPA;
    public ActionNode m_weaknessB;
    public ActionNode m_powerAttackC;
    public ActionNode m_selectAttackD;

    // Start is called before the first frame update
    void Start()
    {
        //
        m_attackTime = 0.0f;
        //
        m_currentState = TurnState.PROCESSING;
        //m_aiState = AIState.RANDOM;
        //
        m_bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>();
        m_enemyObject = GameObject.Find("Enemy");
        m_startPos = transform.position;
        //
        attack = new HandleTurn();
        //
        m_animationSpeed = 5.0f;

        //
        m_lowestHPA = new ActionNode(lowestHP);
        m_lowestMPB = new ActionNode(lowestMP);
        m_canHealC = new ActionNode(canHeal);
        m_selectTargetD = new ActionNode(selectTarget);
        //
        m_currentMPA = new ActionNode(currentMP);
        m_weaknessB = new ActionNode(whatWeakness);
        m_powerAttackC = new ActionNode(powerVHP);
        m_selectAttackD = new ActionNode(selectAttack);
        //
        List<Node> m_rootA = new List<Node>();

        m_rootA.Add(m_lowestHPA);
        m_rootA.Add(m_lowestMPB);
        m_rootA.Add(m_canHealC);
        m_rootA.Add(m_selectTargetD);

        //m_rootNode = new Selector(m_rootA);
        m_sequenceA = new Sequence(m_rootA);
        //
        List<Node> m_rootB = new List<Node>();

        m_rootB.Add(m_currentMPA);
        m_rootB.Add(m_weaknessB);
        m_rootB.Add(m_powerAttackC);
        m_rootB.Add(m_selectAttackD);

        m_sequenceB = new Sequence(m_rootB);
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(m_targetDTData[0].GetComponent<DTTarget>().m_lowHP);
        /*Debug.Log(m_attackDTData[0].GetComponent<DTAttack>().m_dtAttackPriority);
        m_attackDTData[0].GetComponent<DTAttack>().m_dtAttackPriority++;*/

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
                attack = new HandleTurn();
                //clearData();
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
        if(m_aiState == AIState.RANDOM)
        {
            //Debug.Log("Random: We're here!");
            //
            attack.Attacker = m_enemy.name;
            attack.Type = "Enemy";
            attack.AttackingObject = GameObject.Find("Enemy");

            attack.Target = m_bsm.m_heroes[Random.Range(0, m_bsm.m_heroes.Count)];
            setHeroTarget(attack.Target);

            //
            int num = Random.Range(0, m_enemy.m_attacks.Count);
            attack.m_choosenAttack = m_enemy.m_attacks[num];
            Debug.Log("Random: " + this.gameObject.name + " has choosen " +
                      attack.m_choosenAttack.m_attackName + " and does " +
                      attack.m_choosenAttack.m_attackDamage + " damage!");
        }
        //
        else if (m_aiState == AIState.DT)
        {
            attack.Attacker = m_enemy.name;
            attack.Type = "Enemy";
            attack.AttackingObject = GameObject.Find("Enemy");
            //
            setDTTargetData();
            dtLowHP();
            dtLowMP();
            //
            //var trunk = MainDecisionTree();
            //trunk.Evaluate(john);

            selectDTTarget();
            //
            setDTAttackData();

            //var trunk = MainDecisionTree();
            //trunk.Evaluate(john);

            selectDTAttack();
            //
            Debug.Log(this.gameObject.name + " has choosen " +
                      attack.m_choosenAttack.m_attackName + " and does " +
                      attack.m_choosenAttack.m_attackDamage + " damage!");
        }
        //
        else if (m_aiState == AIState.BT)
        {
            //Debug.Log("BT: We're here!");
            attack.Attacker = m_enemy.name;
            attack.Type = "Enemy";
            attack.AttackingObject = GameObject.Find("Enemy");
            setBTTargetData();


            m_sequenceA.Evaluate();

            //
            setBTAttackData();

            m_sequenceB.Evaluate();

            Debug.Log("BT:" + this.gameObject.name + " has choosen " +
                      attack.m_choosenAttack.m_attackName + " and does " +
                      attack.m_choosenAttack.m_attackDamage + " damage!");
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
    public abstract class Decision
    {
        public abstract void evaluate(DTTarget target);
    }

    //
    public class DecisionQuery : Decision
    {
        public string title { get; set; }
        public Decision positive { get; set; }
        public Decision negative { get; set; }
        public System.Func<DTTarget, bool> targetTest { get; set; }

        public override void evaluate(DTTarget target)
        {
            bool result = this.targetTest(target);

            Debug.Log(this.title);

            if (result)
            {
                this.positive.evaluate(target);
            }
            else
            {
                this.negative.evaluate(target);
            }
        }

    }

    //
    public class DecisionResult : Decision
    {
        public bool result { get; set; }
        public override void evaluate(DTTarget target)
        {

        }
    }

    //
    private DecisionQuery targetDecisionTree()
    {
        
         
        //Decision 9
        var waterWeak = new DecisionQuery
        {
            title = "Finding Water weakness",
            targetTest = (target) => target.m_dtWaterWeak = true,
            positive = new DecisionResult { result = true },
            negative = new DecisionResult { result = false }
        };

        //Decision 8
        var earthWeak = new DecisionQuery
        {
            title = "Finding Earth weakness",
            targetTest = (target) => target.m_dtEarthWeak = true,
            positive = waterWeak,
            negative = waterWeak
        };

        //Decision 7
        var windWeak = new DecisionQuery
        {
            title = "Finding Wind weakness",
            targetTest = (target) => target.m_dtWindWeak = true,
            positive = earthWeak,
            negative = earthWeak
        };

        //Decision 6
        var fireWeak = new DecisionQuery
        {
            title = "Finding Fire weakness",
            targetTest = (target) => target.m_dtFireWeak = true,
            positive = windWeak,
            negative = windWeak
        };
        
        //Decision 5
        var pierceWeak = new DecisionQuery
        {
            title = "Finding Pierce weakness",
            targetTest = (target) => target.m_dtPierceWeak = true,
            positive = fireWeak,
            negative = fireWeak
        };

        //Decision 4
        var slashWeak = new DecisionQuery
        {
            title = "Finding Slash weakness",
            targetTest = (target) => target.m_dtSlashWeak = true,
            positive = pierceWeak,
            negative = pierceWeak
        };


        //Decision 3
        var bluntWeak = new DecisionQuery
        {
            title = "Finding Blunt weakness",
            targetTest = (target) => target.m_dtBluntWeak = true,
            positive = slashWeak,
            negative = slashWeak
        };
   
         


        //Decision 2
        var canHeal = new DecisionQuery
        {
            title = "Finding if they can heal",
            targetTest = (target) => target.m_dtCanHeal = true,
            positive = bluntWeak,
            negative = bluntWeak
        };

        //Decision 1
        var lowestMP = new DecisionQuery
        {
            title = "Finding lowest MP",
            targetTest = (target) => target.m_lowMP = true,
            positive = new DecisionResult { result = true },
            negative = canHeal
        };

        //Decision 0
        var lowestHP = new DecisionQuery
        {
            title = "Finding lowest HP",
            targetTest = (target) => target.m_lowHP = true,
            positive = canHeal,
            negative = lowestMP
        };

        return lowestHP;
    }


    //
    public abstract class DecisionTwo
    {
        public abstract void evaluate(DTAttack attack);
    }

    //
    public class DecisionQueryTwo : DecisionTwo
    {
        public string title { get; set; }
        public DecisionTwo positive { get; set; }
        public DecisionTwo negative { get; set; }
        public System.Func<DTAttack, bool> attackTest { get; set; }
        public System.Func<DTTarget, bool> targetTest { get; set; }
        

        public override void evaluate(DTAttack attack)
        {

            bool result = this.attackTest(attack);


            Debug.Log(this.title);

            if (result)
            {
                this.positive.evaluate(attack);
            }
            else
            {
                this.negative.evaluate(attack);
            }
        }

    }

    //
    public class DecisionResultTwo : DecisionTwo
    {
        public bool result { get; set; }
        public override void evaluate(DTAttack attack)
        {

        }
    }

    //
    private DecisionQueryTwo attackDecisionTree()
    {
        //Decision 4
        /*var creditBranch = new DecisionQueryTwo
        {
            Title = "Use credit card",
            Test = (client) => client.UsesCreditCard,
            Positive = new DecisionResult { Result = true },
            Negative = new DecisionResult { Result = false }
        };

        //Decision 3
        var experienceBranch = new DecisionQueryTwo
        {
            Title = "Have more than 3 years experience",
            Test = (client) => client.YearsInJob > 3,
            Positive = creditBranch,
            Negative = new DecisionResult { Result = false }
        };*/


        //Decision 2
        var useBlunt = new DecisionQueryTwo
        {
            title = "Use Blunt",
            attackTest = (attack) => attack.m_dtBlunt = m_dtTargetObject.m_dtBluntWeak,
            //positive = experienceBranch,
            negative = new DecisionResultTwo { result = false }
        };
        
        

        //Decision 0
        var currentMP = new DecisionQueryTwo
        {
            title = "Checking if there is enough MP",
            attackTest = (attack) => attack.m_dtMP < m_enemy.m_currentMP,
            positive = useBlunt,
            negative = new DecisionResultTwo { result = false }
        };

        return currentMP;
    }

    //
    void dtLowHP()
    {
        float lowHP = m_targetDTData[0].GetComponent<DTTarget>().m_dtTargetHP;

        foreach (DTTarget a in m_targetDTData)
        {
            if (a.GetComponent<DTTarget>().m_dtTargetHP < lowHP)
            {
                lowHP = a.GetComponent<DTTarget>().m_dtTargetHP;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetHP == lowHP &&
               m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetHP != m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetMaxHP)
            {
                m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetPriority += 1;
                m_targetDTData[i].GetComponent<DTTarget>().m_lowHP = true;
                break;
            }

        }

    }

    //
    void dtLowMP()
    {
        float lowMP = m_targetDTData[0].GetComponent<DTTarget>().m_dtTargetMP;

        foreach (DTTarget a in m_targetDTData)
        {
            if (a.GetComponent<DTTarget>().m_dtTargetMP < lowMP)
            {
                lowMP = a.GetComponent<DTTarget>().m_dtTargetMP;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetMP == lowMP &&
               m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetMP != m_attackDTData[i].GetComponent<DTTarget>().m_dtTargetMaxMP)
            {
                m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetPriority += 1;
                m_targetDTData[i].GetComponent<DTTarget>().m_lowMP = true;
                break;
            }
        }

    }

    //
    void selectDTAttack()
    {
        int highPrior = 0;

        if (attack.m_choosenAttack == null)
        {
            foreach (DTAttack a in m_attackDTData)
            {
                if (a.GetComponent<DTAttack>().m_dtAttackPriority >= highPrior)
                {
                    highPrior = a.GetComponent<DTAttack>().m_dtAttackPriority;
                }
            }//End for

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for (int j = 0; j < m_enemy.m_attacks.Count; j++)
                {
                    // Select attack with highest priority value
                    if (m_attackDTData[i].GetComponent<DTAttack>().m_dtAttackPriority == highPrior)
                    {
                        // Set attack
                    }
                }//End for
            }//End for
        }
    }

    //
    void selectDTTarget()
    {
        int highPrior = 0;

        if (attack.m_choosenAttack == null)
        {
            foreach (DTTarget a in m_targetDTData)
            {
                if (a.GetComponent<DTTarget>().m_dtTargetPriority >= highPrior)
                {
                    highPrior = a.GetComponent<DTTarget>().m_dtTargetPriority;
                }
            }//End for

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for (int j = 0; j < m_enemy.m_attacks.Count; j++)
                {
                    // Select attack with highest priority value
                    if (m_attackDTData[i].GetComponent<DTTarget>().m_dtTargetPriority == highPrior)
                    {
                        //
                        if (m_targetDTData[i].GetComponent<DTTarget>().m_dtName == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name)
                        {
                            attack.Target = m_bsm.m_heroes[i];
                            setHeroTarget(attack.Target);
                        }
                    }
                }//End for
            }//End for
        }
    }


    // Behaviour Tree
    //
    // Hero targeting
    private NodeStates lowestHP()
    {
        float lowHP = m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_hero.m_currentHP;
        

        if (attack.Target == null)
        {
            foreach (GameObject a in m_bsm.m_heroes)
            {
                if (a.GetComponent<HeroStateMachine>().m_hero.m_currentHP <= lowHP)
                {
                    lowHP = a.GetComponent<HeroStateMachine>().m_hero.m_currentHP;
                }
            }

            for (int i = 0; i < 2; i++)
            {
                
                // If target data HP is equal to the HP of any of the heroes...
                if (lowHP == m_targetBTData[i].GetComponent<BTTarget>().m_btTargetHP)
                {
                    m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;          
                    /*if(m_firstAttack == false)
                    {
                        m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                    }
                    // ...and is not equal to base HP(maximum) of any of the heroes
                    else if (lowHP != m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseHP)
                    {
                        // Increase the target's priority value
                        m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                    }*/
                }

                if(i >= 2)
                {
                    return NodeStates.SUCCESS;
                }
            }//End for
        }

        // Returns failure if their is not data to use
        else if (m_targetBTData == null)
        {
            return NodeStates.FAILURE;
        }


        return NodeStates.SUCCESS;
       

        //return NodeStates.FAILURE;
    }

    //
    private NodeStates lowestMP()
    {
        float lowMP = m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_hero.m_currentMP;

        //Debug.Log("First lowMP: " + lowMP);

        if (attack.Target == null)
        {
            foreach (GameObject a in m_bsm.m_heroes)
            {
                if (a.GetComponent<HeroStateMachine>().m_hero.m_currentMP <= lowMP)
                {
                    lowMP = a.GetComponent<HeroStateMachine>().m_hero.m_currentMP;
                    //Debug.Log("New lowMP: " + lowMP);
                }
            }

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                // If target data MP is equal to the MP of any of the heroes...
                if (lowMP == m_targetBTData[i].GetComponent<BTTarget>().m_btTargetMP)
                {
                    m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                    
                    
                    /*// ...and is not equal to base MP(maximum) of any of the heroes
                    if (lowMP != m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseMP)
                    {
                        // Increase the target's priority value
                        m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                    }*/
                }

                if(i >=2)
                {
                    return NodeStates.SUCCESS;
                }
            }//End for

            
        }

        // Returns failure if their is not data to use
        else if (m_targetBTData == null)
        {
            return NodeStates.FAILURE;
        }

        
        return NodeStates.SUCCESS;
    }

    //
    private NodeStates canHeal()
    {
        //
        if (attack.Target == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for (int j = 0; j < m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks.Count; j++)
                {
                    //Debug.Log("First Priority: " + m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority);
                    // If any of the heroes have any healing abilities...
                    if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Guiding Light" ||
                        m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Blessed Water")
                    {
                        // ...increase the target's priority value
                        m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                        /*Debug.Log("i: " + i);
                        Debug.Log("j: " + j);
                        Debug.Log("New Priority: " + m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority);*/
                    }// End if  


                    if (i >= 2 && j >= 7)
                    {
                        return NodeStates.SUCCESS;
                    }
                }// End for

            }// End for

            return NodeStates.SUCCESS;
        }// End if

        // Returns failure if their is not data to use
        else if (m_targetBTData == null)
        {
            return NodeStates.FAILURE;
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    private NodeStates selectTarget()
    {
        int highPrior = 0;
        //
        if (attack.Target == null)
        {
            foreach (BTTarget a in m_targetBTData)
            {
                if (a.GetComponent<BTTarget>().m_btTargetPriority >= highPrior)
                {
                    highPrior = a.GetComponent<BTTarget>().m_btTargetPriority;
                }
            }

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                // Select target with the highest Priority value...
                if (m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority == highPrior)
                {
                    //
                    if(m_targetBTData[i].GetComponent<BTTarget>().m_btName == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name)
                    {
                        attack.Target = m_bsm.m_heroes[i];
                        setHeroTarget(attack.Target);
                    }

                    if (i >= 2)
                    {
                        return NodeStates.SUCCESS;
                    }
                }
            }//End for

            //return NodeStates.SUCCESS;
        }

        // Returns failure if their is no data to use
        else if (m_targetBTData == null)
        {
            return NodeStates.FAILURE;
        }

        //
        else
        {
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }


    // Attack decision
    private NodeStates currentMP()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_enemy.m_attacks.Count; i++)
            {
                if(m_enemy.m_currentMP < m_attackBTData[i].GetComponent<BTAttack>().m_btMP)
                {
                    m_attackBTData[i].GetComponent<BTAttack>().m_cantUse = true;
                }
                else
                {
                    m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                if(i >= 7)
                {
                    return NodeStates.SUCCESS;
                }
            }
        }

        // Returns failure if their is not data to use
        else if (m_attackBTData == null)
        {
            return NodeStates.FAILURE;
        }

        return NodeStates.SUCCESS;
    }

    //
    private NodeStates whatWeakness()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for (int j = 0; j < m_enemy.m_attacks.Count; j++)
                {
                    //
                    if(m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_bluntWeak == true &&
                       m_attackBTData[i].GetComponent<BTAttack>().m_btBlunt == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_slashWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btSlash == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_pierceWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btPierce == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_fireWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btFire == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_windWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btWind == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_earthWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btEarth == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    //
                    else if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_waterWeak == true &&
                             m_attackBTData[i].GetComponent<BTAttack>().m_btWater == true)
                    {
                        m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
                    }

                    if(i >= 2 && j >= 7)
                    {
                        return NodeStates.SUCCESS;
                    }
                }
            }//End for
        }//End if

        // Returns failure if their is not data to use
        else if (m_attackBTData == null)
        {
            return NodeStates.FAILURE;
        }


        return NodeStates.SUCCESS;
    }

    //
    private NodeStates powerVHP()
    {
        //
        if (attack.m_choosenAttack == null)
        {
            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                for(int j =0; j < m_enemy.m_attacks.Count; j++)
                {
                    if (m_attackBTData[j].GetComponent<BTAttack>().m_cantUse == false)
                    {
                        // If the attacks damage is greater than the heroes hp
                        if (m_attackBTData[j].GetComponent<BTAttack>().m_btDamage < m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentHP)
                        {
                            m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 4;
                        }// End if
                    }// End if

                    if (i >= 2 && j >= 7)
                    {
                        return NodeStates.SUCCESS;
                    }

                }// End for
            }//End for
        }//End if

        // Returns failure if their is not data to use
        else if (m_attackBTData == null)
        {
            return NodeStates.FAILURE;
        }

        return NodeStates.SUCCESS;
    }

    //
    private NodeStates selectAttack()
    {
        int highPrior = 0;

        //
        if (attack.m_choosenAttack == null)
        {
            foreach (BTAttack a in m_attackBTData)
            {
                if (a.GetComponent<BTAttack>().m_btAttackPriority >= highPrior)
                {
                    highPrior = a.GetComponent<BTAttack>().m_btAttackPriority;
                }
            }//End for


            for (int j = 0; j < m_enemy.m_attacks.Count; j++)
            {
                // Select attack with highest priority value
                if (m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority == highPrior)
                {
                    if (m_attackBTData[j].GetComponent<BTAttack>().m_btName == m_enemy.m_attacks[j].m_attackName)
                    {
                        attack.m_choosenAttack = m_enemy.m_attacks[j];
                        return NodeStates.SUCCESS;
                    }
                }

                if(j >= 7)
                {
                    return NodeStates.SUCCESS;
                }
            }//End for


            return NodeStates.SUCCESS;
        }
    
        // Returns failure if their is not data to use
        else if(m_attackBTData == null)
        {
            return NodeStates.FAILURE;
        }
        
        return NodeStates.SUCCESS;
    }

    //

    //
    //
    void setDTAttackData()
    {
        //
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
                //
                m_attackDTData.Add(m_dtAttack.GetComponent<DTAttack>());
            }
        }
    }

    //
    void setDTTargetData()
    {
        if (m_targetDTData.Count < 3)
        {
            for (int i = 0; i < 3; i++)
            {
                //
                m_dtTarget.GetComponent<DTTarget>().m_dtName = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name;

                //
                for(int j = 0; j < 4; j++)
                {
                    if(m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName  == "Guiding Light" &&
                       m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Blessed Water")
                    {
                        m_dtTarget.GetComponent<DTTarget>().m_dtCanHeal = true;
                    }
                    else
                    {
                        m_dtTarget.GetComponent<DTTarget>().m_dtCanHeal = false;
                    }
                }

                //
                m_dtTarget.GetComponent<DTTarget>().m_dtBluntWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_bluntWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtSlashWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_slashWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtPierceWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_pierceWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtFireWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_fireWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtWindWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_windWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtEarthWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_earthWeak;
                m_dtTarget.GetComponent<DTTarget>().m_dtWaterWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_waterWeak;
                //
                m_dtTarget.GetComponent<DTTarget>().m_dtTargetMaxHP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseHP;
                m_dtTarget.GetComponent<DTTarget>().m_dtTargetMaxMP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_baseMP;
                m_dtTarget.GetComponent<DTTarget>().m_dtTargetHP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentHP;
                m_dtTarget.GetComponent<DTTarget>().m_dtTargetMP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentMP;
                m_dtTarget.GetComponent<DTTarget>().m_dtTargetPriority = 0;
                //
                m_targetDTData.Add(m_dtTarget.GetComponent<DTTarget>());
            }
        }

        
    }

    //
    void setBTAttackData()
    {
        //
        if (m_attackBTData.Count < 7)
        {
            for (int i = 0; i < 8; i++)
            {
                //
                m_btAttack.GetComponent<BTAttack>().m_btName = m_enemy.m_attacks[i].m_attackName;
                //
                m_btAttack.GetComponent<BTAttack>().m_btBlunt = m_enemy.m_attacks[i].m_blunt;
                m_btAttack.GetComponent<BTAttack>().m_btSlash = m_enemy.m_attacks[i].m_slash;
                m_btAttack.GetComponent<BTAttack>().m_btPierce = m_enemy.m_attacks[i].m_pierce;
                m_btAttack.GetComponent<BTAttack>().m_btFire = m_enemy.m_attacks[i].m_fire;
                m_btAttack.GetComponent<BTAttack>().m_btWind = m_enemy.m_attacks[i].m_wind;
                m_btAttack.GetComponent<BTAttack>().m_btEarth = m_enemy.m_attacks[i].m_earth;
                m_btAttack.GetComponent<BTAttack>().m_btWater = m_enemy.m_attacks[i].m_water;
                //
                m_btAttack.GetComponent<BTAttack>().m_btDamage = m_enemy.m_attacks[i].m_attackDamage;
                m_btAttack.GetComponent<BTAttack>().m_btMP = m_enemy.m_attacks[i].m_attackCost;
                m_btAttack.GetComponent<BTAttack>().m_btAttackPriority = 0;

                m_attackBTData.Add(m_btAttack.GetComponent<BTAttack>());
            }
        }
    }

    //
    void setBTTargetData()
    {
        if (m_targetBTData.Count < 3)
        {
            for (int i = 0; i < 3; i++)
            {
                //
                m_btTarget.GetComponent<BTTarget>().m_btName = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name;

                //
                for (int j = 0; j < 4; j++)
                {
                    if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Guiding Light" &&
                       m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Blessed Water")
                    {
                        m_btTarget.GetComponent<BTTarget>().m_btCanHeal = true;
                    }
                    else
                    {
                        m_btTarget.GetComponent<BTTarget>().m_btCanHeal = false;
                    }
                }

                //
                m_btTarget.GetComponent<BTTarget>().m_btBluntWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_bluntWeak;
                m_btTarget.GetComponent<BTTarget>().m_btSlashWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_slashWeak;
                m_btTarget.GetComponent<BTTarget>().m_btPierceWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_pierceWeak;
                m_btTarget.GetComponent<BTTarget>().m_btFireWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_fireWeak;
                m_btTarget.GetComponent<BTTarget>().m_btWindWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_windWeak;
                m_btTarget.GetComponent<BTTarget>().m_btEarthWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_earthWeak;
                m_btTarget.GetComponent<BTTarget>().m_btWaterWeak = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_waterWeak;
                //
                m_btTarget.GetComponent<BTTarget>().m_btTargetHP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentHP;
                m_btTarget.GetComponent<BTTarget>().m_btTargetMP = m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentMP;
                m_btTarget.GetComponent<BTTarget>().m_btTargetPriority = 0;
                //
                m_targetBTData.Add(m_btTarget.GetComponent<BTTarget>());
            }
        }
    }

    //
    void clearData()
    {
        m_attackBTData.Clear();

        m_targetBTData.Clear();

        m_attackDTData.Clear();

        m_targetDTData.Clear();
    }

    //
}
