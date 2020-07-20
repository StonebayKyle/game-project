using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorInspector : Editor
{

    private TerrainGenerator generator;

    private void OnEnable()
    {
        generator = target as TerrainGenerator;

        generator.UpdateReadOnlyValues();
        Undo.undoRedoPerformed += RefreshCreator;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }

    private void RefreshCreator()
    {
        generator.UpdateReadOnlyValues();
        if (Application.isPlaying)
        {
            generator.EditorUpdate();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            RefreshCreator();
        }

        if (GUILayout.Button("Randomize Offsets")) {
            Undo.RecordObject(generator, "Randomize Offsets");
            generator.RandomizeOffsets();
            EditorUtility.SetDirty(generator);
            RefreshCreator();
        }
    }
}