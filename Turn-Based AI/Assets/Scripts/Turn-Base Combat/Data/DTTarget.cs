using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DTTarget : MonoBehaviour
{
    public string m_dtName;

    public bool m_dtCanHeal = false;
    public bool m_lowHP = false;
    public bool m_lowMP = false;

    public bool m_physicalWeak;
    public bool m_dtBluntWeak;
    public bool m_dtSlashWeak;
    public bool m_dtPierceWeak;

    public bool m_magicWeak;
    public bool m_dtFireWeak;
    public bool m_dtWindWeak;
    public bool m_dtEarthWeak;
    public bool m_dtWaterWeak;

    public float m_dtTargetHP;
    public float m_dtTargetMaxHP;
    public float m_dtTargetMP;
    public float m_dtTargetMaxMP;
    public int m_dtTargetPriority;
}
