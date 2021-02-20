using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap : MonoBehaviour
{
    [SerializeField] private HeatmapVisual heatmapVisual;
    public int width;
    public int height;
    public float cellSize;

    public float d, i_0;

    private Grid grid;

    private void Start()
    {
        EnvironmentFactory.CreateBasicEnvionment(d, i_0, transform.position.x, transform.position.z);

        Vector3 heatmapPosition = transform.position - new Vector3(width*cellSize*.5f, transform.position.y+1, height*cellSize* .5f);
        grid = new Grid(width,height,cellSize,heatmapPosition);

        heatmapVisual.SetGrid(grid);
    }
    
}
