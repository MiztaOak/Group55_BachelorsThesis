using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapVisual : MonoBehaviour, GridListeners
{
    //based on the turorial located at: https://www.youtube.com/watch?v=mZzZXfySeFQ
    private Grid grid;
    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();

        //Color color = this.GetComponent<MeshRenderer>().material.color;
        //color.a = 0.5f;
        //this.GetComponent<MeshRenderer>().material.color = color;
    }

    private void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] verticies, out Vector2[] uv, out int[] triangels);

        for(int x = 0; x < grid.GetWidth(); x++) //creates the meshes based on the grid
        {
            for(int z = 0; z < grid.GetHeight(); z++)
            {
                int index = x * grid.GetHeight() + z;
                Vector3 quadSize = new Vector3(1, 0, 1) * grid.GetCellSize();

                float gridValue = grid.GetValue(x, z);

                Vector2 gridValueUV = new Vector2(gridValue, 0f);

                MeshUtils.AddToMeshArrays(verticies, uv, triangels, index, grid.GetPostion(x, z) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
            }
        }

        mesh.vertices = verticies;
        mesh.uv = uv;
        mesh.triangles = triangels;
    }

    public void OnGridUpdate()
    {
        UpdateHeatMapVisual();
    }
}
