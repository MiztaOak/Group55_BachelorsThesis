using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//Class that will manage the overarching data and operations that are needed by all of the program, sort of like the hub of the program
public class Model
{
    private static Model instance;
    private DataToExport cellData;
    private Iteration oneIteration;


    public AbstractEnvironment
        environment { get; set; } //allows for the environment to be changed by the program if needed

    private float
        timeScaleFactor; //variable that scales the "time" of the simulation might be better to place this in a repo class later

    private List<Cell> allCells; //all cells that where ever present in the simulation
    private List<Cell>[] cells; //matrix where each list contains the cells that existed in that time step

    private int[] numCells;

    private float[] averageLigandC;

    private List<ICellBirthListener> cellBirthListeners;

    private Model()
    {
        //add code as it is needed
        environment = new Environment(); //super base case just to prevent any scary null pointers
        timeScaleFactor = 1;
        cells = new List<Cell>[0];
        cellBirthListeners = new List<ICellBirthListener>();
        allCells = new List<Cell>();
    }

    public static Model GetInstance()
    {
        if (instance == null)
            instance = new Model();
        return instance;
    }

    //Simulates numCells many cells with iterations many steps each
    public void CreateCells(int numCells)
    {
        cells[0] = new List<Cell>();
        for (int i = 0; i < numCells; i++)
        {
            float x = RandomFloat.Range(-12f, 12f);
            float z = RandomFloat.Range(-12f, 12f);
            /* use this incase you want to prevent cells from spawning in the center
            float z;
            if (x < 3 && x > -3) //if x close to the center make sure that the cell does not spawn in the center
            {
                z = Random.Range(3f, 12f) * (Random.value <= 0.5 ? -1 : 1);
            }
            else
            {
                z = Random.Range(-12f, 12f);
            }
            */

            Cell cell = BacteriaFactory.CreateNewCell(x, z, RandomFloat.Range(0, (float)(2 * Math.PI)), false);
            cells[0].Add(cell);
            allCells.Add(cell);
        }
    }

    public void SimulateTimeStep(int timeStep)
    {
        if (timeStep != 0)
            numCells[timeStep] = numCells[timeStep - 1];

        cells[timeStep - 1].ForEach(c => cells[timeStep].Add(c)); //Copy all the cells from the previous time step
        //add code for updating the environment or something i guess
        for (int i = 0; i < cells[timeStep].Count; i++)
        {
            int tmp = cells[timeStep].Count;
            ((ForwardInternals) cells[timeStep][i].GetInternals()).SimulateMovementStep(timeStep);

            if (tmp > cells[timeStep].Count) //if the current cell died and was removed the index has to be updated
                i--;
        }
    }

    //Sets up the model and factory to simulate numCells many cells with iterations many steps 
    public void SetupCells(int numCells, int iterations)
    {
        cells = new List<Cell>[iterations + 1];
        for (int i = 0; i < cells.Length; i++)
            cells[i] = new List<Cell>();

        BacteriaFactory.SetCellIterations(iterations);
        this.numCells = new int[iterations + 1];
        this.numCells[0] = numCells;
        timeScaleFactor = 1;
        cellBirthListeners = new List<ICellBirthListener>();
        allCells = new List<Cell>();
        IterationHandler.GetInstance().Reset();
        averageLigandC = null;
    }

    //Method that adds a new cell to the simulation
    public void AddCell(Cell cell, int iteration)
    {
        cells[iteration].Add(cell);
        allCells.Add(cell);
        numCells[iteration]++;
    }

    public void GiveBirthToCell(Cell cell)
    {
        CellDoneHandler.Birth();
        foreach (ICellBirthListener listener in cellBirthListeners)
            listener.Notify(cell);
    }

    //Method that removes a cell that has died
    public void KillCell(int iteration, Cell cell)
    {
        numCells[iteration]--;
        cells[iteration].Remove(cell);
    }

    //Returns the cells for a given timeStep
    public List<Cell> GetCells(int timeStep)
    {
        return cells[timeStep];
    }

    //Returns all the cells that were ever present in the simulation
    public List<Cell> GetCells()
    {
        return allCells;
    }

