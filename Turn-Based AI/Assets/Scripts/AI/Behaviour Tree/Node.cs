using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base Node clss used for Behaviour Tree nodes
// C00204076
// Brandon Seah-Dempsey
// Started at 10:20 12 December 2019
// Finished at
// Time taken:
// Known bugs:

// Make Node public
/*[System.Serializable]
// Base Node class
public abstract class Node : MonoBehaviour
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

// Extends Nodes to selectors
public class Selector : Node
{
    // The child nodes for this selector 
    protected List m_nodes = new List();


    // The constructor requires a lsit of child nodes to be  passed in
    public Selector(List nodes)
    {
        m_nodes = nodes;
    }

    // If any of the children reports a success, the selector will 
    // immediately report a success upwards. If all children fail, 
    // it will report a failure instead.
    public override NodeStates Evaluate()
    {
        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    continue;

                case NodeStates.SUCCESS:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;

                case NodeStates.RUNNING:
                    m_nodeState = NodeStates.RUNNING;
                    return m_nodeState;

                default:
                    continue;
            }
        }
        m_nodeState = NodeStates.FAILURE;
        return m_nodeState;
    }
}

// Node Sequences
public class Sequence : Node
{
    // Children nodes that belong to this sequence
    private List m_nodes = new List();

    // Must provide an initial set of children nodes to work
    public Sequence(List nodes)
    {
        m_nodes = nodes;
    }

    // If any child node returns a failure, the entire node fails. Whence all  
    // nodes return a success, the node reports a success.
    public override NodeStates Evaluate()
    {
        bool anyChildRunning = false;

        foreach (Node node in m_nodes)
        {
            switch (node.Evaluate())
            {
                case NodeStates.FAILURE:
                    m_nodeState = NodeStates.FAILURE;
                    return m_nodeState;

                case NodeStates.SUCCESS:
                    continue;

                case NodeStates.RUNNING:
                    anyChildRunning = true;
                    continue;

                default:
                    m_nodeState = NodeStates.SUCCESS;
                    return m_nodeState;
            }
        }

        m_nodeState = anyChildRunning ? NodeStates.RUNNING : NodeStates.SUCCESS;
        return m_nodeState;
    }
}

// Acts as inverter in the form of a decorator
public class Inverter : Node
{
    // Child node to evaluate 
    private Node m_node;

    public Node node
    {
        get { return m_node; }
    }

    // The constructor requires the child node that this inverter decorator 
    // wraps
    public Inverter(Node node)
    {
        m_node = node;
    }

    // Reports a success if the child fails and 
    // a failure if the child succeeds. Running will report 
    // as running 
    public override NodeStates Evaluate()
    {
        switch (m_node.Evaluate())
        {
            case NodeStates.FAILURE:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;

            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;

            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;
        }

        m_nodeState = NodeStates.SUCCESS;
        return m_nodeState;
    }
}

// Generic Action Node
public class ActionNode : Node
{
    // Method signature for the action. 
    public delegate NodeStates ActionNodeDelegate();

    // The delegate that is called to evaluate this node 
    private ActionNodeDelegate m_action;

    // Because this node contains no logic itself, 
    // the logic must be passed in in the form of  
    // a delegate. As the signature states, the action 
    // needs to return a NodeStates enum 
    public ActionNode(ActionNodeDelegate action)
    {
        m_action = action;
    }

    // Evaluates the node using the passed in delegate and  
    // reports the resulting state as appropriate 
    public override NodeStates Evaluate()
    {
        switch (m_action())
        {
            case NodeStates.SUCCESS:
                m_nodeState = NodeStates.SUCCESS;
                return m_nodeState;

            case NodeStates.FAILURE:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;

            case NodeStates.RUNNING:
                m_nodeState = NodeStates.RUNNING;
                return m_nodeState;

            default:
                m_nodeState = NodeStates.FAILURE;
                return m_nodeState;
        }
    }
}*/