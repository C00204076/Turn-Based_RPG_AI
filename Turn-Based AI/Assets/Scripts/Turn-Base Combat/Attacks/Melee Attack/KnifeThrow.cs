using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeThrow : BasicAttack
{
    public KnifeThrow()
    {
        m_attackName = "Knife Throw";
        m_attackDescription = "Throw a sharp knife at a foe!";
        m_attackDamage = 15f;
        m_attackCost = 7.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
