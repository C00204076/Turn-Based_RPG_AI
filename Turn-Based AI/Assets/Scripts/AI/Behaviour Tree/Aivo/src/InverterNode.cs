/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterNode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}*/
//
// C00204076
// Brandon Seah-Dempsey
// Started at 13:37 9 January 2020
// Finished at
// Time taken:
// Known bugs:


namespace AivoTree
{
    public class InverterNode<T> : TreeNode<T>
    {
        private readonly TreeNode<T> node;

        public InverterNode(TreeNode<T> node)
        {
            this.node = node;
        }

        public AivoTreeStatus Tick(long timeTick, T context)
        {
            var status = node.Tick(timeTick, context);
            switch (status)
            {
                case AivoTreeStatus.Success:
                    return AivoTreeStatus.Failure;
                case AivoTreeStatus.Failure:
                    return AivoTreeStatus.Success;
                default:
                    return status;
            }
        }
    }
}