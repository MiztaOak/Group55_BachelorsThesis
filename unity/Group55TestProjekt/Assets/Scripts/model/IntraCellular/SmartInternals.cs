using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.Distributions;

public class SmartInternals : IInternals
{
    private IPointAdapter startLocation;
    private IPointAdapter endLocation;
    private Queue<IPointAdapter> walkLocations;
    private int N;
    private float delta;
    private float dT;
    private Model model;

    private float v;

    public SmartInternals(float x, float z, float dT, float v)
    {
        this.dT = dT;
        this.v = v;

        model = Model.GetInstance();

        startLocation = new Vector3Adapter(x, z);
        endLocation = new Vector3Adapter(model.environment.GetX(), model.environment.GetZ());
        walkLocations = new Queue<IPointAdapter>();

        //walkLocations.Push(CalculateNextPoint(startLocation.GetX(), startLocation.GetZ(),model.environment));
       /* Vector3Adapter tmp = CalculateNextPoint(startLocation.GetX(), startLocation.GetZ(), model.environment);
        int i = 0;
        
        while (DistTo(tmp,endLocation) > 0.5f && i < 1000)
        {
            tmp = CalculateNextPoint(tmp.GetX(), tmp.GetZ(), model.environment);
           
            walkLocations.Enqueue(tmp);
            i++;
        }
        Debug.Log("Size of stack is " + walkLocations.Count);*/
        
    }

    private float DistTo(IPointAdapter p1, IPointAdapter p2)
    {
        return Mathf.Sqrt(Mathf.Pow(p1.GetX() - p2.GetX(), 2) + Mathf.Pow(p1.GetZ() - p2.GetZ(), 2));
    }

    private Vector3Adapter CalculateNextPoint(float x, float z, AbstractEnvironment environment)
    {
        float dx = environment.GradX(x, z) * dT;
        float dz = environment.GradZ(x, z) * dT;
        Debug.Log("dx = " + dx + " dz = " + dz);
        return new Vector3Adapter(x + dx, z + dz);
    }

    public IPointAdapter getNextLocation()
    {
        /*if (walkLocations.Count != 0)
            return walkLocations.Dequeue();
        Debug.Log("Shit is empty bro");*/
        startLocation = CalculateNextPoint(startLocation.GetX(), startLocation.GetZ(), model.environment);
        return startLocation;
    }

    /*
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
        float dist = Mathf.Sqrt(Mathf.Pow(startLocation.GetX() - endLocation.GetX(), 2) + Mathf.Pow(startLocation.GetZ() - endLocation.GetZ(), 2))/N;

        walkLocations.Push(CalculateNextLocation(startLocation, dist));

        double[,] normalVals = new double[N,N];
        for(int i = 0; i < N; i++)
        {
            for(int j = 0; j < N; j++)
            {
                normalVals[i,j] = (Normal.Sample(0.0, 1.0)-N)/(delta*Mathf.Sqrt(dT));
            }
            //Normal.Samples(normalVals[i], 0.0, 1.0);
        }

        for (int i = 0; i < N-1; i++)
        {
            
            //walkLocations.Push(CalculateNextLocation(walkLocations.Peek(), dist));
        }

    }

    private float[] CumSum(float[] input)
    {
        float[] output = new float[input.Length];

        output[0] = input[0];
        for(int i = 1; i < input.Length; i++)
        {
            output[i] = input[i] + output[i - 1];
        }

        return output;
    }

    private IPointAdapter CalculateNextLocation(IPointAdapter location, float dist)
    {
        float angle = CalculateTumbleAngle(startLocation);
        float dX = dist * Mathf.Cos(angle), dZ = dist * Mathf.Sin(angle);
        return new Vector3Adapter(location.GetX() + dX, location.GetZ() + dZ);
    }

    

    public IPointAdapter getNextLocation()
    {
        if (walkLocations.Count != 0)
            return walkLocations.Pop();
        return endLocation;
    }*/
}
