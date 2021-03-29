using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that will manage the overarching data and operations that are needed by all of the program, sort of like the hub of the program
public class Model
{
    private static Model instance;
    public AbstractEnvironment environment { get; set; } //allows for the environment to be changed by the program if needed
    private float timeScaleFactor; //variable that scales the "time" of the simulation might be better to place this in a repo class later
    private Cell[] cells;

    private int cellIndex = 0;
    private int numCells;

    private Model()
    {
        //add code as it is needed
        environment = new Environment(); //super base case just to prevent any scary null pointers
        timeScaleFactor = 1;
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
    public int GetNumCells() 
    {
        return numCells;
    }

    //Simulates numCells many cells with iterations many steps each
    public void SimulateCells(int numCells, int iterations)
    {
        cells = new Cell[numCells];
        BacteriaFactory.SetCellIterations(iterations);
        this.numCells = numCells;
        cellIndex = 0;

        for(int i = 0; i < numCells; i++)
        {
            cells[i] = BacteriaFactory.CreateNewCell(Random.Range(-10.0F, 10.0F), Random.Range(-10.0F, 10.0F), Random.Range(0, 2 * Mathf.PI),false);
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
        cells[index] = BacteriaFactory.CreateNewCell(Random.Range(-10.0F, 10.0F), Random.Range(-10.0F, 10.0F), Random.Range(0, 2 * Mathf.PI), false);
    }

    public Cell[] GetCells()
    {
        return cells;
    }

    //Returns the "next" cell is used by the movement class to connect a cell object to a given e-coli object
    public Cell GetCell()
    {
        Cell cell = cells [cellIndex];
        cellIndex = cellIndex + 1 > numCells - 1 ? numCells - 1 : cellIndex + 1;
        return cell;
    }
    
}
