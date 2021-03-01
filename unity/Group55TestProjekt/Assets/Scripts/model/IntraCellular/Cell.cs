using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

//class for the entire cell might be kind of point less not to sure
public class Cell
{
    private IInternals cellInternals;

    public Cell(IInternals cellInternals)
    {
        this.cellInternals = cellInternals;
    }
   
    //gets the next location that the cell should move to should only be called when a new location is needed
    public IPointAdapter GetNextLocation()
    {
        return cellInternals.getNextLocation();
    }
}
