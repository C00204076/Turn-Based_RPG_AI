using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAttack : MonoBehaviour
{
    public string m_attackName; // Name
    public string m_attackDescription; //
    public float m_attackDamage; // Base Damage 15, melee, lv 10, stamina 35 = basedmg + stamina + lvl = 60
    public float m_attackCost; //Mana cost

    // Typing
    // Physical Damage
    public bool m_blunt;
    public bool m_slash;
    public bool m_pierce;

    // Magic Damage
    public bool m_fire;
    public bool m_wind;
    public bool m_earth;
    public bool m_water;
    //
    public DTAttack m_dtAttack;
    public BTAttack m_btAttack;
}
