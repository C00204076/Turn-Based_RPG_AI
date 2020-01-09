using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 13:35 9 January 2020
// Finished at
// Time taken:
// Known bugs:

/*public class ActionNode : MonoBehaviour
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
    public class ActionNode<T> : TreeNode<T>
    {
        private readonly Func<long, T, AivoTreeStatus> _fn;

        public ActionNode(Func<long, T, AivoTreeStatus> fn)
        {
            _fn = fn;
        }

        public AivoTreeStatus Tick(long timeTick, T context)
        {
            return _fn(timeTick, context);
        }
    }
}
