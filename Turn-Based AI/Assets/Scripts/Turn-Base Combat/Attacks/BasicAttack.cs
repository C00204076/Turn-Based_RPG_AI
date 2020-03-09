using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAttack
{
    public string m_attackName; // Name
    public string m_attackDescription; //
    public float m_attackDamage; // Base Damage 15, melee, lv 10, stamina 35 = basedmg + stamina + lvl = 60
    public float m_attakcCost; //Mana cost
}
