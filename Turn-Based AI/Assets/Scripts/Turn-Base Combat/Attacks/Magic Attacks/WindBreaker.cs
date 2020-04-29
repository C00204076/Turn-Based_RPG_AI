using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBreaker : BasicAttack
{
    public WindBreaker()
    {
        m_attackName = "Wind Breaker";
        m_attackDescription = "Hit a foe with a powerful blast of wind!";
        m_attackDamage = 200f;
        m_attackCost = 100.0f;

        m_blunt = false;
        m_slash = true;
        m_pierce = true;

        m_fire = false;
        m_wind = true;
        m_earth = false;
        m_water = false;
    }
}
