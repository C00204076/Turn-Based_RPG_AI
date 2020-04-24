using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caltrops : BasicAttack
{
    public Caltrops()
    {
        m_attackName = "Caltrops";
        m_attackDescription = "Drop caltrops at a foe's feet!";
        m_attackDamage = 10f;
        m_attackCost = 5.0f;

        m_blunt = false;
        m_slash = false;
        m_pierce = true;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
