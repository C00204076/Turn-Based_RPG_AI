using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFist : BasicAttack
{
    public StoneFist()
    {
        m_attackName = "Stone Fist";
        m_attackDescription = "Hit a foe with a massive boulder";
        m_attackDamage = 200f;
        m_attackCost = 90.0f;

        m_blunt = true;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = true;
        m_water = false;
    }
}
