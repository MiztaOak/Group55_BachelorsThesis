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

    private float smartnessFactor; //decides how random the movement of the bacteria should be 1=random and 0=deterministic

    public SmartInternals(float x, float z, float dT, float v, float smartnessFactor)
    {
        this.dT = dT;
        this.v = v;
        this.smartnessFactor = smartnessFactor;

        model = Model.GetInstance();

        location = new Vector3Adapter(x, z);
        
    }

    private Vector3Adapter CalculateNextPoint(float x, float z, AbstractEnvironment environment)
    {
        for(int i = 0; i < 5; i++)
        {
            float dx = environment.GradX(x, z) * dT * v + smartnessFactor*(float)Normal.Sample(0.0, v * dT * 0.25);
            float dz = environment.GradZ(x, z) * dT * v + smartnessFactor*(float)Normal.Sample(0.0, v * dT * 0.25);

            x = Mathf.Clamp(x + dx, -14, 14);
            z = Mathf.Clamp(z + dz, -14, 14);
        }

        
        //Debug.Log("dx = " + dx + " dz = " + dz);
        return new Vector3Adapter(x, z);
    }

    public IPointAdapter getNextLocation()
    {
        location = CalculateNextPoint(location.GetX(), location.GetZ(), model.environment);
        return location;
    }
}
