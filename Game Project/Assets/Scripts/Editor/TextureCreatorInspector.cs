using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextureCreator), editorForChildClasses:true)]
public class TextureCreatorInspector : Editor
{

    private TextureCreator creator;

    private void OnEnable()
    {
        creator = target as TextureCreator;
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
            creator.Refresh();
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

        if (GUILayout.Button("Randomize Offsets"))
        {
            Undo.RecordObject(creator, "Randomize Offsets");
            creator.RandomizeOffsets();
            EditorUtility.SetDirty(creator);
            RefreshCreator();
        }

        if (GUILayout.Button("Randomize Rotation"))
        {
            Undo.RecordObject(creator, "Randomize Rotation");
            creator.RandomizeRotation();
            EditorUtility.SetDirty(creator);
            RefreshCreator();
        }
    }
}