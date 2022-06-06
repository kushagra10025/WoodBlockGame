using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Block Identifier", menuName ="Wood Block Game/New Block Identifier")]
public class BlockIdentifier : ScriptableObject
{
    public int columns = 0;
    public int rows = 0;
    public Row[] board;

    /// <summary>
    /// Delete All the Rows and its columns data in the board
    /// </summary>
    public void Clear()
    {
        for(int i = 0; i< rows; i++)
        {
            board[i].ClearRow();
        }
    }

    /// <summary>
    /// Create new board of Row x Columns Size
    /// Each Row will be of Column Size
    /// </summary>
    public void CreateNewBoard()
    {
        board = new Row[rows];
        for(int i = 0; i<rows; i++)
        {
            board[i] = new Row(columns);
        }
    }

    [System.Serializable]
    public class Row
    {
        public bool[] column;
        private int _size = 0;

        public Row() { }
        public Row(int size)
        {
            CreateRow(size);
        }

        /// <summary>
        /// Create Row of with specified column count
        /// Clear it to start from fresh slate
        /// </summary>
        /// <param name="size"></param>
        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        /// <summary>
        /// Delete/Deactive all columns
        /// </summary>
        public void ClearRow()
        {
            for(int i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

}
