using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessedWater : BasicAttack
{
    public BlessedWater()
    {
        m_attackName = "Blessed Water";
        m_attackDescription = "Heal an ally with the lowest health!";
        m_attackDamage = 180f;
        m_attackCost = 80.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
