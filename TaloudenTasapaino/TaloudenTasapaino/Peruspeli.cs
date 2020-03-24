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
    List<GameObject> incomeTransactionBoxes;
    List<GameObject> expenseTransactionBoxes;

    Dictionary<GameObject, List<GameObject>> dropAreaToTransactionList;

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

        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, CheckForDragStart, "Raahaa tuloja ja menoja paikoilleen");
        Mouse.Listen(MouseButton.Left, ButtonState.Released, CheckForDragEnd, "");

        // Time meter bar
        Meter timeMeter = new DoubleMeter(18, 0, 24);
        ProgressBar availableTime = new ProgressBar(Screen.Width * 0.85, 20, timeMeter);
        availableTime.Color = Color.LightCyan;
        availableTime.Position = new Vector(0, 0.33 * Screen.Height);
        Add(availableTime);

        // -----------------------------------
        // Income |  Snakey diagram | Expenses 
        incomeTransactionBoxes = CreateTransactionBoxes(DataModel.defaultIncomes, Direction.Left);
        expenseTransactionBoxes = CreateTransactionBoxes(DataModel.defaultExpenses, Direction.Right);
        // -----------------------------------
        
        LayoutTransactionBoxes(incomeTransactionBoxes);
        LayoutTransactionBoxes(expenseTransactionBoxes);

        GameObject incomeDropBox = CreateDropArea(incomeTransactionBoxes[0]);
        GameObject expenseDropBox = CreateDropArea(expenseTransactionBoxes[0]);
        dropAreaToTransactionList = new Dictionary<GameObject, List<GameObject>>();
        dropAreaToTransactionList.Add(incomeDropBox, incomeTransactionBoxes);
        dropAreaToTransactionList.Add(expenseDropBox, expenseTransactionBoxes);
    }

    private GameObject CreateDropArea(GameObject anchor)
    {
        GameObject dropArea = new GameObject(anchor.Width, Screen.Height * 3 / 4.0);
        dropArea.Position = new Vector(anchor.X, anchor.Y - anchor.Height / 2 - dropArea.Height / 2);
        dropArea.Tag = "DROP_HERE";
        Add(dropArea, -1);
        return dropArea;
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
            if (incomeTransactionBoxes.Contains(draggedWidget)) incomeTransactionBoxes.Remove(draggedWidget);
            if (expenseTransactionBoxes.Contains(draggedWidget)) expenseTransactionBoxes.Remove(draggedWidget);
        }
    }
    private void CheckForDragEnd()
    {
        GameObject toDropTo = null;
        foreach (var toDropToCandidate in GetObjectsAt(Mouse.PositionOnScreen))
        {
            if (toDropToCandidate.Tag == "DROP_HERE")
            {
                toDropTo = toDropToCandidate;
                break;
            }
        }
        if (toDropTo != null && dropAreaToTransactionList.ContainsKey(toDropTo))
        {
            List<GameObject> transactionList = dropAreaToTransactionList[toDropTo];
            transactionList.Add(draggedWidget);
        }
        else
        {
            //TODO: Return the dragged object to where it originated from.
        }
        draggedWidget = null;
    }

    private List<GameObject> CreateTransactionBoxes(
        List<DataModel.Transaction> transactions,
        Direction side)
    {
        var boxList = new List<GameObject>();

        GameObject anchorObject = new GameObject(Screen.Width * 0.35, 5);
        anchorObject.Color = Color.Transparent;
        double xPos = 0;
        if (side == Direction.Left) xPos = -Screen.Width / 4;
        if (side == Direction.Right) xPos = Screen.Width / 4;
        anchorObject.Position = new Vector(xPos, Screen.Height * 0.3);
        Add(anchorObject);
        boxList.Add(anchorObject);

        foreach (var t in transactions)
        {
            var box = new Label(Screen.Width * 0.35, Math.Abs(t.delta.Money / 5), t.label);
            box.Color = RandomGen.NextLightColor();
            box.Tag = "TRANSACTION";
            boxList.Add(box);
            Add(box);
        }
        return boxList;
    }

    protected override void Update(Time time)
    {
        base.Update(time);

        // Make sure the transaction lists are anchored to the top of the screen
        double y = incomeTransactionBoxes[0].Y + incomeTransactionBoxes[0].Height / 2;

        LayoutTransactionBoxes(incomeTransactionBoxes);
        LayoutTransactionBoxes(expenseTransactionBoxes);

        if (draggedWidget != null)
            draggedWidget.Position = Mouse.PositionOnScreen;
    }

    private void LayoutTransactionBoxes(List<GameObject> transactionBoxes)
    {
        var anchorObject = transactionBoxes[0];
        double y = anchorObject.Y - anchorObject.Height / 2;
        foreach (var transactionBox in transactionBoxes)
        {
            if (transactionBox.Tag != "TRANSACTION") continue;

            transactionBox.Y = y - transactionBox.Height / 2;
            transactionBox.X = anchorObject.X;
            y -= transactionBox.Height;
        }
    }
}
