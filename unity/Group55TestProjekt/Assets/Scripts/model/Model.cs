using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private Cell[] cells;

    private int cellIndex = 0;
    private int numCells;

    private float[] averageLigandC;

    private Model()
    {
        //add code as it is needed
        environment = new Environment(); //super base case just to prevent any scary null pointers
        timeScaleFactor = 1;
        cells = new Cell[0];
    }

    public static Model GetInstance()
    {
        if (instance == null)
            instance = new Model();
        return instance;
    }

    //Code bellow is a very dumb implementation but will be usefull before the ui is implemented

    //creates a basic enironment
    public void SetEnvironment(float d, float i_0, float x, float y)
    {
        environment = new Environment(d, i_0, x, y);
    }

    //creates a time dependent environment
    public void SetEnvironment(float d, float i_0, float x, float y, float maxTime, float k)
    {
        environment = new TimeDependentEnvironment(d, i_0, x, y, maxTime, k);
    }

    public void SetTimeScaleFactor(float timeScaleFactor)
    {
        this.timeScaleFactor = timeScaleFactor;
    }

    public float GetTimeScaleFactor()
    {
        return timeScaleFactor;
    }

    //Simulates numCells many cells with iterations many steps each
    public void SimulateCells(int numCells, int iterations)
    {
        cells = new Cell[numCells];
        BacteriaFactory.SetCellIterations(iterations);
        this.numCells = numCells;
        cellIndex = 0;

        for (int i = 0; i < numCells; i++)
        {
            cells[i] = BacteriaFactory.CreateNewCell(Random.Range(-10.0F, 10.0F), Random.Range(-10.0F, 10.0F),
                Random.Range(0, 2 * Mathf.PI), false);
        }
    }

    //Sets up the model and factory to simulate numCells many cells with iterations many steps 
    public void SetupCells(int numCells, int iterations)
    {
        cells = new Cell[numCells];
        BacteriaFactory.SetCellIterations(iterations);
        this.numCells = numCells;
        cellIndex = 0;
    }

    //Simulates a single cell
    public void SimulateNextCell(int index)
    {
        if (index >= numCells)
            return;
        cells[index] = BacteriaFactory.CreateNewCell(Random.Range(-10.0F, 10.0F), Random.Range(-10.0F, 10.0F),
            Random.Range(0, 2 * Mathf.PI), false);
    }

    public Cell[] GetCells()
    {
        return cells;
    }

    //Returns the "next" cell is used by the movement class to connect a cell object to a given e-coli object
    public Cell GetCell()
    {
        Cell cell = cells[cellIndex];
        cellIndex = cellIndex + 1 > numCells - 1 ? numCells - 1 : cellIndex + 1;
        return cell;
    }

    public void Reset()
    {
        timeScaleFactor = 1;
        cells = new Cell[0];
        cellIndex = 0;
        numCells = 0;
        averageLigandC = null;
    }

    // metohd to export to fetch and export the needed data ( used in LoadingScreen )
    public void ExportData(int index, int iterations)
    {
        List<Iteration> iteration_list = new List<Iteration>();
        List<DataToExport> data_list = new List<DataToExport>();

        if (index >= numCells)
            return;
        Cell cell = cells[index];


        for (int j = 0; j < iterations; j++)
        {
            float x = cell.GetNextLocation().GetX();
            float z = cell.GetNextLocation().GetZ();
            float ap = (float) cell.GetInternalState().ap;
            float bp = (float) cell.GetInternalState().bp;
            float yp = (float) cell.GetInternalState().yp;
            float m = (float) cell.GetInternalState().m;
            float l = (float) cell.GetInternalState().l;
            oneIteration = new Iteration(j, x, z, ap, bp, yp, m, l);
            iteration_list.Add(oneIteration);
        }

        for (int i = 0; i < numCells; i++)
        {
            cellData = new DataToExport() {id = i, Iterations = iteration_list};
            data_list.Add(cellData);
        }

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
        if (cells.Length == 0)
            return null;

        float[] averageLigandC = new float[BacteriaFactory.GetIterations()+1];

        for(int i = 0; i <= averageLigandC.Length; i++)
        {
            float averageC = 0;
            for (int j = 0; j < cells.Length; j++)
            {
                averageC += (float)((ForwardInternals)cells[j].GetInternals()).GetInternalStates()[i].l;
            }
            averageLigandC[i] = averageC / cells.Length;
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

        public Iteration(int iteration, float x, float z, float ap, float bp, float yp, float m, float l)
        {
            this.iteration = iteration;
            this.x = x;
            this.z = z;
            this.ap = ap;
            this.bp = bp;
            this.yp = yp;
            this.m = m;
            this.l = l;
        }
    }
}