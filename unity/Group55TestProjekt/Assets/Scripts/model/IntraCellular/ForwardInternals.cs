using System.Collections.Generic;

public class ForwardInternals : AbstractInternals
{

    //Arrays containing the positions that were calculated and the states for these positions
    private IPointAdapter[] positions;
    private State[] states;

    private int currentIteration = 0;
    private readonly int iterations;

    private float initalAngel;

    private bool isDone = false;

    private int deathDate;
    private int birthDate;
    private Dictionary<int, Cell> children = new Dictionary<int, Cell>();

    private ILifeRegulator lifeRegulator;
    private List<ICellDeathListener> cellDeathListeners;

    private Cell parentObject; //TODO replce this with something smarter

    private ForwardInternals(float v, float dT, float angle, ICellRegulation regulation, int iterations): base(v,dT,angle,regulation)
    {      
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

        if (deathDate <= step || positions[step] != null) //check if cell is dead or already handled in a previous timestep
            return;
       
        float c = model.environment.GetConcentration(positions[step - 1].GetX(), positions[step - 1].GetZ(),step-1);

        if (lifeRegulator.Die(c)) //check if the cell should die and kill it if that is the case
        {
            deathDate = step;
            model.KillCell(step,parentObject);
            State deathState = new State();
            for(int i = step; i <= iterations; i++) //set the remaining positions and states to the current one
            {
                positions[i] = positions[step-1];
                states[i] = deathState;
                states[i].death = 1;
            }

            return;
        }
        else if (lifeRegulator.Split(c)) //check if the cell should split and split the cell if that is the case
        {
            Split(step);
        }

        positions[step] = new Vector3Adapter(positions[step - 1].GetX(), positions[step - 1].GetZ());
        if (!regulator.DecideState(c)) //tumble
        {
            angle = CalculateTumbleAngle();
            AddState(step);
            step++;
            if (step == positions.Length)
                return;
            positions[step] = positions[step-1].Copy();
        }

        CalculateNextLocation(positions[step]);
        AddState(step);
    }

    private void AddState(int i)
    {
        State state = new State();
        if (this.regulator is ODERegulation r)
        {
            state.yp = r.GetYP();
            state.ap = r.GetAP();
            state.bp = r.GetBP();
            state.m = r.GetM();
            state.l = r.GetL();
            state.death = lifeRegulator.GetDeath();
            state.life = lifeRegulator.GetLife();
        }
        else if (this.regulator is DeltaRegulation h)
        {
            state.l = h.GetL();
        }
        states[i] = state;
    }

    private void Split(int iteration)
    {
        IInternals copy = Copy(iteration-1);
        Cell child = new Cell(Copy(iteration-1));
        ((ForwardInternals)copy).SetPartentObject(child);
        //birthDate = iteration - 1;

        model.AddCell(child,iteration);
        children.Add(iteration, child);
    }

    public override State GetInternalState()
    {
        return states[currentIteration];
    }

    public override IPointAdapter GetNextLocation()
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

        return point;
    }

    public override float GetAngle()
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

    public override IInternals Copy()
    {
        return new ForwardInternals(positions,states, v, dT, angle, regulator.Copy(), iterations, currentIteration);
    }

    public IInternals Copy(int iteration)
    {
        return new ForwardInternals(positions, states, v, dT, angle, regulator.Copy(), iterations, iteration);
    }

    public override bool IsDead()
    {
        return currentIteration == deathDate;
    }

    public override bool IsSplit()
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

    public int BirthDate
    {
        get => birthDate;
        set => birthDate = value;
    }

    public int DeathDate
    {
        get => deathDate;
        set => deathDate = value;
    }
}
