using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : BasicAttack
{
   public Bash()
    {
        m_attackName = "Bash";
        m_attackDescription = "Hit a foe with a blunt weapon";
        m_attackDamage = 3f;
        m_attackCost = 0.0f;

        m_blunt = true;
        m_slash = false;
        m_pierce = false;

        m_fire = false;
        m_wind = false;
        m_earth = false;
        m_water = false;
    }
}
