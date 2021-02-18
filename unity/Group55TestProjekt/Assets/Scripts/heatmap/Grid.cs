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

    private List<GridListeners> listeners = new List<GridListeners>();

    public Grid(int width, int height, float cellSize, Vector3 origin)
    {
        this.width = width;
        this.height = height;
        this.origin = origin;
        this.cellSize = cellSize;

        gridArray = new float[width, height];

        PopulateGrid();
    }

    public Grid(int widht, int height, float cellSize) : this(widht, height, cellSize, Vector3.zero) { }

    public Grid(int widht, int height) : this(widht, height, 1, Vector3.zero) { }

    private void PopulateGrid()
    {
        Model model = Model.GetInstance();
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                Vector3 pos = GetPostion(x, z);
                gridArray[x,z] = Mathf.Pow(10,10)*model.environment.getConcentration(pos.x + cellSize * .5f, pos.z + cellSize * .5f); //problem here prob
            }
        }

        notify();
    }

    public Vector3 GetPostion(int x, int z)
    {
        return new Vector3(x,0,z) * cellSize + origin;
    }

    private void GetXZ(Vector3 pos, out int x, out int z)
    {
        x = Mathf.FloorToInt((pos - origin).x / cellSize);
        z = Mathf.FloorToInt((pos - origin).z / cellSize);
    }

    public float GetValue(int x, int z)
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

    public float GetValue(Vector3 pos)
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

    public void notify()
    {
        foreach(GridListeners listener in listeners)
        {
            listener.OnGridUpdate();
        }
    }
}
