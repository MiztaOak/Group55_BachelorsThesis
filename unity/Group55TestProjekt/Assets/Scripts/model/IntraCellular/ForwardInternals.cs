using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardInternals : IInternals
{
    private Model model;
    private ICellRegulation regulator;

    private IPointAdapter[] positions;
    private State[] states;

    private int currentIteration = 0;
    private readonly int iterations;

    private readonly float v; //velocity
    private readonly float dT; //time step
    private float angle; //current angle
    private float initalAngel;

    public ForwardInternals(float x, float z, float v, float dT, float angle, ICellRegulation regulator, int iterations)
    {
        this.model = Model.GetInstance();
        this.regulator = regulator;

        //crete the arrays
        positions = new IPointAdapter[iterations+1];
        states = new State[iterations + 1];

        //add the initial values to the arrays
        positions[0] = new Vector3Adapter(x, z);
        AddState(0);

        this.v = v;
        this.dT = dT;
        this.angle = angle;
        initalAngel = angle;
        this.iterations = iterations;

        //Run the forward simulation
        SimulateMovement();
    }

    public bool IsDone()
    {
        return currentIteration == iterations;
    }

    private void SimulateMovement()
    {
        for(int i = 1; i < iterations+1; i++)
        {
            positions[i] = new Vector3Adapter(positions[i - 1].GetX(), positions[i - 1].GetZ());
            angle = CalculateTumbleAngle();

            float dX = v * dT * Mathf.Cos(angle), dZ = v * dT * Mathf.Sin(angle);
            while(GetRunningState(positions[i].GetX(), positions[i].GetZ()))
            {
                positions[i].Add(dX, dZ);
                if (positions[i].GetX() + dX > 14 || positions[i].GetX() - dX < -14 || positions[i].GetZ() + dZ > 14 || positions[i].GetZ() - dZ < -14)
                    break;
            }

            AddState(i);
        }
    }

    //Returns absolute tumble angle in radians
    private float CalculateTumbleAngle()
    {
        //Tumble angle based on article (Edgington)
        float newAngle = Random.Range(18f, 98f);
        float rand = Random.Range(0.0f, 1.0f);
        if (rand > 0.5)
            newAngle *= -1;
        newAngle *= Mathf.PI / 180;
        newAngle += this.angle;
        return newAngle;
    }

    private bool GetRunningState(float x, float z)
    {
        float c = model.environment.getConcentration(x, z);
        bool run = regulator.DecideState(c);
        return run;
    }

    private void AddState(int i)
    {
        State state = new State();
        if (this.regulator is ODERegulation)
        {
            ODERegulation r = (ODERegulation)this.regulator;
            state.yp = r.GetYP();
            state.ap = r.GetAP();
            state.bp = r.GetBP();
            state.m = r.GetM();
            state.l = r.GetL();
        }
        states[i] = state;
    }

    public State GetInternalState()
    {
        return states[currentIteration];
    }

    public IPointAdapter GetNextLocation()
    {
        IPointAdapter point = positions[currentIteration];
        currentIteration = currentIteration+1 > iterations ? iterations : currentIteration+1;
       
        return point;
    }

    public float GetAngle()
    {
        return initalAngel;
    }
}
