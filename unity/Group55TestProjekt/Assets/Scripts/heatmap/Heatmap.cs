using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    [SerializeField] private HeatmapVisual heatmapVisual;
    public int width;
    public int height;
    public float cellSize;

    private Grid grid;

    private void Start()
    {
        EnvironmentFactory.CreateBasicEnvionment(20, 0, transform.position.x, transform.position.z);
        grid = new Grid(width,height,cellSize,new Vector3(0,0,0));

        heatmapVisual.SetGrid(grid);
    }
    
}
