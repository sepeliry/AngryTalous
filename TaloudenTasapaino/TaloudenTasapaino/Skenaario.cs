using System;
using System.Linq;
using System.Collections.Generic;

public class DataModel
{
    public struct Stats
    {
        public double FreeTime;
        public double Happiness;
        public double Energy;
        public double Health;
        public double Money;
    }

    public delegate bool CheckPrerequisites(List<Transaction> incomeTransactions, List<Transaction> expenseTransactions);

    public struct Transaction
    {
        public string label;
        public Stats delta;
        public CheckPrerequisites arePrerequsitesMet;
    }

    public static List<Transaction> defaultIncomes = new List<Transaction>
    {
        new Transaction()
        { label="Kaupan kassa",
          delta=new Stats{
              FreeTime = -8,
              Happiness = -4,
              Energy = -10,
              Health = -4,
              Money = +1800
          },
          arePrerequsitesMet = (inc,exp)=>true // always true
        },
        new Transaction()
        { label="Opintotuki",
          delta=new Stats{
              FreeTime = -4,
              Happiness = 0,
              Energy = -4,
              Health = -2,
              Money = +400
          },
          arePrerequsitesMet = (inc,exp)=>( exp.Any( t => t.label=="Opiskelua" ) )
        }
    };

    public static List<Transaction> defaultExpenses = new List<Transaction>
    {
        new Transaction()
        { label="Pizzaa",
          delta=new Stats{
              FreeTime = -1,
              Happiness = +1,
              Energy = +2,
              Health = -2,
              Money = -300
          },
          arePrerequsitesMet = (inc,exp)=>true // always true
        },
        new Transaction()
        { label="Opiskelua",
          delta=new Stats{
              FreeTime = -4,
              Happiness = 0,
              Energy = -4,
              Health = -2,
              Money = -100
          },
          arePrerequsitesMet = (inc,exp)=>true // always true
        },
        new Transaction()
        { label="Bussikortti",
          delta=new Stats{
              FreeTime = +4,
              Happiness = +2,
              Energy = +2,
              Health = -2,
              Money = -50
          },
          arePrerequsitesMet = (inc,exp)=>true // always true
        }
    };

    public static List<Transaction> incomeOptions = new List<Transaction>
    {
        // TBD
    };
    public static List<Transaction> expenseOptions = new List<Transaction>
    {
        // TBD
    };
}