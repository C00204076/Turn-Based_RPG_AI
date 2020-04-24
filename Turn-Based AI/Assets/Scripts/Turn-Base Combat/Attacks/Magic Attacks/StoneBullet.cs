using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneBullet : BasicAttack
{
    public StoneBullet()
    {
        m_attackName = "Stone Bullet";
        m_attackDescription = "Hit a foe with a sharp piece of stone";
        m_attackDamage = 22f;
        m_attackCost = 20.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = true;
        m_water = false;
    }
}
