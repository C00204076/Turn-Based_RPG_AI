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


    // Start is called before the first frame update
    void Start()
    {
        //
        m_barScale = new Vector3();
        //
        m_turnBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
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

                break;
            case TurnState.WAITING:

                break;
            case TurnState.SELECTING:

                break;
            case TurnState.ACTION:

                break;
            case TurnState.DEAD:

                break;
        }
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
}
