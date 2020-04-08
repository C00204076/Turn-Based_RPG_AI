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
        PERFORMACTION
    }

    public PerformAction m_battleStates;

    public List<HandleTurn> m_performList = new List<HandleTurn>();

    //
    public List<GameObject> m_heroes = new List<GameObject>();
    public List<GameObject> m_enemies = new List<GameObject>();

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
    //
    public List<GameObject> m_heroToManage = new List<GameObject>();
    private HandleTurn m_herosChoice;
    //
    public GameObject m_actionPanel;
    public GameObject m_actionSpacer;
    //
    public GameObject m_magicPanel;
    public GameObject m_magicSpacer;

    public GameObject m_actionButton;
    public GameObject m_magicButton;
    private List<GameObject> m_atkBtns = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_battleStates = PerformAction.WAIT;
        m_heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        m_enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        

        //enemyButtons();
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


                break;
        }

        switch(m_heroInput)
        {
            case (HeroGUI.ACTIVATE):
                if(m_heroToManage.Count > 0)
                {
                    m_herosChoice = new HandleTurn();

                    createAttackButtons();

                    m_heroInput = HeroGUI.WAITING;
                }
                break;
            case (HeroGUI.WAITING):

                break;
            case (HeroGUI.DONE):
                heroInputDone();
                break;
        }
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

        foreach (GameObject atkBtn in m_atkBtns)
        {
            Destroy(atkBtn);
        }
        m_atkBtns.Clear();

        m_heroToManage.RemoveAt(0);
        m_heroInput = HeroGUI.ACTIVATE;
    }

    void createAttackButtons()
    {
        m_actionButton = Instantiate(m_actionButton) as GameObject;
        m_stats = m_actionButton.GetComponent<ActionButtonStats>();

        m_stats.m_heroName.text = "Attack";
        m_actionButton.GetComponent<Button>().onClick.AddListener( () => input1());

        m_actionButton.transform.SetParent(m_actionSpacer.transform, false);
        m_atkBtns.Add(m_actionButton);

        m_magicButton = Instantiate(m_magicButton) as GameObject;
        m_stats = m_magicButton.GetComponent<ActionButtonStats>();

        m_stats.m_heroName.text = "Magic";
        //m_magicButton.GetComponent<Button>().onClick.AddListener(() => input1());

        m_magicButton.transform.SetParent(m_actionSpacer.transform, false);
        m_atkBtns.Add(m_magicButton);
    }
}
