using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineHelper : MonoBehaviour
{
    /// <summary>
    /// Same as Rows in Grid Class
    /// </summary>
    public int rows = 10;
    /// <summary>
    /// Same as Columns in Grid Class
    /// </summary>
    public int cols = 10;

    // TODO: AutoGenerate Array Depending on Number of Columns
    [HideInInspector]
    public int[] colIndexes = new int[10]
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9
    };

    // TODO: AutoGenerate Array Depending on Number of Rows
    [HideInInspector]
    public int[] rowIndexes = new int[10]
    {
        0, 10, 20, 30, 40, 50, 60, 70, 80, 90
    };

    /// <summary>
    /// For Cell Row and Column Get it's Index
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public int GetCellData(int row, int col)
    {
        int cellPosition;
        cellPosition = (row * 10) + col;
        return cellPosition;
    }

    /// <summary>
    /// For Cell Index Get it's Row and Column
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <returns></returns>
    public (int, int) GetCellPosition(int cellIndex)
    {
        int rowIndex = -1, colIndex = -1;

        colIndex = cellIndex % cols;
        rowIndex = Mathf.FloorToInt(cellIndex / rows);

        return (rowIndex, colIndex);
    }

    /// <summary>
    /// Get All Cells in a column for a particular cell index
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <returns></returns>
    public int[] GetVerticalLine(int cellIndex)
    {
        int[] line = new int[cols];

        int cellColumn = GetCellPosition(cellIndex).Item2;

        line[0] = cellColumn;
        for(int i = 1; i < cols; i++)
        {
            line[i] = cellColumn + 10;
            cellColumn += 10;
        }
        return line;
    }

    /// <summary>
    /// Get All Cells in a row for a particular cell index
    /// </summary>
    /// <param name="cellIndex"></param>
    /// <returns></returns>
    public int[] GetHorizontalLine(int cellIndex)
    {
        int[] line = new int[cols];

        int rowColumn = GetCellPosition(cellIndex).Item1;

        line[0] = rowColumn * 10;
        for (int i = 1; i < cols; i++)
        {
            line[i] = line[i-1] + 1;

        }
        return line;
    }
}
