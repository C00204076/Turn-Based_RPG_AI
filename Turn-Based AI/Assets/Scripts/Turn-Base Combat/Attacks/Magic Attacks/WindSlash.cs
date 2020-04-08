using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSlash : BasicAttack
{
    public WindSlash()
    {
        m_attackName = "Wind Slash";
        m_attackDescription = "Hit a foe with a blade of wind";
        m_attackDamage = 20f;
        m_attackCost = 16.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = true;
        m_earth = false;
        m_water = false;
    }
}
