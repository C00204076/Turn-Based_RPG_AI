using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBash : BasicAttack
{
    public ShieldBash()
    {
        m_attackName = "Shield Bash";
        m_attackDescription = "Whack a foe with your shield";
        m_attackDamage = 30f;
        m_attackCost = 25.0f;

        m_blunt = true;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
