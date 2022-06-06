using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineHelper))]
public class Grid : MonoBehaviour
{
    public BlockLoader blockLoader;

    public int columns = 0;
    public int rows = 0;
    public float cellGap = 0.1f;
    public GameObject cellSquarePrefab;
    public Vector2 startPosition = Vector2.zero;
    public float cellScale = 0.5f;
    public float cellOffset = 0.0f;

    private Vector2 _offset = Vector2.zero;
    private List<GameObject> _cellSquares = new List<GameObject>();
    private LineHelper _lineHelper;
    private int _placedBlockScore = 0;

    /// <summary>
    /// Subscribe to Check if block can be placed
    /// </summary>
    private void OnEnable()
    {
        GameEvents.CheckIfBlockCanBePlaced += CheckIfBlockCanBePlaced;
    }

    /// <summary>
    /// Subscribe to Check if block can be placed
    /// </summary>
    private void OnDisable()
    {
        GameEvents.CheckIfBlockCanBePlaced -= CheckIfBlockCanBePlaced;
    }

    /// <summary>
    /// Get Components
    /// </summary>
    private void Awake()
    {
        _lineHelper = GetComponent<LineHelper>();
    }

    /// <summary>
    /// Create Grid at Game Start and Initialize Vibration System
    /// </summary>
    private void Start()
    {
        Vibration.Init();
        CreateGrid();
    }

    /// <summary>
    /// Create Grid by spawning cells and settings their positions
    /// </summary>
    private void CreateGrid()
    {
        SpawnCells();
        SetCellPositions();
    }

    /// <summary>
    /// Set Cell Position, 10 in each row and 10 in each column
    /// </summary>
    private void SetCellPositions()
    {
        int colNumber = 0;
        int rowNumber = 0;
        Vector2 cellGapNumber = Vector2.zero;
        bool rowMoved = false;
        var cellRect = _cellSquares[0].GetComponent<RectTransform>();

        _offset.x = cellRect.rect.width * cellRect.transform.localScale.x + cellOffset;
        _offset.y = cellRect.rect.height * cellRect.transform.localScale.y + cellOffset;

        foreach(var cell in _cellSquares)
        {
            if(colNumber+1 > columns)
            {
                cellGapNumber.x = 0;
                // Goto Next Column
                colNumber = 0;
                rowNumber++;
                rowMoved = false;
            }

            var posXOffset = _offset.x * colNumber + (cellGapNumber.x * cellGap);
            var posYOffset = _offset.y * rowNumber + (cellGapNumber.y * cellGap);

            if(colNumber > 0 && colNumber % 3 == 0)
            {
                cellGapNumber.x++;
                posXOffset += cellGap;
            }

            if(rowNumber > 0 && rowNumber % 3 == 0 && !rowMoved)
            {
                // Row has changed
                rowMoved = true;
                cellGapNumber.y++;
                posYOffset += cellGap;
            }

            cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + posXOffset, startPosition.y - posYOffset);
            cell.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + posXOffset, startPosition.y - posYOffset, 0.0f);

