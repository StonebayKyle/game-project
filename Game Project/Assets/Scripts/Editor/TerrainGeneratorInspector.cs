using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorInspector : Editor
{

    private TerrainGenerator generator;

    private void OnEnable()
    {
        generator = target as TerrainGenerator;
        Undo.undoRedoPerformed += RefreshCreator;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }

    private void RefreshCreator()
    {
        if (Application.isPlaying)
        {
            generator.EditorUpdate();
        }
    }

    public override void OnInspectorGUI()
    {
        //update
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            RefreshCreator();
        }

        if (GUILayout.Button("Randomize Offsets")) {
            generator.RandomizeOffsets();
            RefreshCreator();
        }
    }
}