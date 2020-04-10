using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour
{
    public BasicAttack m_skillAttackToPerform;

    public void useSkillAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine1>().input4(m_skillAttackToPerform);
    }
}
