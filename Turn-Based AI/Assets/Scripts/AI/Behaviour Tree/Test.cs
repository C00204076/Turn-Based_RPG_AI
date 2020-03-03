using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Selector m_rootNode;

    public ActionNode m_equalNodeA;
    public ActionNode m_addOneNodeB;

    public ActionNode m_equalNodeC;
    public ActionNode m_addOneNodeD;

    public Inverter m_inverter;

    public int m_value;
    public int m_targetVal;

    // Start is called before the first frame update
    void Start()
    {
        /*ActionNode action;

        var m_actionOne = new ActionNode(()  => Debug.Log("Speak"));
        var m_actionTwo = new ActionNode(action);
        var m_actionThree = new ActionNode(action);
        var m_actionFour = new ActionNode(action);*/

        m_equalNodeA = new ActionNode(notEqualTo);
        m_addOneNodeB = new ActionNode(addUp);

        m_addOneNodeD = new ActionNode(notEqualTo);

       // m_equalNodeA = new Inverter(m_addOneNodeD);

        List<Node> m_root = new List<Node>();



        m_root.Add(m_equalNodeA);
        m_root.Add(m_addOneNodeB);



        m_rootNode = new Selector(m_root);
    }

    // Update is called once per frame
    void Update()
    {
        m_rootNode.Evaluate();


    }


    private NodeStates notEqualTo()
    {
        

        if (m_value >= m_targetVal)
        {
            Debug.Log("Done Node 1");
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }

    private NodeStates addUp()
    {
        m_value++;
        Debug.Log(m_value.ToString());

        if (m_value == m_targetVal)
        {
            Debug.Log("Done Node 2");
            return NodeStates.SUCCESS;
        }
        else
        {
            return NodeStates.FAILURE;
        }
    }
}
