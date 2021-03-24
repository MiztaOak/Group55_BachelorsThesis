using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System;

//class for the entire cell might be kind of point less not to sure
public class Cell
{
    private IInternals cellInternals;

    public Cell(float x, float z, float v, float dT, float angle)
    {
        cellInternals = new Internals(x, z, v, dT, angle);
    }

    public Cell() : this(0, 0, 1.2f, 1, 0) { }
   
    //gets the next location that the cell should move to should only be called when a new location is needed
    public IPointAdapter GetNextLocation()
    {
        return cellInternals.GetNextLocation();
    }

    public State GetInternalState()
    {
        return cellInternals.GetInternalState();
    }

    public float GetAngle()
    {
        return cellInternals.GetAngle();
    }

    internal bool IsDone()
    {
        if (cellInternals is ForwardInternals)
            return ((ForwardInternals)cellInternals).IsDone();
        return false;
    }
}
