using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : BasicAttack
{
    public WaterBall()
    {
        m_attackName = "Water Ball";
        m_attackDescription = "Hit a foe with a ball of water";
        m_attackDamage = 15f;
        m_attackCost = 11.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = true;
    }
}
