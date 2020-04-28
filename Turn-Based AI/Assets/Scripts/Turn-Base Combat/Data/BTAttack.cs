using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BTAttack : MonoBehaviour
{
    public string m_btName;

    public bool m_cantUse = false;

    public bool m_btBlunt;
    public bool m_btSlash;
    public bool m_btPierce;
    public bool m_btFire;
    public bool m_btWind;
    public bool m_btEarth;
    public bool m_btWater;

    public float m_btDamage;
    public float m_btMP;
    public int m_btAttackPriority;
}
