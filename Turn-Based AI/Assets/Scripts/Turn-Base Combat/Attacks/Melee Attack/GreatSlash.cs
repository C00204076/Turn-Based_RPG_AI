using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSlash : BasicAttack
{
    public GreatSlash()
    {
        m_attackName = "Great Slash";
        m_attackDescription = "Strike a foe with a might sword slash!";
        m_attackDamage = 150f;
        m_attackCost = 100.0f;

        m_blunt = false;
        m_slash = true;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
