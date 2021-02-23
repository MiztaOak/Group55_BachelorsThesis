using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class Cell
{

    private Model model;
    private ICellRegulation regulator;

    public Cell()
    {
        this.model = Model.GetInstance();
        this.regulator = new ODERegulation();
    }

    public bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        Debug.Log("Running: " + run);
        return run;
    }
}
