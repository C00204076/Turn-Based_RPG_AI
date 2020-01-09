using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 13:45 9 January 2020
// Finished at
// Time taken:
// Known bugs:

/*public class SequenceNode : MonoBehaviour
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

namespace AivoTree
{
    public class SequenceNode<T> : TreeNode<T>
    {
        private readonly TreeNode<T>[] _nodes;
        private TreeNode<T> runningNode;

        public SequenceNode(params TreeNode<T>[] nodes)
        {
            _nodes = nodes;
        }

        public AivoTreeStatus Tick(long timeTick, T context)
        {
            var nodesToSearch = runningNode == null
                ? _nodes
                : _nodes.SkipWhile(node => node != runningNode);
            return nodesToSearch.Aggregate(AivoTreeStatus.Success, (acc, curr) =>
            {
                if (acc == AivoTreeStatus.Success)
                {
                    var result = curr.Tick(timeTick, context);
                    if (result == AivoTreeStatus.Running)
                    {
                        runningNode = curr;
                    }
                    else
                    {
                        runningNode = null;
                    }
                    return result;
                }
                return acc;
            });
        }
    }
}