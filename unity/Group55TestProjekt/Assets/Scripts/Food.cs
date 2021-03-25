using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Filters;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


public class Food : MonoBehaviour
{
    
    [SerializeField] private HeatmapVisual heatmapVisual;
    public int width;
    public int height;
    public float cellSize;

    public float d, i_0;

    private Grid grid;

    private void Start()
    {
        Vector3 position = transform.position;
     
        grid = new Grid(width, height, cellSize);

        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class     
    }

}
