using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{

    public class Client
    {
        public string Name { get; set; }
        public bool IsLoanNeeded { get; set; }
        public decimal Income { get; set; }
        public int YearsInJob { get; set; }
        public bool UsesCreditCard { get; set; }
        public bool CriminalRecord { get; set; }
        public int TestVal { get; set; }
    }


    public abstract class Decision
    {
        public abstract void Evaluate(Client client);
    }


    public class DecisionQuery : Decision
    {
        public string Title { get; set; }
        public Decision Positive { get; set; }
        public Decision Negative { get; set; }
        public System.Func<Client, bool> Test { get; set; }

        public override void Evaluate(Client client)
        {
            bool result = this.Test(client);
            string resultAsString = result ? "yes" : "no";

            Debug.Log($"\t- {this.Title}? {resultAsString}");

            if (result)
            {
                this.Positive.Evaluate(client);
                client.TestVal += 1;
            }
            else
            {
                this.Negative.Evaluate(client);
            }
        }
    }

    public class DecisionResult : Decision
    {
        public bool Result { get; set; }
        public override void Evaluate(Client client)
        {
            //Debug.Log("\r\nOFFER A LOAN: {0}", Result ? "YES" : "NO");
        }
    }

    private static DecisionQuery MainDecisionTree()
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
    }

    

    // Start is called before the first frame update
    void Start()
    {
        


    }


    // Update is called once per frame
    void Update()
    {
        var trunk = MainDecisionTree();

        var john = new Client
        {
            Name = "John Doe",
            IsLoanNeeded = true,
            Income = 45000,
            YearsInJob = 4,
            UsesCreditCard = true,
            CriminalRecord = false,
            TestVal = 0
        };

        trunk.Evaluate(john);

        Debug.Log(john.TestVal);

        //Console.WriteLine("Press any key...");
        //Console.ReadKey();
    }
}

