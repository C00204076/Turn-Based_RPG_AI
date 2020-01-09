using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//
// C00204076
// Brandon Seah-Dempsey
// Started at 13:46 9 January 2020
// Finished at
// Time taken:
// Known bugs:


/*public class NewBehaviourScript : MonoBehaviour
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
    public class Weighted
    {
        public static int RandomIndexByWeight(int[] weights, Func<int, int, int> randomBetween)
        {
            var sumOfWeights = weights.Aggregate((i, acc) => i + acc);
            var picked = randomBetween(0, sumOfWeights - 1);
            var indexDistribution = weights.SelectMany((weight, index) => Enumerable.Repeat(index, weight)).ToArray();
            return indexDistribution[picked];
        }
    }
}