using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 16:26 14 November 2019
// Finished at
// Time taken:
// Known bugs:

public class HeroStateMachine : MonoBehaviour
{
    public BaseHero m_hero;
    private BattleStateMachine1 m_bsm;
    public GameObject m_heroObject;
    //
    public DTTarget m_dtTarget;
    public BTTarget m_btTarget;
    //
    private HandleTurn attack;
    //
    public Vector3 m_startPos;
    //actionTime
    private bool m_startedAct = false;
    public GameObject m_enemyTarget;
    private bool m_healing = false;
    public bool m_hit = false, m_attacking = false;
    
    //
    public GameObject lowPlayer;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState m_currentState;

    private Image m_turnBar;

    private Vector3 m_barScale;

    private bool m_alive = true;
    // Hero panel
    private HeroPanelStats m_stats;
    public GameObject m_heroPanel;
    private Transform m_heroPanelSpacer;

    float m_barSpeed;

    private int left = 0, right = 0;
    private int timer = 0, aniTimer = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Find Spacer
        m_heroPanelSpacer = GameObject.Find("HeroPanelSpacer").transform;
        // Create panel with info
        createHeroPanel();

        //
        m_barScale = new Vector3();
        //
        m_turnBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        //
        m_bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>();
        //
        m_startPos = transform.position;
        //
        m_currentState = TurnState.PROCESSING;
        //
        m_barSpeed = m_hero.m_agility / 10000;
        //
        setDT();
        //
        setBT();
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
                break;
            case TurnState.ADDTOLIST:
                m_bsm.m_heroToManage.Add(m_heroObject);
                m_currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                // idle
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
                    // Change tag
                    this.gameObject.tag = "DeadHero";
                    // Not attackable enemy
                    m_bsm.m_heroes.Remove(this.gameObject);
                    // Not managable
                    m_bsm.m_heroToManage.Remove(this.gameObject);
                    // Deactive selector
                    //selector.SetActive(false);
                    // Reset GUI
                    //m_bsm.m_attackPanel.SetActive(false);
                    // Remove this item from preformList
                    if (m_bsm.m_heroes.Count > 0)
                    {
                        for (int i = 0; i < m_bsm.m_performList.Count; i++)
                        {
                            if (m_bsm.m_performList[i].AttackingObject == this.gameObject)
                            {
                                m_bsm.m_performList.Remove(m_bsm.m_performList[i]);
                            }

                            if (m_bsm.m_performList[i].Target == this.gameObject)
                            {
                                m_bsm.m_performList[i].Target = m_bsm.m_heroes[Random.Range(0, m_bsm.m_heroes.Count)];
                            }
                        }
                    }
                    // Change or play death animation
                    //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);
                    // Reset heroInput
                    m_bsm.m_battleStates = BattleStateMachine1.PerformAction.CHECKALIVE;
                    m_alive = false;           
                }

                break;
        }
        //
        setDT();
        //
        setBT();
    }

    //
    private void actionTime()
    {
        m_attacking = true;
        // Do damage
        doDamage();
        
        // Remove performer from BSM list
        m_bsm.m_performList.RemoveAt(0);

        // Reset BSM -> Wait
        if (m_bsm.m_battleStates != BattleStateMachine1.PerformAction.WIN &&
            m_bsm.m_battleStates != BattleStateMachine1.PerformAction.LOSE)
        {
            m_bsm.m_battleStates = BattleStateMachine1.PerformAction.WAIT;

            // Reset this enemy state
            //m_attackTime = 0.0f;
            m_currentState = TurnState.PROCESSING;
        }
        else
        {
            m_currentState = TurnState.WAITING;
        }
        // End
        m_currentState = TurnState.PROCESSING;
    }

    //
    void updateTurnBarProgress()
    {
        m_barScale = m_turnBar.transform.localScale;

        if (m_barScale.x < 0.98f)
        {
            //
            m_turnBar.transform.localScale += new Vector3(0.009f, 0.0f, 0.0f);
            //m_turnBar.transform.localScale += new Vector3(m_barSpeed, 0.0f, 0.0f);
            
        }


        //
        else if (m_barScale.x >= 0.98f)
        {
            m_barScale.x = 0;
            m_currentState = TurnState.ADDTOLIST;
        }
    }

    //
    public void manaCost(float cost)
    {
        m_hero.m_currentMP -= cost;

        if (m_hero.m_currentMP <= 0)
        {
            m_hero.m_currentMP = 0;
        }

        updateHeroPanel();
    }

    public void takeDamage(float damage)
    {
        m_hero.m_currentHP -= damage;

        if (m_hero.m_currentHP <= 0)
        {
            m_hero.m_currentHP = 0;
            m_currentState = TurnState.DEAD;
        }

        updateHeroPanel();
    }

    void doDamage()
    {
        
        float damageCal = m_hero.m_currentATK + m_bsm.m_performList[0].m_choosenAttack.m_attackDamage;
        //manaCost(m_bsm.m_performList[0].m_choosenAttack.m_attackCost);

        if (m_bsm.m_performList[0].m_choosenAttack.m_attackName == "Guiding Light")
        {
            if (m_healing == false)
            {
                m_enemyTarget = m_bsm.m_heroes[Random.Range(0, m_bsm.m_heroes.Count)];
                m_healing = true;
            }

            m_enemyTarget.GetComponent<HeroStateMachine>().heal(damageCal);
        }
        else if (m_bsm.m_performList[0].m_choosenAttack.m_attackName == "Blessed Water")
        {
            if (m_healing == false)
            {
                lowestHP();
                m_enemyTarget = lowPlayer;
                Debug.Log(m_enemyTarget);

                m_healing = true;
            }

            m_enemyTarget.GetComponent<HeroStateMachine>().heal(damageCal);
        }
        else
        {
            m_enemyTarget = GameObject.Find("Enemy");
            m_enemyTarget.GetComponent<EnemyStateMachine>().takeDamage(damageCal);
            m_enemyTarget.GetComponent<EnemyStateMachine>().m_hit = true;
        }

        m_turnBar.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }

    //
    void heal(float heal)
    {
        m_hero.m_currentHP += heal / m_hero.m_currentDEF;

        if(m_hero.m_currentHP > m_hero.m_baseHP)
        {
            m_hero.m_currentHP = m_hero.m_baseHP;
        }

        m_healing = false;
        updateHeroPanel();
    }

    // Creates Hero Panel
    void createHeroPanel()
    {
        m_heroPanel = Instantiate(m_heroPanel) as GameObject;
        m_stats = m_heroPanel.GetComponent<HeroPanelStats>();

        m_stats.m_heroName.text = m_hero.m_name;
        m_stats.m_heroHP.text = "HP: " + m_hero.m_currentHP;
        m_stats.m_heroMP.text = "MP: " + m_hero.m_currentMP;

        m_turnBar = m_stats.m_progressBar;
        m_heroPanel.transform.SetParent(m_heroPanelSpacer, false);
        
    }

    // Updates the stats
    void updateHeroPanel()
    {
        m_stats.m_heroHP.text = "HP: " + m_hero.m_currentHP;
        m_stats.m_heroMP.text = "MP: " + m_hero.m_currentMP;
    }

    //
    void lowestHP()
    {
        float lowHP = m_bsm.m_heroes[0].GetComponent<HeroStateMachine>().m_hero.m_currentHP;

        foreach(GameObject a in m_bsm.m_heroes)
        {
            if(a.GetComponent<HeroStateMachine>().m_hero.m_currentHP <= lowHP)
            {
                lowHP = a.GetComponent<HeroStateMachine>().m_hero.m_currentHP;
                Debug.Log(lowHP);
            }
        }

        for(int i = 0; i < m_bsm.m_heroes.Count; i++)
        {
            if(m_bsm.m_heroes[i].GetComponent<HeroStateMachine>().m_hero.m_currentHP == lowHP)
            {
                lowPlayer = m_bsm.m_heroes[i];
                Debug.Log(lowPlayer);
            }
        }
    }

    
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
                this.gameObject.transform.position -= new Vector3(0.1f, 0, 0);
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
    void setDT()
    {
        m_dtTarget.m_dtBluntWeak = m_hero.m_bluntWeak;
        m_dtTarget.m_dtSlashWeak = m_hero.m_slashWeak;
        m_dtTarget.m_dtPierceWeak = m_hero.m_pierceWeak;

        if(m_dtTarget.m_dtBluntWeak == true ||
           m_dtTarget.m_dtSlashWeak == true ||
           m_dtTarget.m_dtPierceWeak == true)
        {
            m_dtTarget.m_physicalWeak = true;
        }

        m_dtTarget.m_dtFireWeak = m_hero.m_fireWeak;
        m_dtTarget.m_dtWindWeak = m_hero.m_windWeak;
        m_dtTarget.m_dtEarthWeak = m_hero.m_earthWeak;
        m_dtTarget.m_dtWaterWeak = m_hero.m_waterWeak;

        if (m_dtTarget.m_dtFireWeak == true ||
            m_dtTarget.m_dtWindWeak == true ||
            m_dtTarget.m_dtEarthWeak == true ||
            m_dtTarget.m_dtWaterWeak == true)
        {
            m_dtTarget.m_magicWeak = true;
        }

        m_dtTarget.m_dtTargetHP = m_hero.m_currentHP;
        m_dtTarget.m_dtTargetMP = m_hero.m_currentMP;

        if(m_dtTarget.m_dtTargetHP < m_hero.m_baseHP)
        {
            m_dtTarget.m_lowHP = true;
        }
        else
        {
            m_dtTarget.m_lowHP = false;
        }

        if (m_dtTarget.m_dtTargetMP < m_hero.m_baseMP)
        {
            m_dtTarget.m_lowMP = true;
        }
        else
        {
            m_dtTarget.m_lowMP = false;
        }

    }
    //
    void setBT()
    {
        m_btTarget.m_btBluntWeak = m_hero.m_bluntWeak;
        m_btTarget.m_btSlashWeak = m_hero.m_slashWeak;
        m_btTarget.m_btPierceWeak = m_hero.m_pierceWeak;
        m_btTarget.m_btFireWeak = m_hero.m_fireWeak;
        m_btTarget.m_btWindWeak = m_hero.m_windWeak;
        m_btTarget.m_btEarthWeak = m_hero.m_earthWeak;
        m_btTarget.m_btWaterWeak = m_hero.m_waterWeak;
          
        m_btTarget.m_btTargetHP = m_hero.m_currentHP;
        m_btTarget.m_btTargetMP = m_hero.m_currentMP;
    }

}
