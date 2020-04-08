using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : BasicAttack
{
    public FireBall()
    {
        m_attackName = "Fire Ball";
        m_attackDescription = "Hit a foe with a ball of fire";
        m_attackDamage = 18f;
        m_attackCost = 14.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = false;

        m_fire = true;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
