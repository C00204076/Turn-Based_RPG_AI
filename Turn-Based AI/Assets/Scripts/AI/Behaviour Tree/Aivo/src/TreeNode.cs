/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode : MonoBehaviour
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
// Started at 13:33 9 January 2020
// Finished at
// Time taken:
// Known bugs:

namespace AivoTree
{
    public interface TreeNode<T>
    {
        AivoTreeStatus Tick(long timeTick, T context);
    }
}