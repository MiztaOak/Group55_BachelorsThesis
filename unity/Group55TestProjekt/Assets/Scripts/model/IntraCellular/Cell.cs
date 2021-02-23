using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;

public class Cell
{

    private Model model;
    private ICellRegulation regulator;
    private IPointAdapter location, nextlocation;

    private float v; //velocity
    private float dT; //time step
    private float angle;

    public Cell(float x, float z, float v, float dT)
    {
        this.model = Model.GetInstance();
        this.regulator = new HazardRegulation();

        location = new Vector3Adapter(x,z);

        this.v = v;
        this.dT = dT;
        angle = 0;
    }

    public Cell()
    {
        this.model = Model.GetInstance();
        this.regulator = new HazardRegulation();

        location = new Vector3Adapter(0, 0);
    }

    public void CalculateNextPosition()
    {

    }

    public bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        return run;
    }
}
