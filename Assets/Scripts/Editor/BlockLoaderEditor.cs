using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockLoader))]
[CanEditMultipleObjects]
public class BlockLoaderEditor : Editor
{
    private BlockLoader BlockLoaderInstance => target as BlockLoader;

    public override void OnInspectorGUI()
    {
        // Create the Redraw Blocks Button only at Editor Runtime
        base.OnInspectorGUI();
        if (EditorApplication.isPlaying)
        {
            RedrawBlocks();
        }
    }

    private void RedrawBlocks()
    {
        // Allows Quick Check of Drawing random buttons
        if (GUILayout.Button("Redraw Blocks"))
        {
            BlockLoaderInstance.DrawRandomBlocks();
        }
    }
}