    // metohd to export to fetch and export the needed data ( used in LoadingScreen )
    public void ExportData(int index, int iterations)
    {
        int currentCellNum = 0;


        if (iterations == 0)
            return;
        var data_list = new List<object>();
        List<Iteration> iteration_list = new List<Iteration>();
        //List<DataToExport> data_list = new List<DataToExport>();
        List<int> currentCellNumList = new List<int>();

        if (index >= allCells.Count && allCells.Count == 0)
            return;

        for (int i = 0; i < allCells.Count; i++)
        {
            ForwardInternals cell = ((ForwardInternals) allCells[i].GetInternals());


            for (int j = 0; j < iterations; j++)
            {
                float x = cell.GetPosition(j).GetX();
                float z = cell.GetPosition(j).GetZ();
                State interalState = cell.GetInternalStates()[j];
                float ap = (float) interalState.ap;
                float bp = (float) interalState.bp;
                float yp = (float) interalState.yp;
                float m = (float) interalState.m;
                float l = (float) interalState.l;
                float life = (float) interalState.life;
                float death = (float) interalState.death;
                int birth_date =  cell.BirthDate;
                int death_date = cell.DeathDate;

                oneIteration = new Iteration(j, x, z, ap, bp, yp, m, l, life, death,birth_date,death_date);
                iteration_list.Add(oneIteration);
            }

            List<Iteration> copy = new List<Iteration>(iteration_list);
            cellData = new DataToExport() {id = i, Iterations = copy};
            data_list.Add(cellData);
            iteration_list.Clear();
        }

        for (int i = 0; i < iterations; i++)
        {
            currentCellNum = GetNumCells(i);
            currentCellNumList.Add(currentCellNum);
        }

        data_list.Add(currentCellNumList);


        ExportHandler.exportData(data_list);
    }

    // Class represents all information related to a single cell during the simulation
    public class DataToExport
    {
        public int id;
        public List<Iteration> Iterations;
    }

    //Calculates the average ligand consentration for each time step
    private float[] CalculateAverageLigandC()
    {
        if (allCells.Count == 0)
            return null;

        float[] averageLigandC = new float[BacteriaFactory.GetIterations()];

        for (int i = 0; i < averageLigandC.Length; i++) //for each iteration
        {
            float averageC = 0;
            for (int j = 0; j < cells[i + 1].Count; j++) //for the cells present in that iteration
            {
                averageC += (float) ((ForwardInternals) cells[i + 1][j].GetInternals()).GetInternalStates()[i + 1].l;
            }

            averageLigandC[i] = (numCells[i + 1] != 0 ? averageC / numCells[i + 1] : 0);
        }

        return averageLigandC;
    }

    //Gets the list of average ligand consentrations only calculating them once per simulation
    public float[] GetAverageLigandC()
    {
        if (averageLigandC == null)
            averageLigandC = CalculateAverageLigandC();
        return averageLigandC;
    }

    // Class representing infromation held in one iteration.
    public class Iteration
    {
        public int iteration;
        public float x;
        public float z;
        public float ap;
        public float bp;
        public float yp;
        public float m;
        public float l;
        public float life;
        public float death;
        public int birth_date;
        public int death_date;

        public Iteration(int iteration, float x, float z, float ap, float bp, float yp, float m, float l, float life,
            float death,int birth_date,int death_date)
        {
            this.iteration = iteration;
            this.x = x;
            this.z = z;
            this.ap = ap;
            this.bp = bp;
            this.yp = yp;
            this.m = m;
            this.l = l;
            this.life = life;
            this.death = death;
            this.birth_date = birth_date;
            this.death_date = death_date;
        }
    }

    public void SetTimeScaleFactor(float timeScaleFactor)
    {
        this.timeScaleFactor = timeScaleFactor;
    }

    public float GetTimeScaleFactor()
    {
        return timeScaleFactor;
    }

    public int GetNumCells(int iteration)
    {
        return numCells[iteration]; //might cause problems later
    }

    //Method that returns the number of cells that are within a given distance of the given location
    //only to be used with cells that have a ForwardsInternals as their internals (since iteration does not make sense otherwise)
    public int GetNumOfCloseCells(int iteration, float distance, IPointAdapter location)
    {
        int num = 0;

        distance *= distance; //removes the need for sqrt operation

        foreach (Cell cell in cells[iteration])
        {
            if (!(cell.GetInternals() is ForwardInternals))
                continue;
            IPointAdapter cellLocation = ((ForwardInternals)cell.GetInternals()).GetPosition(iteration);
            if (MathFloat.Pow(cellLocation.GetX() - location.GetX(), 2) + MathFloat.Pow(cellLocation.GetZ() - location.GetZ(), 2) <= distance)
                num++;
        }

        return num;
    }

    public void AddListener(ICellBirthListener listener)
    {
        cellBirthListeners.Add(listener);
    }
}