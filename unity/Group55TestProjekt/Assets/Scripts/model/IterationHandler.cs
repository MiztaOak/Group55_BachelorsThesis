using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterationHandler
{
    private static IterationHandler instance;
    private int currentIteration;
    private List<IIterationListener> iterationListeners;

    private IterationHandler()
    {
        currentIteration = 0;
        iterationListeners = new List<IIterationListener>();
    }

    public static IterationHandler GetInstance()
    {
        if (instance == null)
            instance = new IterationHandler();
        return instance;
    }

    public void Reset()
    {
        instance = new IterationHandler();
    }

    public void UpdateIteration(int iteration)
    {
        if(iteration > currentIteration)
        {
            currentIteration = iteration;
            iterationListeners.ForEach(n => n.NotifyIterationChanged(currentIteration));
        }
    }

    public int GetCurrentInteration()
    {
        return currentIteration;
    }

    public void Subscribe(IIterationListener listener)
    {
        iterationListeners.Add(listener);
    }
}
