using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLance : BasicAttack
{
    public FlameLance()
    {
        m_attackName = "Flame Lance";
        m_attackDescription = "Pierce a foe with a spear of fire";
        m_attackDamage = 130f;
        m_attackCost = 75.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = true;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
