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

    //public int areaWidth, areaHight;

    private Grid grid;

    private void Start()
    {
        Vector3 position = transform.position;
        EnvironmentFactory.CreateBasicEnvionment(d, i_0, position.x, position.z);

        //Vector3 heatmapPosition = transform.position - new Vector3(width*cellSize*.5f, position.y+1, height*cellSize* .5f); //calculates the position based on the pos of the food object
        //float minX = position.x - areaWidth / 2, maxX = position.x + areaWidth / 2, minZ = position.z - areaHight / 2, maxZ = position.z + areaHight / 2;
        //grid = new Grid(width,height,cellSize,heatmapPosition,minX,maxX,minZ,maxZ);

        grid = new Grid(width, height, cellSize);

        heatmapVisual.SetGrid(grid); //sends the grid to the heatmapVisual class     
    }

    
    

}
