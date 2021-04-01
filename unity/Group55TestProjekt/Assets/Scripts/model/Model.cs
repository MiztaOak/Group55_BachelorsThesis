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

    private List<Cell> cells;

    private int[] numCells;

    private float[] averageLigandC;

    private Model()
    {
        //add code as it is needed
        environment = new Environment(); //super base case just to prevent any scary null pointers
        timeScaleFactor = 1;
        cells = new List<Cell>();
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
        for (int i = 0; i < numCells; i++)
        {
            cells.Add(BacteriaFactory.CreateNewCell(Random.Range(-10.0F, 10.0F), Random.Range(-10.0F, 10.0F),
                Random.Range(0, 2 * Mathf.PI), false));
        }
    }

    public void SimulateTimeStep(int timeStep)
    {
        //add code for updating the environment or something i guess
        foreach (Cell cell in cells)
        {
            ((ForwardInternals)cell.GetInternals()).SimulateMovementStep(timeStep);
        }
    }

    //Sets up the model and factory to simulate numCells many cells with iterations many steps 
    public void SetupCells(int numCells, int iterations)
    {
        cells = new List<Cell>(numCells);
        BacteriaFactory.SetCellIterations(iterations);
        this.numCells = new int[iterations + 1];
        for (int i = 0; i <= iterations; i++)
            this.numCells[i] = numCells;
        timeScaleFactor = 1;
    }

    public void AddCell(Cell cell,int iteration)
    {
        cells.Add(cell);
        numCells[iteration]++;
    }

    public void KillCell(int iteration)
    {
        numCells[iteration]--;
    }

    public List<Cell> GetCells()
    {
        return cells;
    }

    // metohd to export to fetch and export the needed data ( used in LoadingScreen )
    public void ExportData(int index, int iterations)
    {
        if (iterations == 0)
            return;

        List<Iteration> iteration_list = new List<Iteration>();
        List<DataToExport> data_list = new List<DataToExport>();
        int Iteration_counter = 0;

        if (index >= cells.Count && cells.Count == 0)
            return;

        for (int i = 0; i < index; i++)
        {
            ForwardInternals cell = ((ForwardInternals) cells[i].GetInternals());


            for (int j = 0; j < iterations; j++)
            {
                Debug.Log(cell.GetHashCode());
                float x = cell.GetPosition(j).GetX();
                float z = cell.GetPosition(j).GetZ();
                State interalState = cell.GetInternalStates()[j];
                float ap = (float) interalState.ap;
                float bp = (float) interalState.bp;
                float yp = (float) interalState.yp;
                float m = (float) interalState.m;
                float l = (float) interalState.l;
                oneIteration = new Iteration(j, x, z, ap, bp, yp, m, l);
                iteration_list.Add(oneIteration);
                Iteration_counter++;
            }

            List<Iteration> copy = new List<Iteration>(iteration_list);
            cellData = new DataToExport() {id = i, Iterations = copy};
            data_list.Add(cellData);
            iteration_list.Clear();
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
        if (cells.Count == 0)
            return null;

        float[] averageLigandC = new float[BacteriaFactory.GetIterations()];

        for (int i = 0; i < averageLigandC.Length; i++)
        {
            float averageC = 0;
            for (int j = 0; j < cells.Count; j++)
            {
                averageC += (float) ((ForwardInternals) cells[j].GetInternals()).GetInternalStates()[i + 1].l;
            }

            averageLigandC[i] = averageC / cells.Count;
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
}