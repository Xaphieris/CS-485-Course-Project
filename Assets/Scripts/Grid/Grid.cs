
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;

    public  Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];

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

    private Vector3 GetAbsolutePosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }
}
