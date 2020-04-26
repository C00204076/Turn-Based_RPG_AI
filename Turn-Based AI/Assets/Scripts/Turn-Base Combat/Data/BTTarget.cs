using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BTTarget : MonoBehaviour
{
    public string m_btName;

    public bool m_btCanHeal;
    public bool m_btBluntWeak;
    public bool m_btSlashWeak;
    public bool m_btPierceWeak;
    public bool m_btFireWeak;
    public bool m_btWindWeak;
    public bool m_btEarthWeak;
    public bool m_btWaterWeak;

    public float m_btTargetHP;
    public float m_btTargetMP;
    public int m_btTargetPriority;
}
