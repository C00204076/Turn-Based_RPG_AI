using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DTTarget : MonoBehaviour
{
    public string m_dtName;

    public bool m_dtCanHeal;

    public bool m_dtBluntWeak;
    public bool m_dtSlashWeak;
    public bool m_dtPierceWeak;
    public bool m_dtFireWeak;
    public bool m_dtWindWeak;
    public bool m_dtEarthWeak;
    public bool m_dtWaterWeak;

    public float m_dtTargetHP;
    public float m_dtTargetMP;
    public int m_dtTargetPriority;
}
