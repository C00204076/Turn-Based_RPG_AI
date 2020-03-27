using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BasicAttack
{
    public Slash()
    {
        m_attackName = "Slash";
        m_attackDescription = "Swipe at a foe with a sharp weapon.";
        m_attackDamage = 12f;
        m_attackCost = 0.0f;

        m_blunt = false;
        m_slash = true;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
