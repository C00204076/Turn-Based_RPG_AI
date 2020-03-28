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
    private GameObject m_heroObject;
    //
    private HandleTurn attack;
    //
    private Vector3 m_startPos;
    //actionTime
    private bool m_startedAct = false;
    public GameObject m_enemyTarget;

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

    public Image m_turnBar;

    private Vector3 m_barScale;

    private bool m_alive = true;


    // Start is called before the first frame update
    void Start()
    {
        //
        m_barScale = new Vector3();
        //
        m_turnBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        //
        m_bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>();
        m_heroObject = GameObject.Find("Hero");
        m_startPos = transform.position;
        //
        m_currentState = TurnState.PROCESSING;
    }

    // Update is called once per frame
    void Update()
    {
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
            case TurnState.SELECTING:

                break;
            case TurnState.ACTION:
                //StartCoroutine(actionTime());
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
                    for (int i = 0; i < m_bsm.m_performList.Count; i++)
                    {
                        if (m_bsm.m_performList[i].AttackingObject == this.gameObject)
                        {
                            m_bsm.m_performList.Remove(m_bsm.m_performList[i]);
                        }
                    }
                    // Change or play death animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);
                    // Reset heroInput
                    m_bsm.m_heroInput = BattleStateMachine1.HeroGUI.ACTIVATE;
                    m_alive = false;           
                }

                break;
        }
    }

    //
    private IEnumerator actionTime()
    {
        // Break IEnumerator if action has already started
        if (m_startedAct)
        {
            yield break;
        }

        m_startedAct = true;

        // Animate enemy attacking hero, when near
        Vector2 heroPos = new Vector2(m_enemyTarget.transform.position.x,
                                      m_enemyTarget.transform.position.y);
        // Return null if true
        /*while (moveTowardsHero(heroPos))
        {
            yield return null;
        }*/

        // Wait
        yield return new WaitForSeconds(0.5f);
        // Do damage

        // Animate back to sart position
        Vector3 firstPos = m_startPos;
        /*while (moveTowardsStart(firstPos))
        {
            yield return null;
        }*/

        // Remove performer from BSM list
        m_bsm.m_performList.RemoveAt(0);
        // Reset BSM -> Wait
        m_bsm.m_battleStates = BattleStateMachine1.PerformAction.WAIT;
        // End of Coroutine
        m_startedAct = false;
        // Reset this enemy state
        //m_attackTime = 0.0f;
        m_currentState = TurnState.PROCESSING;
    }

    //
    void updateTurnBarProgress()
    {
        m_barScale = m_turnBar.transform.localScale;

        if (m_barScale.x < 1.0f)
        {
            //
            m_turnBar.transform.localScale += new Vector3(0.009f, 0.0f, 0.0f);
        }
        

        //
        else
        {
            m_currentState = TurnState.ADDTOLIST;
        }
    }

    public void takeDamage(float damage)
    {
        m_hero.m_currentHP -= damage;

        if (m_hero.m_currentHP <= 0)
        {
            m_currentState = TurnState.DEAD;
        }
    }
}
