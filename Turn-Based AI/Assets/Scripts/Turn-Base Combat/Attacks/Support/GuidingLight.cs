using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidingLight : BasicAttack
{
    public GuidingLight()
    {
        m_attackName = "Guiding Light";
        m_attackDescription = "Heal a random ally!";
        m_attackDamage = 250f;
        m_attackCost = 50.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
