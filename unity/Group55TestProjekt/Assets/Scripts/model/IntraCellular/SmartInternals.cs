using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartInternals : IInternals
{
    private IPointAdapter startLocation;
    private IPointAdapter endLocation;
    private Stack<IPointAdapter> walkLocations;
    private int N;
    private float delta;
    private float dT;
    private Model model;

    public SmartInternals(float x, float z, int N, float delta, float dT)
    {
        this.N = N;
        this.delta = delta;
        this.dT = dT;

        model = Model.GetInstance();

        startLocation = new Vector3Adapter(x, z);
        endLocation = new Vector3Adapter(model.environment.GetX(), model.environment.GetZ());
        walkLocations = new Stack<IPointAdapter>();

        CalculateWalk();
    }

    private void CalculateWalk()
    {

    }

    public IPointAdapter getNextLocation()
    {
        if (walkLocations.Count != 0)
            return walkLocations.Pop();
        return endLocation;
    }
}
