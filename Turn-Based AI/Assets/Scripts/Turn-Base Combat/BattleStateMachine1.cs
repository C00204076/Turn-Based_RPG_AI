using System.Collections;
using System.Collections.Generic;
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
    public HeroGUI m_heroInput;
    //
    public List<GameObject> m_heroToManage = new List<GameObject>();
    private HandleTurn m_herosChoice;


    // Start is called before the first frame update
    void Start()
    {
        m_battleStates = PerformAction.WAIT;
        m_heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        m_enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
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
                    esm.m_heroTarget = m_performList[0].Target;
                    esm.m_currentState = EnemyStateMachine.TurnState.ACTION;
                }

                //
                if (m_performList[0].Type == "Hero")
                {

                }

                m_battleStates = PerformAction.PERFORMACTION;

                break;
            case (PerformAction.PERFORMACTION):


                break;
        }
    }

    //
    public void collectActions(HandleTurn input)
    {
        m_performList.Add(input);
    }
}