            colNumber++;
        }
    }

    /// <summary>
    /// Instantiate Cells (10x10)
    /// </summary>
    private void SpawnCells()
    {
        int cellIndex = 0;

        for(int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject cell = GameObject.Instantiate(cellSquarePrefab);
                cell.GetComponent<GridCell>().CellIndex = cellIndex;
                cell.transform.SetParent(this.transform);
                cell.transform.localScale = new Vector3(cellScale, cellScale, cellScale);
                /*cell.GetComponent<GridCell>().SetImage(cellIndex % 2 == 0);*/
                cell.GetComponent<GridCell>().SetImage();
                _cellSquares.Add(cell);
                cellIndex++;
            }
        }
    }

    /// <summary>
    /// If Block can be placed
    ///     Place on Board
    ///     Check if any line is completed
    /// Else
    ///     Move to Default Place
    /// </summary>
    private void CheckIfBlockCanBePlaced()
    {
        _placedBlockScore = 0;
        List<int> cellIndexes = new List<int>();
        foreach(var cell in _cellSquares)
        {
            var gridCell = cell.GetComponent<GridCell>();

            if(gridCell.Selected && !gridCell.CellOccupied)
            {
                cellIndexes.Add(gridCell.CellIndex);
                gridCell.Selected = false;
            }
        }

        Block currentSelectedBlock = blockLoader.GetCurrentSelectedBlock();
        if (currentSelectedBlock == null) return;

        if(currentSelectedBlock.TotalCellCount == cellIndexes.Count)
        {
            foreach(int cellIndex in cellIndexes)
            {
                _placedBlockScore++;
                Vibration.VibratePop();
                _cellSquares[cellIndex].GetComponent<GridCell>().PlaceBlockOnBoard();
            }

            int blockLeft = 0;

            foreach(var block in blockLoader.blocks)
            {
                if(block.IsOnStartPosition() && block.IsAnyOfBlockCellActive())
                {
                    blockLeft++;
                }
            }

            if(blockLeft == 0)
            {
                GameEvents.RequestNewBlock();
                Vibration.VibrateNope();
            }
            else
            {
                GameEvents.SetBlockInactive();
            }

            CheckIfAnyLineCompleted();
        }
        else
        {
            GameEvents.MoveBlockToInitialPosition();
            Vibration.VibratePeek();
        }
    }

    /// <summary>
    /// Retrieve all Rows and Cols and Indexes and Check Score based on whether any row or column is completed.
    /// </summary>
    private void CheckIfAnyLineCompleted()
    {
        List<int[]> lines = new List<int[]>();

        // Cols
        foreach(var column in _lineHelper.colIndexes)
        {
            lines.Add(_lineHelper.GetVerticalLine(column));
        }

        // Rows
        foreach (var row in _lineHelper.rowIndexes)
        {
            lines.Add(_lineHelper.GetHorizontalLine(row));
        }

        var completedLines = CheckIfCellCompleted(lines);

        if(completedLines > 2)
        {
            // Play Bonus Vibration
            long[] pattern = { 0, 100, 100, 100, 100};
            Vibration.Vibrate(pattern, -1);
        }

        var completedLineScore = 10 * completedLines;
        var totalScore = completedLineScore + _placedBlockScore;
        GameEvents.AddScores(totalScore);
        CheckIfPlayerLost();
    }

    /// <summary>
    /// Check if all cells is completed for the given Line
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private int CheckIfCellCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();

        var linesCompleted = 0;

        foreach(var line in data)
        {
            var lineCompleted = true;
            foreach(var cellIndex in line)
            {
                var comp = _cellSquares[cellIndex].GetComponent<GridCell>();
                if (!comp.CellOccupied)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach(var completedLine in completedLines)
        {
            var completed = false;
            foreach(var cellIndex in completedLine)
            {
                var comp = _cellSquares[cellIndex].GetComponent<GridCell>();
                comp.DeactivateCell();
                completed = true;
            }

            foreach (var cellIndex in completedLine)
            {
                var comp = _cellSquares[cellIndex].GetComponent<GridCell>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }

        return linesCompleted;
    }

    /// <summary>
    /// Check If Player Lost
    /// </summary>
    private void CheckIfPlayerLost()
    {
        int validShapes = 0;
        for(var index = 0; index < blockLoader.blocks.Count; index++)
        {
            bool isShapeActive = blockLoader.blocks[index].IsAnyOfBlockCellActive();
            if (CheckIfBlockCanBePlacedOnGrid(blockLoader.blocks[index]) && isShapeActive)
            {
                blockLoader.blocks[index]?.ActivateBlock();
                validShapes++;
            }
        }

        if(validShapes == 0)
        {
            // Game Over
            GameEvents.GameOver();
        }
    }

    /// <summary>
    /// Check whether the placement is a valid move or not
    /// </summary>
    /// <param name="currentBlock"></param>
    /// <returns></returns>
    private bool CheckIfBlockCanBePlacedOnGrid(Block currentBlock)
    {
        var currentBlockIdentifier = currentBlock.currentBlockIdentifier;
        var blockColumns = currentBlockIdentifier.columns;
        var blockRows = currentBlockIdentifier.rows;

        List<int> originalShapeFilledUpCells = new List<int>();
        var cellIndex = 0;

        for(var rowIndex = 0; rowIndex < blockRows; rowIndex++)
        {
            for (var colIndex = 0; colIndex < blockColumns; colIndex++)
            {
                if (currentBlockIdentifier.board[rowIndex].column[colIndex])
                {
                    originalShapeFilledUpCells.Add(cellIndex);
                }
                cellIndex++;
            }
        }

        if(currentBlock.TotalCellCount != originalShapeFilledUpCells.Count)
        {
            Debug.LogError("Number of filled up Cells != Original Shape");
        }

        var cellList = GetAllCellCombination(blockColumns, blockRows);

        bool canBePlaced = false;

        foreach(var cell in cellList)
        {
            bool blockCanBePlacedOnTheBoard = true;
            foreach(var blockIndexToCheck in originalShapeFilledUpCells)
            {
                var comp = _cellSquares[cell[blockIndexToCheck]].GetComponent<GridCell>();
                if (comp.CellOccupied)
                {
                    blockCanBePlacedOnTheBoard = false;
                }
            }

            if (blockCanBePlacedOnTheBoard)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    /// <summary>
    /// Get all possible placements of a block, helps in determining game over condition
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="rows"></param>
    /// <returns></returns>
    private List<int[]> GetAllCellCombination(int columns, int rows)
    {
        List<int[]> cellList = new List<int[]>();

        int lastColIndex = 0;
        int lastRowIndex = 0;
        int safeIndex = 0;

        while(lastRowIndex + (rows-1) < 10)
        {
            var rowData = new List<int>();
            for(var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var col = lastColIndex; col < lastColIndex + columns; col++)
                {
                    rowData.Add(_lineHelper.GetCellData(row, col));
                }
            }

            cellList.Add(rowData.ToArray());
            lastColIndex++;
            if(lastColIndex + (columns - 1) >= 10)
            {
                lastRowIndex++;
                lastColIndex = 0;
            }

            safeIndex++;
            if(safeIndex > 150)
            {
                break;
            }
        }

        return cellList;
    }

}
