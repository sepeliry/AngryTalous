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
    Widget incomeTransactionWidget;
    Widget expenseTransactionWidget;

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

        Widget sectionsWidget = new Widget(new HorizontalLayout());
        Add(sectionsWidget);

        Meter incomeTimeMeter = new IntMeter(80, 0, 100);
        incomeTransactionWidget = CreateListWidget(DataModel.defaultIncomes, incomeTimeMeter, Direction.Left);
        sectionsWidget.Add(incomeTransactionWidget);

        sectionsWidget.Add(new Label(300, 300, "SNAKEY TBD"));

        Meter expensesTimeMeter = new IntMeter(80, 0, 100);
        expenseTransactionWidget = CreateListWidget(DataModel.defaultExpenses, incomeTimeMeter, Direction.Right);
        sectionsWidget.Add(expenseTransactionWidget);
    }

    private void CheckForDragStart()
    {

    }
    private void CheckForDragEnd()
    {

    }

    private Widget CreateListWidget(
        List<DataModel.Transaction> transactions, 
        Meter availableTimeMeter,
        Direction side)
    {
        double xPos = 0;
        if (side == Direction.Left) xPos = -Screen.Width / 4;
        if (side == Direction.Right) xPos = Screen.Width / 4;

        Widget listWidget = new Widget(new VerticalLayout());
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
