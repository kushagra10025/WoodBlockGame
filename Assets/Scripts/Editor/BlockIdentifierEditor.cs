using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockIdentifier), false)]
[CanEditMultipleObjects]
public class BlockIdentifierEditor : Editor
{
    private BlockIdentifier BlockIdentifierInstance => target as BlockIdentifier;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        // Create Clear Board Button
        ClearBoardButton();
        EditorGUILayout.Space();
        // Draw Columns Input Fields
        DrawColumnsInputFields();
        EditorGUILayout.Space();
        // Create the Board
        if(BlockIdentifierInstance.board != null && BlockIdentifierInstance.columns > 0 && BlockIdentifierInstance.rows > 0)
        {
            DrawBoardTable();
        }

        // Apply the values within the BlockIdentifierInstance.
        serializedObject.ApplyModifiedProperties();

        // Update the BlockIdentifierInstance with the values from the inspector.
        if (GUI.changed)
        {
            EditorUtility.SetDirty(BlockIdentifierInstance);
        }
    }

    private void ClearBoardButton()
    {
        // Clear all the values from the Board
        if(GUILayout.Button("Clear Board"))
        {
            BlockIdentifierInstance.Clear();
        }
    }

    private void DrawColumnsInputFields()
    {
        int colTemp = BlockIdentifierInstance.columns;
        int rowTemp = BlockIdentifierInstance.rows;

        // Verify that columns and rows count are > 0 and exist and then create a new board based on the same size.
        BlockIdentifierInstance.columns = EditorGUILayout.IntField("Columns", BlockIdentifierInstance.columns);
        BlockIdentifierInstance.rows = EditorGUILayout.IntField("Rows", BlockIdentifierInstance.rows);

        if((BlockIdentifierInstance.columns != colTemp || BlockIdentifierInstance.rows != rowTemp) &&
            (BlockIdentifierInstance.columns > 0 && BlockIdentifierInstance.rows > 0))
        {
            BlockIdentifierInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        // Custom Table Formatting for Storing the Pattern Binding Buttons
        var tableStyle = new GUIStyle("box");
        tableStyle.padding = new RectOffset(10, 10, 10, 10);
        tableStyle.margin.left = 32;

        // Custom Column Formatting Placing the Pattern Binding Buttons
        var columnStyle = new GUIStyle();
        columnStyle.fixedWidth = 65;
        columnStyle.fixedHeight = 25;
        columnStyle.alignment = TextAnchor.MiddleCenter;

        // Custom Row Formatting Placing the Pattern Binding Buttons
        var rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.fixedWidth = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        // Custom Button Formatting for Pattern Binding Buttons
        var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        // Initialize the Buttons based on rows and columns count
        for(int row = 0; row < BlockIdentifierInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(columnStyle);
            for(int col = 0; col < BlockIdentifierInstance.columns; col++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                bool data = EditorGUILayout.Toggle(BlockIdentifierInstance.board[row].column[col], dataFieldStyle);
                BlockIdentifierInstance.board[row].column[col] = data;
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
