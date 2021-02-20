using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    //based on the tutorial located at: https://www.youtube.com/watch?v=waEsGu--9P8&t=0s

    private int width;
    private int height;
    private float[,] gridArray;
    private float cellSize;
    private Vector3 origin;

    private float minX, minZ, maxX, maxZ;

    private List<GridListeners> listeners = new List<GridListeners>();  //list of objects to notify when stuff changes will have to be used when the c changes over time

    public Grid(int width, int height, float cellSize, Vector3 origin, float minX, float maxX, float minZ, float maxZ)
    {
        this.width = width;
        this.height = height;
        this.origin = origin;
        this.cellSize = cellSize;
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;

        gridArray = new float[width, height];

        PopulateGrid();
    }

    //simple standard constructor to be used for the current setup
    public Grid(int width, int height, float cellSize) : this(width, height, cellSize, new Vector3(-width * cellSize * .5f,1,-height*cellSize*.5f), -20f, 20f, -20f, 20f) { } 

    private void PopulateGrid() //populates the grid with the consentrations for the different "squares"
    {
        Model model = Model.GetInstance();
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Vector3 pos = GetPostion(x, z);
               if(pos.x < maxX && pos.x > minX && pos.z < maxZ && pos.z > minZ)
                    gridArray[x,z] = model.environment.getConcentration(pos.x + cellSize * .5f, pos.z + cellSize * .5f); 
            }
        }

        
    }

    public Vector3 GetPostion(int x, int z)     //gets the world position for index x and z
    {
        return new Vector3(x,0,z) * cellSize + origin;
    }

    private void GetXZ(Vector3 pos, out int x, out int z)   //gets the indexes x and z for the given world position
    {
        x = Mathf.FloorToInt((pos - origin).x / cellSize);
        z = Mathf.FloorToInt((pos - origin).z / cellSize);
    }

    public float GetValue(int x, int z)     //gets the value for the indexes x and z
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return 0;
        }
           
    }

    public float GetValue(Vector3 pos)      //gets the value for the given world position
    {
        int x, z;
        GetXZ(pos, out x, out z);
        return GetValue(x, z);
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public void Subscribe(GridListeners listener)
    {
        listeners.Add(listener);
    }

    public void Notify()
    {
        foreach(GridListeners listener in listeners)
        {
            listener.OnGridUpdate();
        }
    }
}
