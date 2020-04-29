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
    public List<BTAttack> m_attackBTData = new List<BTAttack>();
    //
    public List<BTTarget> m_targetBTData = new List<BTTarget>();
    public BTTarget m_btTargetObject = new BTTarget();
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
    public GameObject m_heroTarget;
    private float m_animationSpeed;

    private bool m_alive = true;

    private bool m_firstAttack = false;
    public bool m_hit = false, m_attacking = false;
    private int left = 0, right = 0;
    private int timer = 0;
    private int aniTimer = 0;

    //
    public Sequence m_sequenceA;
    public Sequence m_sequenceB;
    public Selector m_rootNode;
    // Targeting
    public ActionNode m_lowestHPA;
    public ActionNode m_lowestMPB;
    public ActionNode m_canHealC;
    public ActionNode m_hasWeaknessD;
    public ActionNode m_selectTargetE;
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
        m_hasWeaknessD = new ActionNode(hasWeaknesses);
        m_selectTargetE = new ActionNode(selectTarget);
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
        m_rootA.Add(m_hasWeaknessD);
        m_rootA.Add(m_selectTargetE);

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
        //wasHit();
        attackAni();

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
                attack = new HandleTurn();
                
                break;
            case TurnState.CHOOSEACTION:
                if(m_bsm.m_heroes.Count > 0)
                {
                    chooseAction();
                }
                m_enemy.m_currentMP += 50;
                m_currentState = TurnState.ACTION;
                break;
            case TurnState.WAITING:
                // Idling
                break;
            case TurnState.ACTION:
                actionTime();
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

            int num = Random.Range(0, m_bsm.m_heroes.Count);
            attack.Target = m_bsm.m_heroes[Random.Range(0, m_bsm.m_heroes.Count)];
            setHeroTarget(attack.Target);
            m_bsm.m_heroes[num].GetComponent<HeroStateMachine>().m_hit = true;

            //
            num = Random.Range(0, m_enemy.m_attacks.Count);
            attack.m_choosenAttack = m_enemy.m_attacks[num];
            Debug.Log("Random: " + this.gameObject.name + " has choosen " +
                      attack.m_choosenAttack.m_attackName + " and does " +
                      attack.m_choosenAttack.m_attackDamage + " damage!");
        }
        //
        else if (m_aiState == AIState.DT)
        {
            //Debug.Log("DT: We're here!");
            //
            attack.Attacker = m_enemy.name;
            attack.Type = "Enemy";
            attack.AttackingObject = GameObject.Find("Enemy");
            
            //
            setDTTargetData();
            //
            var targetDT = targetDecisionTree();

            targetDT.evaluate(m_targetDTData[0]);
            targetDT.evaluate(m_targetDTData[1]);
            targetDT.evaluate(m_targetDTData[2]);

            selectDTTarget();
            
            //
            setDTAttackData();

            var attackDT = attackDecisionTree();

            attackDT.evaluate(m_attackDTData[0]);
            attackDT.evaluate(m_attackDTData[1]);
            attackDT.evaluate(m_attackDTData[2]);
            attackDT.evaluate(m_attackDTData[3]);
            attackDT.evaluate(m_attackDTData[4]);
            attackDT.evaluate(m_attackDTData[5]);
            attackDT.evaluate(m_attackDTData[6]);


            selectDTAttack();
            //m_bsm.m_heroes[num].GetComponent<HeroStateMachine>().m_hit = true;

            Debug.Log(attack.m_choosenAttack);
            //
            Debug.Log("DT: " + this.gameObject.name + " has choosen " +
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
            

            //
            setBTTargetData();

            m_sequenceA.Evaluate();

            /*Debug.Log(m_targetBTData[0].m_btTargetPriority);
            Debug.Log(m_targetBTData[1].m_btTargetPriority);
            Debug.Log(m_targetBTData[2].m_btTargetPriority);*/

            //
            setBTAttackData();

            m_sequenceB.Evaluate();

            /*Debug.Log(m_enemy.m_attacks[0].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[1].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[2].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[3].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[4].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[5].m_btAttack.m_btAttackPriority);
            Debug.Log(m_enemy.m_attacks[6].m_btAttack.m_btAttackPriority);//*/
            //m_bsm.m_heroes[num].GetComponent<HeroStateMachine>().m_hit = true;

            Debug.Log("BT:" + this.gameObject.name + " has choosen " +
                      attack.m_choosenAttack.m_attackName + " and does " +
                      attack.m_choosenAttack.m_attackDamage + " damage!");
        }

        //
        m_bsm.collectActions(attack);
        clearData();
    }

    //IEnumerator
    void actionTime()
    {
        m_attacking = true;
        // Do damage
        doDamage();
        // Animate back to sart position
        Vector3 firstPos = m_startPos;

        // Remove performer from BSM list
        m_bsm.m_performList.RemoveAt(0);
        // Reset BSM -> Wait
        m_bsm.m_battleStates = BattleStateMachine1.PerformAction.WAIT;
        // Reset this enemy state
        m_attackTime = 0.0f;
        m_currentState = TurnState.PROCESSING;
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
        //attack*(100/(100+defense))
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

            //Debug.Log(this.title);

            if (result)
            {
                this.positive.evaluate(target);
                target.m_dtTargetPriority++;
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
            targetTest = (target) => target.m_dtWaterWeak == true,
            positive = new DecisionResult { result = true },
            negative = new DecisionResult { result = false }
        };

        //Decision 8
        var earthWeak = new DecisionQuery
        {
            title = "Finding Earth weakness",
            targetTest = (target) => target.m_dtEarthWeak == true,
            positive = waterWeak,
            negative = waterWeak
        };

        //Decision 7
        var windWeak = new DecisionQuery
        {
            title = "Finding Wind weakness",
            targetTest = (target) => target.m_dtWindWeak == true,
            positive = earthWeak,
            negative = earthWeak
        };

        //Decision 6
        var fireWeak = new DecisionQuery
        {
            title = "Finding Fire weakness",
            targetTest = (target) => target.m_dtFireWeak == true,
            positive = windWeak,
            negative = windWeak
        };

        //Decision 7
        var magicWeak = new DecisionQuery
        {
            title = "Finding physical weaknesses",
            targetTest = (target) => target.m_magicWeak == true,
            positive = fireWeak,
            negative = new DecisionResult { result = false }
        };

        //Decision 6
        var pierceWeak = new DecisionQuery
        {
            title = "Finding Pierce weakness",
            targetTest = (target) => target.m_dtPierceWeak == true,
            positive = magicWeak,
            negative = magicWeak
        };

        //Decision 5
        var slashWeak = new DecisionQuery
        {
            title = "Finding Slash weakness",
            targetTest = (target) => target.m_dtSlashWeak == true,
            positive = pierceWeak,
            negative = pierceWeak
        };

        //Decision 4
        var bluntWeak = new DecisionQuery
        {
            title = "Finding Blunt weakness",
            targetTest = (target) => target.m_dtBluntWeak == true,
            positive = slashWeak,
            negative = slashWeak
        };

        //Decision 3
        var physicalWeak = new DecisionQuery
        {
            title = "Finding physical weaknesses",
            targetTest = (target) => target.m_physicalWeak == true,
            positive = bluntWeak,
            negative = magicWeak
        };

        //Decision 2
        var canHeal = new DecisionQuery
        {
            title = "Finding if they can heal",
            targetTest = (target) => target.m_dtCanHeal == true,
            positive = physicalWeak,
            negative = physicalWeak
        };

        //Decision 1
        var lowestMP = new DecisionQuery
        {
            title = "Finding lowest MP",
            targetTest = (target) => target.m_lowMP == true,
            positive = canHeal,
            negative = canHeal
        };

        //Decision 0
        var lowestHP = new DecisionQuery
        {
            title = "Finding lowest HP",
            targetTest = (target) => target.m_lowHP == true,
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

            //Debug.Log(this.title);
            

            if (result)
            {
                this.positive.evaluate(attack);
                attack.m_dtAttackPriority++;
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
        
        
        //Decision 7
        var useWater = new DecisionQueryTwo
        {
            title = "Use Water",
            attackTest = (attack) => attack.m_dtWater == m_dtTargetObject.m_dtWaterWeak,
            positive = new DecisionResultTwo  { result = true },
            negative = new DecisionResultTwo { result = false }
        };

        //Decision 6
        var useEarth = new DecisionQueryTwo
        {
            title = "Use Earth",
            attackTest = (attack) => attack.m_dtEarth == m_dtTargetObject.m_dtEarthWeak,
            positive = useWater,
            negative = useWater
        };

        //Decision 5
        var useWind = new DecisionQueryTwo
        {
            title = "Use Wind",
            attackTest = (attack) => attack.m_dtWind == m_dtTargetObject.m_dtWindWeak,
            positive = useEarth,
            negative = useEarth
        };

        //Decision 4
        var useFire = new DecisionQueryTwo
        {
            title = "Use Fire",
            attackTest = (attack) => attack.m_dtFire == m_dtTargetObject.m_dtFireWeak,
            positive = useWind,
            negative = useWind
        };

        //Decision 3
        var usePierce = new DecisionQueryTwo
        {
            title = "Use Pierce",
            attackTest = (attack) => attack.m_dtPierce == m_dtTargetObject.m_dtPierceWeak,
            positive = useFire,
            negative = useFire
        };

        //Decision 2
        var useSlash = new DecisionQueryTwo
        {
            title = "Use Slash",
            attackTest = (attack) => attack.m_dtSlash == m_dtTargetObject.m_dtSlashWeak,
            positive = usePierce,
            negative = usePierce
        };


        //Decision 1
        var useBlunt = new DecisionQueryTwo
        {
            title = "Use Blunt",
            attackTest = (attack) => attack.m_dtBlunt == m_dtTargetObject.m_dtBluntWeak,
            positive = useSlash,
            negative = useSlash
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
    void selectDTAttack()
    {
        int highPrior = 0;

        float highAttack = m_attackDTData[0].GetComponent<DTAttack>().m_dtDamage;

        if (attack.m_choosenAttack == null)
        {
            foreach (DTAttack a in m_attackDTData)
            {
                if (a.GetComponent<DTAttack>().m_dtAttackPriority >= highPrior)
                {
                    highPrior = a.GetComponent<DTAttack>().m_dtAttackPriority;
                }
            }//End for

            foreach (DTAttack a in m_attackDTData)
            {
                if (a.GetComponent<DTAttack>().m_dtDamage >= highAttack)
                {
                    highAttack = a.GetComponent<DTAttack>().m_dtDamage;
                }
            }

            for (int i = 0; i < m_enemy.m_attacks.Count; i++)
            {
                // Select attack with highest priority value
                if (m_attackDTData[i].GetComponent<DTAttack>().m_dtAttackPriority == highPrior)
                {
                    if (m_attackDTData[i].GetComponent<DTAttack>().m_cantUse == false)
                    {
                        if (m_attackDTData[i].GetComponent<DTAttack>().m_dtName == m_enemy.m_attacks[i].m_attackName)
                        {
                            if (m_attackDTData[i].GetComponent<DTAttack>().m_dtDamage == highAttack)
                            {
                                attack.m_choosenAttack = m_enemy.m_attacks[i];
                            }

                            else
                            {
                                attack.m_choosenAttack = m_enemy.m_attacks[i];
                            }
                        }
                    }
                    
                }//End if
            }//End for
        }
    }

    //
    void selectDTTarget()
    {
        int highPrior = 0;

        if (attack.Target == null)
        {
            foreach (DTTarget a in m_targetDTData)
            {
                if (a.GetComponent<DTTarget>().m_dtTargetPriority >= highPrior)
                {
                    highPrior = a.GetComponent<DTTarget>().m_dtTargetPriority;
                }
            }//End for

            //Debug.Log("DT: We're here!");

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                // Select attack with highest priority value
                if (m_targetDTData[i].GetComponent<DTTarget>().m_dtTargetPriority == highPrior)
                {
                    //
                    if (m_targetDTData[i].GetComponent<DTTarget>().m_dtName == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name)
                    {
                        attack.Target = m_bsm.m_heroes[i];
                        m_dtTargetObject = m_targetDTData[i];
                        setHeroTarget(attack.Target);
                        break;
                    }
                }

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

            for (int i = 0; i < m_bsm.m_heroes.Count; i++)
            {
                if (i > 2)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                // If target data HP is equal to the HP of any of the heroes...
                if (lowHP == m_targetBTData[i].GetComponent<BTTarget>().m_btTargetHP)
                {
                    m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;    
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
                if (i > 2)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                // If target data MP is equal to the MP of any of the heroes...
                if (lowMP == m_targetBTData[i].GetComponent<BTTarget>().m_btTargetMP)
                {
                    m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
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
                    if (i > 2 && j > 7)
                    {
                        return NodeStates.SUCCESS;
                        break;
                    }

                    //Debug.Log("First Priority: " + m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority);
                    // If any of the heroes have any healing abilities...
                    if (m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Guiding Light" ||
                        m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_attacks[j].m_attackName == "Blessed Water")
                    {
                        // ...increase the target's priority value
                        m_targetBTData[i].GetComponent<BTTarget>().m_btTargetPriority += 1;
                    }// End if  


                    
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

    private NodeStates hasWeaknesses()
    {
        //
        if (attack.Target == null)
        {

            for (int j = 0; j < m_bsm.m_heroes.Count; j++)
            {
                if (j > 2)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                //
                if (m_targetBTData[j].m_btBluntWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btSlashWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btPierceWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btFireWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btWindWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btEarthWeak == true)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

                //
                if (m_targetBTData[j].m_btWaterWeak)
                {
                    m_targetBTData[j].GetComponent<BTTarget>().m_btTargetPriority += 1;
                }

            }//End for
        }//End if

        // Returns failure if their is not data to use
        else if (m_targetBTData == null)
        {
            return NodeStates.FAILURE;
        }


        return NodeStates.SUCCESS;
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
                    if (i > 2)
                    {
                        return NodeStates.SUCCESS;
                        break;
                    }

                    //
                    if (m_targetBTData[i].GetComponent<BTTarget>().m_btName == m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_name)
                    {
                        attack.Target = m_bsm.m_heroes[i];
                        setHeroTarget(attack.Target);
                        m_btTargetObject = m_targetBTData[i];
                        break;
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
                if (i > 7)
                {
                    return NodeStates.SUCCESS;
                }

                if (m_enemy.m_currentMP < m_attackBTData[i].GetComponent<BTAttack>().m_btMP)
                {
                    m_attackBTData[i].GetComponent<BTAttack>().m_cantUse = true;
                }
                else
                {
                    m_attackBTData[i].GetComponent<BTAttack>().m_btAttackPriority += 1;
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

            for (int j = 0; j < m_enemy.m_attacks.Count; j++)
            {
                if (j > 7)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                //
                if (m_btTargetObject.m_btBluntWeak == true &&
                   m_attackBTData[j].GetComponent<BTAttack>().m_btBlunt == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btSlashWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btSlash == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btPierceWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btPierce == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btFireWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btFire == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btWindWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btWind == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btEarthWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btEarth == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
                }

                //
                if (m_btTargetObject.m_btWaterWeak == true &&
                         m_attackBTData[j].GetComponent<BTAttack>().m_btWater == true)
                {
                    m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 1;
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
        float highAttack = m_attackBTData[0].GetComponent<BTAttack>().m_btDamage;

        //
        if (attack.m_choosenAttack == null)
        {
            foreach (BTAttack a in m_attackBTData)
            {
                if (a.GetComponent<BTAttack>().m_btDamage >= highAttack)
                {
                    highAttack = a.GetComponent<BTAttack>().m_btDamage;
                }
            }


            for (int j = 0; j < m_enemy.m_attacks.Count; j++)
            {
                if (j > 7)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                if (m_attackBTData[j].GetComponent<BTAttack>().m_cantUse == false)
                {
                    // If the attacks damage is greater than the heroes hp
                    if (m_attackBTData[j].GetComponent<BTAttack>().m_btDamage < m_btTargetObject.m_btTargetHP)
                    {
                        m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 2;
                    }// End if
                    else if(m_attackBTData[j].GetComponent<BTAttack>().m_btDamage == highAttack)
                    {
                        m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority += 2;
                    }

                }// End if
            }// End for
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

        float highAttack = m_attackBTData[0].GetComponent<BTAttack>().m_btDamage;

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

            foreach (BTAttack a in m_attackBTData)
            {
                if (a.GetComponent<BTAttack>().m_btDamage >= highAttack)
                {
                    highAttack = a.GetComponent<BTAttack>().m_btDamage;
                }
            }


            for (int j = 0; j < m_enemy.m_attacks.Count; j++)
            {
                //
                if (j > 7)
                {
                    return NodeStates.SUCCESS;
                    break;
                }

                // Select attack with highest priority value
                if (m_attackBTData[j].GetComponent<BTAttack>().m_btAttackPriority == highPrior)
                {
                    if (m_attackBTData[j].GetComponent<BTAttack>().m_btName == m_enemy.m_attacks[j].m_attackName)
                    {
                        Debug.Log("Here");
                        if (m_attackBTData[j].GetComponent<BTAttack>().m_cantUse == false)
                        {
                            if (m_attackBTData[j].GetComponent<BTAttack>().m_btDamage == highAttack)
                            {

                                attack.m_choosenAttack = m_enemy.m_attacks[j];
                            }

                            else
                            {
                                attack.m_choosenAttack = m_enemy.m_attacks[j];
                            }
                        }
                        // Default
                        else
                        {
                            if (m_attackBTData[j].GetComponent<BTAttack>().m_btDamage == highAttack)
                            {

                                attack.m_choosenAttack = m_enemy.m_attacks[j];
                            }

                            else
                            {
                                attack.m_choosenAttack = m_enemy.m_attacks[j];
                            }
                        }
                    }

                    
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
    void wasHit()
    {
        if (m_hit == true)
        {
            if (timer < 100)
            {
                if (left < 5)
                {
                    this.gameObject.transform.position -= new Vector3(0.1f, 0, 0);
                    left++;
                }

                else if (right < 5)
                {
                    this.gameObject.transform.position += new Vector3(0.1f, 0, 0);
                    right++;
                }

                else
                {
                    left = 0;
                    right = 0;
                }

                timer++;
            }
            else
            {
                this.gameObject.transform.position = m_startPos;
                timer = 0;
                m_hit = false;
            }
        }
    }
    // 
    void attackAni()
    {
        if (m_attacking == true)
        {
            if (aniTimer < 10)
            {
                this.gameObject.transform.position += new Vector3(0.1f, 0, 0);
                aniTimer++;
            }
            else
            {
                this.gameObject.transform.position = m_startPos;
                aniTimer = 0;
                m_attacking = false;
            }
        }
    }

    //
    //
    void setDTAttackData()
    {
        //
        m_enemy.m_attacks[0].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[1].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[2].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[3].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[4].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[5].m_dtAttack.m_dtAttackPriority = 0;
        m_enemy.m_attacks[6].m_dtAttack.m_dtAttackPriority = 0;
        //
        m_attackDTData.Add(m_enemy.m_attacks[0].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[1].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[2].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[3].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[4].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[5].m_dtAttack);
        m_attackDTData.Add(m_enemy.m_attacks[6].m_dtAttack);
    }

    //
    void setDTTargetData()
    {
        m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_dtTarget.m_dtTargetPriority = 0;
        m_bsm.m_heroes[1].GetComponent<HeroStateMachine>().m_dtTarget.m_dtTargetPriority = 0;
        m_bsm.m_heroes[2].GetComponent<HeroStateMachine>().m_dtTarget.m_dtTargetPriority = 0;

        m_targetDTData.Add(m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_dtTarget);
        m_targetDTData.Add(m_bsm.m_heroes[1].GetComponent<HeroStateMachine>().m_dtTarget);
        m_targetDTData.Add(m_bsm.m_heroes[2].GetComponent<HeroStateMachine>().m_dtTarget);
    }

    //
    void setBTAttackData()
    {
        m_enemy.m_attacks[0].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[1].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[2].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[3].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[4].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[5].m_btAttack.m_btAttackPriority = 0;
        m_enemy.m_attacks[6].m_btAttack.m_btAttackPriority = 0;
        //
        m_attackBTData.Add(m_enemy.m_attacks[0].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[1].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[2].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[3].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[4].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[5].m_btAttack);
        m_attackBTData.Add(m_enemy.m_attacks[6].m_btAttack);
    }

    //
    void setBTTargetData()
    {
        m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_btTarget.m_btTargetPriority = 0;
        m_bsm.m_heroes[1].GetComponent<HeroStateMachine>().m_btTarget.m_btTargetPriority = 0;
        m_bsm.m_heroes[2].GetComponent<HeroStateMachine>().m_btTarget.m_btTargetPriority = 0;

        m_targetBTData.Add(m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_btTarget);
        m_targetBTData.Add(m_bsm.m_heroes[1].GetComponent<HeroStateMachine>().m_btTarget);
        m_targetBTData.Add(m_bsm.m_heroes[2].GetComponent<HeroStateMachine>().m_btTarget);
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
