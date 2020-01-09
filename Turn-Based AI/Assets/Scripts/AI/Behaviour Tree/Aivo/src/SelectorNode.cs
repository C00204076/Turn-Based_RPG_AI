﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 13:40 9 January 2020
// Finished at
// Time taken:
// Known bugs:

/*public class SelectorNode : MonoBehaviour
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
    public class SelectorNode<T> : TreeNode<T>
    {
        private readonly TreeNode<T>[] _nodes;
        private TreeNode<T> runningNode;

        public SelectorNode(params TreeNode<T>[] nodes)
        {
            _nodes = nodes;
        }

        public AivoTreeStatus Tick(long timeTick, T context)
        {
            var nodesToSearch = runningNode == null
                ? _nodes
                : _nodes.SkipWhile(node => node != runningNode);
            return nodesToSearch.Aggregate(AivoTreeStatus.Failure, (acc, curr) =>
            {
                if (acc != AivoTreeStatus.Success)
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