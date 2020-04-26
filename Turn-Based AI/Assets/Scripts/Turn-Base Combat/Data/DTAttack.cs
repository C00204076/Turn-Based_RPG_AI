using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DTAttack : MonoBehaviour
{
    public string m_dtName;

    public bool m_dtBlunt;
    public bool m_dtSlash;
    public bool m_dtPierce;
    public bool m_dtFire;
    public bool m_dtWind;
    public bool m_dtEarth;
    public bool m_dtWater;

    public float m_dtDamage;
    public float m_dtMP;
    public int m_dtAttackPriority;
}
