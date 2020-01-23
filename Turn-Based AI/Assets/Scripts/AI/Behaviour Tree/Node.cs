using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Node class used for Behaviour Tree nodes
// C00204076
// Brandon Seah-Dempsey
// Started at 10:20 12 December 2019
// Finished at
// Time taken:
// Known bugs:

// Make Node public
[System.Serializable]

public enum NodeStates
{
    FAILURE,
    SUCCESS,
    RUNNING
}


// Base Node class
public abstract class Node // : MonoBehaviour
{
    // Delegate that returns the state of the node.
    public delegate NodeStates NodeReturn();

    // The current state of the node 
    protected NodeStates m_nodeState;

    public NodeStates nodeState
    {
        get { return m_nodeState; }
    }

    // The constructor for the node 
    public Node()
    {

    }

    // Implementing classes use this method to evaluate the desired set of conditions
    public abstract NodeStates Evaluate();
}

/*public class Model : Node
{

}

public class Test
{
    /*
     public class Example
    {
        public static TreeNode<BotContext> createCounterAttackBehaviour()
        {
            var enoughManaToDeployUnit = new ActionNode<BotContext>((timeTick, ctx) => AivoTreeStatus.Success);
            var lessUnitsThanOpponent = new ActionNode<BotContext>((timeTick, ctx) => AivoTreeStatus.Success);
            var bestTargetToAttack = new ActionNode<BotContext>((timeTick, ctx) => AivoTreeStatus.Success);
            var bestUnitToAttack = new ActionNode<BotContext>((timeTick, ctx) => AivoTreeStatus.Success);
            var deploySelectedUnit = new ActionNode<BotContext>((timeTick, ctx) => AivoTreeStatus.Success);

            return new SelectorNode<BotContext>(
                new SequenceNode<BotContext>(enoughManaToDeployUnit, lessUnitsThanOpponent, bestTargetToAttack,
                    bestUnitToAttack, deploySelectedUnit));
        }
    }


    public class BotContext { }
     
}*/