using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 16:00 14 November 2019
// Finished at
// Time taken:
// Known bugs:

// Make BaseHero public
[System.Serializable]
public class BaseHero : MonoBehaviour
{
    public string m_name;

    public float m_baseHP;
    public float m_currentHP;

    public float m_baseMP;
    public float m_currentMP;

    public float m_baseATK;
    public float m_currentATK;

    public float m_baseDEF;
    public float m_currentDEF;

    // Stats/Attributes
    public float m_strength; // Physical power
    public float m_intelligence; // Magic power
    public float m_dexterity; // Balance and critical rate
    public float m_constitution; // HP and defense
    public float m_agility; // Speed and dodge

     // Typing
     // Physical Damage
     // Weakness
     public bool m_bluntWeak;
     public bool m_slashWeak;
     public bool m_pierceWeak;

     // Magic Damage
     // Weakness
     public bool m_fireWeak;
     public bool m_windWeak;
     public bool m_earthWeak;
     public bool m_waterWeak;

    public List<BasicAttack> m_attacks = new List<BasicAttack>();

    public BasicAttack m_normalAttack;
}
