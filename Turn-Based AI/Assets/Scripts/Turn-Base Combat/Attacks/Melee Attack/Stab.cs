using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stab : BasicAttack
{
    public Stab()
    {
        m_attackName = "Stab";
        m_attackDescription = "Lunge toward a foe with a sharp weapon.";
        m_attackDamage = 6f;
        m_attackCost = 0.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
