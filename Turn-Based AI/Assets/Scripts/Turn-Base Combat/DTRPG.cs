using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTRPG : MonoBehaviour
{
    public class Data
    {
        // Target Data
        public bool TargetAlive { get; set; }

        public bool BluntWeak { get; set; }
        public bool SlashWeak { get; set; }
        public bool PierceWeak { get; set; }
        public bool FireWeak { get; set; }
        public bool WindWeak { get; set; }
        public bool EarthWeak { get; set; }
        public bool WaterWeak { get; set; }

        public float TargetHP { get; set; }
        public float TargetMP { get; set; }

        public List<GameObject> Heroes { get; set; }
        public List<BasicAttack> TargetAttacks { get; set; }

        // User data
        public float HP { get; set; }
        public float MP { get; set; }
        public List<BasicAttack> EnemyAttacks { get; set; }
    }


    public abstract class Decision
    {
        public abstract void Evaluate(Data client);
    }


    public class DecisionQuery : Decision
    {
        public string Title { get; set; }
        public Decision Positive { get; set; }
        public Decision Negative { get; set; }
        public System.Func<Data, bool> Test { get; set; }

        public override void Evaluate(Data client)
        {
            bool result = this.Test(client);

            if (result) this.Positive.Evaluate(client);
            else this.Negative.Evaluate(client);
        }
    }

    public class DecisionResult : Decision
    {
        public bool Result { get; set; }
        public override void Evaluate(Data client)
        {
            
        }
    }

    /*private static DecisionQuery MainDecisionTree()
    {
        //Decision 4
        var creditBranch = new DecisionQuery
        {
            Title = "Use credit card",
            Test = (client) => client.UsesCreditCard,
            Positive = new DecisionResult { Result = true },
            Negative = new DecisionResult { Result = false }
        };

        //Decision 3
        var experienceBranch = new DecisionQuery
        {
            Title = "Have more than 3 years experience",
            Test = (client) => client.YearsInJob > 3,
            Positive = creditBranch,
            Negative = new DecisionResult { Result = false }
        };


        //Decision 2
        var moneyBranch = new DecisionQuery
        {
            Title = "Earn more than 40k per year",
            Test = (client) => client.Income > 40000,
            Positive = experienceBranch,
            Negative = new DecisionResult { Result = false }
        };

        //Decision 1
        var criminalBranch = new DecisionQuery
        {
            Title = "Have a criminal record",
            Test = (client) => client.CriminalRecord,
            Positive = new DecisionResult { Result = false },
            Negative = moneyBranch
        };

        //Decision 0
        var trunk = new DecisionQuery
        {
            Title = "Want a loan",
            Test = (client) => client.IsLoanNeeded,
            Positive = criminalBranch,
            Negative = new DecisionResult { Result = false }
        };

        return trunk;
    }*/

    

    // Start is called before the first frame update
    void Start()
    {
        


    }


    // Update is called once per frame
    void Update()
    {
        /*var trunk = MainDecisionTree();

        var john = new Client
        {
            Name = "John Doe",
            IsLoanNeeded = true,
            Income = 45000,
            YearsInJob = 4,
            UsesCreditCard = true,
            CriminalRecord = false
        };

        trunk.Evaluate(john);*/

        //Console.WriteLine("Press any key...");
        //Console.ReadKey();
    }
}
