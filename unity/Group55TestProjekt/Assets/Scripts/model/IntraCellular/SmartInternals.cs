using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;

public class SmartInternals : IInternals
{
    private IPointAdapter location;
    private readonly float dT;
    private Model model;

    private readonly float v;

    private float smartnessFactor; //decides how random the movement of the bacteria should be 1=deterministic and 0=random

    public SmartInternals(float x, float z, float dT, float v, float smartnessFactor)
    {
        this.dT = dT;
        this.v = v;
        this.smartnessFactor = smartnessFactor;//smartnessFactor;

        model = Model.GetInstance();

        location = new Vector3Adapter(x, z);
        
    }

    private Vector3Adapter CalculateNextPoint(float x, float z, AbstractEnvironment environment)
    {
        //calculates the angle that the cell should move towards based on the ligand gradient
        float alfa = Mathf.Atan2(environment.GradZ(x, z), environment.GradX(x, z));
        float factor = 1 - smartnessFactor;

        //calculates the x and z delta based on the angle and the sampled value
        float dx = Mathf.Cos(alfa) * dT * v + (float)Normal.Sample(0.0, v * dT * factor);
        float dz = Mathf.Sin(alfa) * dT * v + (float)Normal.Sample(0.0, v * dT * factor);
        Debug.Log("dx = " + dx + " dz = " + dz);

        //adds the delta to the x and z cords while making sure that it does not move outside the dish
        x = Mathf.Clamp(x +dx, -14, 14);
        z = Mathf.Clamp(z + dz, -14, 14);

        //create the new point
        return new Vector3Adapter(x, z);
    }

    public IPointAdapter GetNextLocation()
    {
        location = CalculateNextPoint(location.GetX(), location.GetZ(), model.environment);
        return location;
    }

    public State GetInternalState() {
        State s = new State
        {
            l = model.environment.getConcentration(location.GetX(), location.GetZ())
        };
        return s;
    }

    public float GetAngle()
    {
        throw new System.NotImplementedException();
    }

    public IInternals Copy()
    {
        return new SmartInternals(location.GetX(), location.GetZ(), dT, v, smartnessFactor);
    }

    public bool IsDead()
    {
        return false;
    }

    public bool IsSplit()
    {
        return false;
    }
}
