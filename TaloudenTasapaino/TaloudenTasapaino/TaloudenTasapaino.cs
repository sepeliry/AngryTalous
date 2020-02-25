using System;
using System.Linq;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class TaloudenTasapaino : Game
{
    struct Stats
    {
        public double FreeTime;
        public double Happiness;
        public double Energy;
        public double Health;
        public double Money;
    }

    delegate bool CheckPrerequisites(List<Transaction> incomeTransactions, List<Transaction> expenseTransactions);

    struct Transaction
    {
        public string label;
        public Stats delta;
        public CheckPrerequisites arePrerequsitesMet;
    }

    Widget incomeTransactionWidget;
    Widget expenseTransactionWidget;

    List<Transaction> defaultIncomes = new List<Transaction>
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

    List<Transaction> defaultExpenses = new List<Transaction>
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

    Widget draggedWidget = null;

    public override void Begin()
    {
#if DEBUG
        // One of the most typical mobile screen sizes
        SetWindowSize(800, 600);
#endif

        // Kirjoita ohjelmakoodisi tähän

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Mouse.Listen(MouseButton.Left, ButtonState.Down, CheckForDragStart, "Raahaa tuloja ja menoja paikoilleen");
        Mouse.Listen(MouseButton.Left, ButtonState.Up, CheckForDragEnd, "");

        Meter incomeTimeMeter = new IntMeter(80, 0, 100);
        incomeTransactionWidget = CreateListWidget(defaultIncomes, incomeTimeMeter, Direction.Left);
        Meter expensesTimeMeter = new IntMeter(80, 0, 100);
        expenseTransactionWidget = CreateListWidget(defaultExpenses, incomeTimeMeter, Direction.Right);
    }

    private void CheckForDragStart()
    {

    }
    private void CheckForDragEnd()
    {

    }

    private Widget CreateListWidget(
        List<Transaction> transactions, 
        Meter availableTimeMeter,
        Direction side)
    {
        double xPos = 0;
        if (side == Direction.Left) xPos = -Screen.Width / 4;
        if (side == Direction.Right) xPos = Screen.Width / 4;
        
        Widget listWidget = new Widget(new VerticalLayout());
        Add(listWidget);
        listWidget.Position = new Vector(xPos, 0);
        listWidget.Height = Screen.Height / 2 * 3.0;

        ProgressBar availableTime = new ProgressBar(300, 20, availableTimeMeter);
        listWidget.Add(availableTime);

        foreach (var t in transactions)
        {
            var box = new Label(300, Math.Abs(t.delta.Money / 5), t.label);
            box.Color = RandomGen.NextLightColor();
            box.Tag = "TRANSACTION";
            listWidget.Add(box);
        }
        return listWidget;
    }

    void DragTransaction(Widget transaction)
    {
        draggedWidget = transaction;
        transaction.Parent.Remove(transaction);
    }

    protected override void Update(Time time)
    {
        base.Update(time);

        // Make sure the transaction lists are anchored to the top of the screen
        incomeTransactionWidget.Y = Screen.Height/2-(Screen.Height/6 + incomeTransactionWidget.Height/2);
        expenseTransactionWidget.Y = Screen.Height/2-(Screen.Height/6 + expenseTransactionWidget.Height/2);
    }

}
