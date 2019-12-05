using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 
// C00204076
// Brandon Seah-Dempsey
// Started at 10:25 5 December 2019
// Finished at
// Time taken:
// Known bugs:

public class EnemySelectButton : MonoBehaviour
{
    public GameObject m_EnemyPrefab;

    public void selectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>(); // Save input
    }
}
