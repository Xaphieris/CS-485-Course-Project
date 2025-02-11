
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

public class Grid
{
    //Width Count
    private int width;
    //Height Count
    private int height;
    //Physical Cell Size
    private float cellSize;
    //Grid containting all cells of the grid, width by height
    private int[,] gridArray;
    private bool boolDebug = true;

    public GameObject Tile;

    public GridDraw gridDraw = new GridDraw();

    public  Grid(int width, int height, float cellSize)
    {
        gridDraw.width = width;
        gridDraw.height = height; 
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        //Debug display grid
        if(boolDebug == true)
        {
            for(float i = -.5f; i < gridArray.GetLength(0); i++)
            {
                for(float j = -.5f; j < gridArray.GetLength(1); j++)
                {
                    //Debug.Log(i + " , " + j);
                    Debug.DrawLine(GetAbsolutePosition(i, j), GetAbsolutePosition(i, j + 1), Color.white, 100F);
                    Debug.DrawLine(GetAbsolutePosition(i, j), GetAbsolutePosition(i + 1, j), Color.white, 100F);
                }
            }
        } 
    }

    //Get the position of the specified grid cell
    private Vector3 GetAbsolutePosition(float x, float z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }
}
