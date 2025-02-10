
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

    public  Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height; 
        this.cellSize = cellSize;

        gridArray = new int[width, height];

        //Debug display grid
        for(int i = 0; i < gridArray.GetLength(0); i++)
        {
            for(int j = 0;j < gridArray.GetLength(1); j++)
            {
                //Debug.Log(i + " , " + j);
                Debug.DrawLine(GetAbsolutePosition(i, j), GetAbsolutePosition(i, j + 1), Color.white, 100F);
                Debug.DrawLine(GetAbsolutePosition(i, j), GetAbsolutePosition(i + 1, j), Color.white, 100F);
            }
        }
        
    }

    //Get the position of the specified grid cell
    private Vector3 GetAbsolutePosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }
}
