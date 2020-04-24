using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackStab : BasicAttack
{
    public BackStab()
    {
        m_attackName = "Back Stab";
        m_attackDescription = "Strike a foe with a sharp stab from behind!";
        m_attackDamage = 35f;
        m_attackCost = 15.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
