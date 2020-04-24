using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBomb : BasicAttack
{
    public PoisonBomb()
    {
        m_attackName = "Poison Bomb";
        m_attackDescription = "Lob as a flask of poison gas at a foe!";
        m_attackDamage = 16f;
        m_attackCost = 15.0f;

        m_blunt = true;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = true;
        m_water = false;
    }
}
