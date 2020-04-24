using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlazingSmite : BasicAttack
{
    public BlazingSmite()
    {
        m_attackName = "Blazing Smite";
        m_attackDescription = "Slash a foe with a blazing sword";
        m_attackDamage = 120f;
        m_attackCost = 80.0f;

        m_blunt = false;
        m_slash = true;
        m_pierce = false;

        m_fire = true;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
