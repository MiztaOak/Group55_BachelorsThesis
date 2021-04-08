using System.Collections;
using System.Collections.Generic;
using System;
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
    private int birthDate;
    private Dictionary<int, Cell> children = new Dictionary<int, Cell>();

    private ILifeRegulator lifeRegulator;
    private List<ICellDeathListener> cellDeathListeners;

    private Cell parentObject; //TODO replce this with something smarter

    private ForwardInternals(float v, float dT, float angle, ICellRegulation regulation, int iterations)
    {
        model = Model.GetInstance();
        regulator = regulation;
        this.v = v;
        this.dT = dT;
        this.angle = angle;
        initalAngel = angle;
        this.iterations = iterations;
        deathDate = iterations + 1;

        lifeRegulator = BacteriaFactory.GetInstance().GetLifeRegulator();

        //crete the arrays
        positions = new IPointAdapter[iterations + 1];
        states = new State[iterations + 1];
        cellDeathListeners = new List<ICellDeathListener>();
    }

    //Constructor called when a cell is created in the begining of the program
    public ForwardInternals(float x, float z, float v, float dT, float angle, ICellRegulation regulator, int iterations):
        this(v,dT,angle,regulator,iterations)
    {
        //add the initial values to the arrays
        positions[0] = new Vector3Adapter(x, z);
        AddState(0);
        birthDate = 0;
    }

    //Constructure that is used when a cell is created as the result of a cell division
    public ForwardInternals(IPointAdapter[] locations, State[] states, float v, float dT, float angle, ICellRegulation regulator, int iterations, int iteration) :
        this(v, dT, angle, regulator, iterations)
    {

        //Fix the states so that there are no null pointers this should maybe 
        for(int i = 0; i <= iteration; i++)
        {
            Debug.Log("i = " + i);
            positions[i] = locations[i].Copy();
            this.states[i] = states[i];
        }
        this.positions[iteration + 1] = locations[iteration].Copy();
        this.states[iteration+1] = GetInternalState();
        currentIteration = iteration;
        birthDate = iteration;
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

        if (deathDate <= step)
            return;
        
        float factor = model.GetNumOfCloseCells(step - 1, 0.5f, positions[step - 1]);
        float c = model.environment.getConcentration(positions[step - 1].GetX(), positions[step - 1].GetZ()) / factor;

        if (lifeRegulator.Die(c)) //kill the cell
        {
            deathDate = step;
            model.KillCell(step,parentObject);
            State deathState = new State();
            for(int i = step; i <= iterations; i++) //set the remaining positions and states to the current one
            {
                positions[i] = positions[step-1];
                states[i] = deathState;
            }

            return;
        }
        else if (lifeRegulator.Split(c)) //split the cell
        {
            Split(step);
        }

        positions[step] = new Vector3Adapter(positions[step - 1].GetX(), positions[step - 1].GetZ());
        if (!regulator.DecideState(c))
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
        float newAngle = UnityEngine.Random.Range(18f, 98f);
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
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
            state.death = lifeRegulator.GetDeath();
            state.life = lifeRegulator.GetLife();
        }
        else if(regulator is HazardRegulation)
        {
            state.l = ((HazardRegulation)regulator).GetL();
        }
        states[i] = state;
    }

    private void Split(int iteration)
    {
        IInternals copy = Copy(iteration-1);
        Cell child = new Cell(Copy(iteration-1));
        ((ForwardInternals)copy).SetPartentObject(child);

        model.AddCell(child,iteration);
        children.Add(iteration, child);
    }

    public State GetInternalState()
    {
        return states[currentIteration];
    }

    public IPointAdapter GetNextLocation()
    {
        IPointAdapter point = positions[currentIteration];
        IterationHandler.GetInstance().UpdateIteration(currentIteration);
        if(currentIteration == deathDate) //notify the movement script that the cell should die
        {
            isDone = true;
            CellDoneHandler.CellDone();
            foreach (ICellDeathListener listener in cellDeathListeners)
                listener.Notify();
        }
        else if (children.ContainsKey(currentIteration)) //send the command to create the new e-coli object
        {
            model.GiveBirthToCell(children[currentIteration]);
        }

        if (currentIteration == iterations && !isDone)
        { 
            CellDoneHandler.CellDone();
            isDone = true;
        }
        currentIteration = currentIteration+1 > iterations ? iterations : currentIteration+1;

        if (point == null)
            Debug.Log("Fucked");
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

    public IInternals Copy(int iteration)
    {
        return new ForwardInternals(positions, states, v, dT, angle, regulator.Copy(), iterations, iteration);
    }

    public bool IsDead()
    {
        return currentIteration == deathDate;
    }

    public bool IsSplit()
    {
        return children.ContainsKey(currentIteration);
    }

    internal void SetPartentObject(Cell parentObject)
    {
        this.parentObject = parentObject;
    }

    public void AddListener(ICellDeathListener listener)
    {
        cellDeathListeners.Add(listener);
    }
}
