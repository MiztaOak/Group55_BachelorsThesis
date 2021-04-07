using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDoneHandler
{
    private static int cellsNotDone;
    private static List<ICellDoneListener> cellDoneListeners = new List<ICellDoneListener>();

    public static void AddListener(ICellDoneListener listener)
    {
        cellDoneListeners.Add(listener);
    }

    public static void CellDone()
    {
        cellsNotDone--;
        if(cellsNotDone == 0)
        {
            foreach (ICellDoneListener listener in cellDoneListeners)
                listener.OnCellDone();
        }
    }

    public static void Birth()
    {
        cellsNotDone++;
    }

    public static void Setup(int cells)
    {
        cellsNotDone = cells;
        cellDoneListeners = new List<ICellDoneListener>();
    }
}
