using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardInternals : IInternals
{
    private Model model;
    private ICellRegulation regulator;

    //Arrays containing the positions that were calculated and the states for these positions
    private IPointAdapter[] positions;
    private State[] states;

    private int currentIteration = 0;
    private readonly int iterations;

    private readonly float v; //velocity
    private readonly float dT; //time step
    private float angle; //current angle
    private float initalAngel;

    private bool isDone = false;

    private int deathDate;

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
        this.deathDate = iterations+1;
    }

    public ForwardInternals(IPointAdapter[] locations, State[] states, float v, float dT, float angle, ICellRegulation regulator, int iteration, int iterations)
    {
        this.regulator = regulator;
        positions = new IPointAdapter[iterations + 1];
        states = new State[iterations + 1];
        this.deathDate = iterations + 1;

        //Fix the states so that there are no null pointers this should maybe 
        for(int i = 0; i <= iteration; i++)
        {
            positions[i] = locations[i];
            this.states[i] = states[i];
        }
        this.states[iteration] = GetInternalState();
        currentIteration = iteration;
    }

    //Returns true if the cell has reached its final positon
    public bool IsDone()
    {
        return isDone;
    }

    //Simulates the movement of the cell
    private void SimulateMovement()
    {
        for(int i = 1; i < iterations+1; i++)
        {
            SimulateMovementStep(i);
        }
    }

    //Simulates a single step of movement public since this might be usefull to call from the outside for the dynmaic environment
    public void SimulateMovementStep(int step)
    {
        if (step < 1 || step > iterations)
            throw new IncorrectSimulationStepException(step);

        positions[step] = new Vector3Adapter(positions[step - 1].GetX(), positions[step - 1].GetZ());
        if (!GetRunningState(positions[step].GetX(), positions[step].GetZ()))
        {
            angle = CalculateTumbleAngle();
        }
        else
        {
            float dX = v * dT * Mathf.Cos(angle), dZ = v * dT * Mathf.Sin(angle);

            while (positions[step].GetX() + dX > 14 && positions[step].GetX() - dX < -14 && positions[step].GetZ() + dZ > 14 && positions[step].GetZ() - dZ < -14)
            {
                angle = CalculateTumbleAngle();
                dX = v * dT * Mathf.Cos(angle);
                dZ = v * dT * Mathf.Sin(angle);
            }
            positions[step].Add(dX, dZ);
        }
        AddState(step);
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
        if (currentIteration == iterations && !isDone)
        { 
            CellDoneHandler.CellDone();
            isDone = true;
        }
        currentIteration = currentIteration+1 > iterations ? iterations : currentIteration+1;
       
        return point;
    }

    public float GetAngle()
    {
        return initalAngel;
    }

    public State[] GetInternalStates()
    {
        return states;
    }

    public IPointAdapter GetPosition(int i)
    {
        if (i >= positions.Length)
            return null;
        return positions[i];
    }

    public IInternals Copy()
    {
        return new ForwardInternals(positions,states, v, dT, angle, regulator.Copy(), iterations, currentIteration);
    }
}
