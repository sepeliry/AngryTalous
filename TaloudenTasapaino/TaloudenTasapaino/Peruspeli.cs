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

    GameObject draggedWidget = null;

    public override void Begin()
    {
#if DEBUG
        // One of the most typical mobile screen sizes
        SetWindowSize(800, 600);
#endif
        // Controls and event listeners
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Mouse.Listen(MouseButton.Left, ButtonState.Down, CheckForDragStart, "Raahaa tuloja ja menoja paikoilleen");
        Mouse.Listen(MouseButton.Left, ButtonState.Up, CheckForDragEnd, "");

        // Time meter bar
        Widget topLevelWidget = new Widget(new VerticalLayout());
        Meter timeMeter = new DoubleMeter(18, 0, 24);
        ProgressBar availableTime = new ProgressBar(Screen.Width * 0.75, 20, timeMeter);
        topLevelWidget.Add(availableTime);
        Add(topLevelWidget);
        // -----------------------------------
        // Income |  Snakey diagram | Expenses 
        Widget sectionsWidget = new Widget(new HorizontalLayout());
        incomeTransactionWidget = CreateListWidget(DataModel.defaultIncomes, Direction.Left);
        sectionsWidget.Add(incomeTransactionWidget);
        sectionsWidget.Add(new Label(50, 250, "SNAKEY TBD"));
        expenseTransactionWidget = CreateListWidget(DataModel.defaultExpenses, Direction.Right);
        sectionsWidget.Add(expenseTransactionWidget);
        topLevelWidget.Add(sectionsWidget);
        // -----------------------------------
    }

    private void CheckForDragStart()
    {
        GameObject toDragObject = null;
        foreach (var dragCandidate in GetObjectsAt(Mouse.PositionOnScreen))
        {
            if (dragCandidate.Tag == "TRANSACTION")
            {
                toDragObject = dragCandidate;
                break;
            }
        }
        if (toDragObject != null)
        {
            draggedWidget = toDragObject;
            draggedWidget.Parent.Remove(draggedWidget);
            Add(draggedWidget);
        }
    }
    private void CheckForDragEnd()
    {
        draggedWidget = null;
    }


    private Widget CreateListWidget(
        List<DataModel.Transaction> transactions,
        Direction side)
    {
        double xPos = 0;
        if (side == Direction.Left) xPos = -Screen.Width / 4;
        if (side == Direction.Right) xPos = Screen.Width / 4;

        Widget listWidget = new Widget(new VerticalLayout());
        listWidget.Position = new Vector(xPos, 0);
        listWidget.Height = Screen.Height / 2 * 3.0;

        foreach (var t in transactions)
        {
            var box = new Label(300, Math.Abs(t.delta.Money / 5), t.label);
            box.Color = RandomGen.NextLightColor();
            box.Tag = "TRANSACTION";
            listWidget.Add(box);
        }
        return listWidget;
    }

    protected override void Update(Time time)
    {
        base.Update(time);

        // Make sure the transaction lists are anchored to the top of the screen
        incomeTransactionWidget.Y = Screen.Height / 2 - (Screen.Height / 6 + incomeTransactionWidget.Height / 2);
        expenseTransactionWidget.Y = Screen.Height / 2 - (Screen.Height / 6 + expenseTransactionWidget.Height / 2);

        if (draggedWidget != null)
            draggedWidget.Position = Mouse.PositionOnScreen;
    }

}