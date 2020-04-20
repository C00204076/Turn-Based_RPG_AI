using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BattleStateMachine1 : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public PerformAction m_battleStates;

    public List<HandleTurn> m_performList = new List<HandleTurn>();
    public List<HandleTurn> m_performHero = new List<HandleTurn>();
    public List<HandleTurn> m_performEnemy = new List<HandleTurn>();

    //
    public List<GameObject> m_heroes = new List<GameObject>();
    public List<GameObject> m_enemies = new List<GameObject>();

    //
    public enum AIState
    {
        BT,
        DT
    }

    //
    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    //
    private ActionButtonStats m_stats;
    private ActionButtonStats m_statsM;
    //
    public HeroGUI m_heroInput;
    public AIState m_aiState;
    //
    public List<GameObject> m_heroToManage = new List<GameObject>();
    private HandleTurn m_herosChoice;
    //
    public GameObject m_actionPanel;
    public GameObject m_actionSpacer;

    public GameObject m_actionButton;
    public GameObject m_magicButton;
    private List<GameObject> m_atkBtns = new List<GameObject>();
    //
    public GameObject m_magicPanel;
    public GameObject m_magicSpacer;
    public GameObject m_skillButton;

    //
    //public List<GameObject> m_attackQueue = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_battleStates = PerformAction.WAIT;
        m_heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        m_enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        m_heroInput = HeroGUI.ACTIVATE;
        m_aiState = AIState.DT;

        m_actionPanel.SetActive(false);
        m_magicPanel.SetActive(false);

        //m_attackQueue = m_heroToManage;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_battleStates)
        {
            case (PerformAction.WAIT):
                //
                if(m_performList.Count > 0)
                {
                    m_battleStates = PerformAction.TAKEACTION;
                }

                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(m_performList[0].Attacker);
                //
                if(m_performList[0].Type == "Enemy")
                {
                    EnemyStateMachine esm = performer.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i < m_heroes.Count; i++)
                    {
                        if (m_performList[0].Target == m_heroes[i])
                        {
                            esm.m_heroTarget = m_performList[0].Target;
                            esm.m_currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }

                        else 
                        {
                            int j = Random.Range(0, m_heroes.Count);
                            m_performList[0].Target = m_heroes[j];
                            esm.m_heroTarget = m_performList[0].Target;
                            esm.m_currentState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                }

                //
                if (m_performList[0].Type == "Hero")
                {
                    HeroStateMachine hsm = performer.GetComponent<HeroStateMachine>();
                    hsm.m_enemyTarget = m_performList[0].Target;
                    hsm.m_currentState = HeroStateMachine.TurnState.ACTION;
                }

                m_battleStates = PerformAction.PERFORMACTION;

                break;
            case (PerformAction.PERFORMACTION):
                // Idle
                break;
            case (PerformAction.CHECKALIVE):
                // Lose battle
                if (m_heroToManage.Count < 1)
                {
                    m_battleStates = PerformAction.LOSE;
                }
                // Win battle
                else if (m_enemies.Count < 1)
                {
                    m_battleStates = PerformAction.WIN;
                }

                else 
                {
                    clearAttackPanel();
                    m_heroInput = HeroGUI.ACTIVATE;
                }
                break;
            case (PerformAction.WIN):
                Debug.Log("You lose!");
                break;
            case (PerformAction.LOSE):
                Debug.Log("You Win!");
                break;
        }

        switch(m_heroInput)
        {
            case (HeroGUI.ACTIVATE):
                if(m_heroToManage.Count > 0)
                {
                    m_herosChoice = new HandleTurn();

                    m_actionPanel.SetActive(true);
                    createAttackButtons();

                    m_heroInput = HeroGUI.WAITING;
                }
                break;
            case (HeroGUI.WAITING):

                break;
            case (HeroGUI.DONE):
                heroInputDone();

                for(int i = 0; i < m_heroes.Count; i++)
                {
                    m_heroes[i].GetComponent<HeroStateMachine>().m_currentState = HeroStateMachine.TurnState.WAITING;
                }
                break;
        }

        //Debug.Log(m_battleStates);
    }

    //
    public void collectActions(HandleTurn input)
    {
        m_performList.Add(input);
    }

    //
    /*void enemyButtons()
    {
        foreach(GameObject enemy in m_enemies)
        {
            GameObject newBtn = Instantiate(m_enemyButton) as GameObject;
            EnemySelectButton btn = newBtn.GetComponent<EnemySelectButton>();

            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            //Text btnText = newBtn.transform.Find("Text").gameObject.GetComponent<Text>();
            //btnText.text = currentEnemy.m_enemy.name;

            //btn.EnemyPrefab = enemy;

            newBtn.transform.SetParent(m_spacer);
        }
    }*/

    //
    public void input1()
    {
        
        m_herosChoice.Attacker = m_heroToManage[0].name;
        m_herosChoice.AttackingObject = m_heroToManage[0];
        m_herosChoice.Type = "Hero";

        m_herosChoice.m_choosenAttack = m_heroToManage[0].GetComponent<HeroStateMachine>().m_hero.m_normalAttack;


        m_actionPanel.SetActive(false);
        m_heroToManage[0].GetComponent<HeroStateMachine>().changeAction();
        m_heroes[0].GetComponent<HeroStateMachine>().m_currentState = HeroStateMachine.TurnState.ACTION;
    }

    //
    public void input2()
    {
        m_herosChoice.Target = m_enemies[0];
        m_heroInput = HeroGUI.DONE;
    }

    //
    void heroInputDone()
    {
        m_performList.Add(m_herosChoice);

        clearAttackPanel();

        m_heroToManage.RemoveAt(0);
        m_heroInput = HeroGUI.ACTIVATE;
    }

    void clearAttackPanel()
    {
        m_actionPanel.SetActive(false);
        m_magicPanel.SetActive(false);
        foreach (GameObject atkBtn in m_atkBtns)
        {
            Destroy(atkBtn);
        }
        m_atkBtns.Clear();
    }

    void createAttackButtons()
    {
        if (m_actionPanel.activeSelf == true)
        {
            //
            m_actionButton = Instantiate(m_actionButton) as GameObject;
            m_stats = m_actionButton.GetComponent<ActionButtonStats>();

            m_stats.m_heroName.text = "Attack";
            m_actionButton.GetComponent<Button>().onClick.AddListener(() => input1());

            m_actionButton.transform.SetParent(m_actionSpacer.transform, false);
            m_atkBtns.Add(m_actionButton);
            //
            m_magicButton = Instantiate(m_magicButton) as GameObject;
            m_stats = m_magicButton.GetComponent<ActionButtonStats>();

            m_stats.m_heroName.text = "Magic";
            m_magicButton.GetComponent<Button>().onClick.AddListener(() => input3());

            m_magicButton.transform.SetParent(m_actionSpacer.transform, false);
            m_atkBtns.Add(m_magicButton);
        }
    }

    void createSkillButtons()
    {
        if (m_heroToManage[0].GetComponent<HeroStateMachine>().m_hero.m_attacks.Count > 0)
        {
            foreach (BasicAttack attack in m_heroToManage[0].GetComponent<HeroStateMachine>().m_hero.m_attacks)
            {
                m_skillButton = Instantiate(m_skillButton) as GameObject;
                m_statsM = m_skillButton.GetComponent<ActionButtonStats>();

                Debug.Log(attack.m_attackName);
                m_statsM.m_heroName.text = "" + attack.m_attackName;
                SkillButton skb = m_skillButton.GetComponent<SkillButton>();

                skb.m_skillAttackToPerform = attack;
                m_skillButton.GetComponent<Button>().onClick.AddListener(() => input4(skb.m_skillAttackToPerform));

                m_skillButton.transform.SetParent(m_magicSpacer.transform, false);
                m_atkBtns.Add(m_skillButton);
            }
        }
        else
        {
            m_magicButton.GetComponent<Button>().interactable = false;
        }
    }

    // Switching to Magic attacks
    public void input3()
    {
        m_actionPanel.SetActive(false);
        m_magicPanel.SetActive(true);
        createSkillButtons();
    }

    //Choosen skill attack
    public void input4(BasicAttack choosenSkill)
    {
        m_herosChoice.Attacker = m_heroToManage[0].name;
        m_herosChoice.AttackingObject = m_heroToManage[0];
        m_herosChoice.Type = "Hero";

        m_herosChoice.m_choosenAttack = choosenSkill;
        m_magicPanel.SetActive(false);
        m_heroToManage[0].GetComponent<HeroStateMachine>().changeAction();
        m_heroes[0].GetComponent<HeroStateMachine>().m_currentState = HeroStateMachine.TurnState.ACTION;
    }


    /*public void adjustHeroManage(GameObject oldHero)
    {
        //Debug.Log(m_q[0]);

        //item = m_q[0];
        //m_q.RemoveAt(0);
        //m_q.Add(item);
        //GameObject hero = oldHero;
        //m_attackQueue.RemoveAt(0);
        //m_attackQueue.Add(hero);
        //m_heroes.RemoveAt(0);
        //m_heroes.Add(hero);
    }*/
     
}
