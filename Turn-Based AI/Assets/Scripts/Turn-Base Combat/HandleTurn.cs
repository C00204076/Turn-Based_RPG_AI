using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 15:32 25 November 2019
// Finished at
// Time taken:
// Known bugs:

// Make HandleTurn public
[System.Serializable]
public class HandleTurn
{
    public string Attacker; // Attacker's name
    public string Type;
    public GameObject AttackingObject; // Who's attacking
    public GameObject Target; // Attacker's target 

    // Which attack is being performed
    public BasicAttack m_choosenAttack;
}
