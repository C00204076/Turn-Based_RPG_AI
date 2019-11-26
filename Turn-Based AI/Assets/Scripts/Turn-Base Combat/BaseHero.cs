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

    // Stats/Attributes
    public int m_strength; // Physical power
    public int m_intelligence; // Magic power
    public int m_dexterity; // Balance and critical rate
    public int m_constitution; // HP and defense
    public int m_agility; // Speed and dodge

   /* // Typing
    // Physical Damage
    // Immune
    public bool m_bluntImmune;
    public bool m_slashImmune;
    public bool m_pierceImmune;
    // Resist
    public bool m_bluntResist;
    public bool m_slashResist;
    public bool m_pierceResist;

    // Magic Damage
    // Immune
    public bool m_fireImmune;
    public bool m_windImmune;
    public bool m_earthImmune;
    public bool m_waterImmune;
    // Resist
    public bool m_fireResist;
    public bool m_windResist;
    public bool m_earthResist;
    public bool m_waterResist;*/

    
}
