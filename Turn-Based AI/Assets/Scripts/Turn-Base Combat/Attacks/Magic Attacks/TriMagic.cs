using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriMagic : BasicAttack
{
    public TriMagic()
    {
        m_attackName = "TriMagic";
        m_attackDescription = "Hit a foe with a three different magic attacks!";
        m_attackDamage = 300f;
        m_attackCost = 325.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = true;
        m_wind = true;
        m_earth = true;
        m_water = true;
    }
}
