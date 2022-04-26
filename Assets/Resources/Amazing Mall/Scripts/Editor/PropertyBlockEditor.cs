using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PropertyBlock))]
[CanEditMultipleObjects]
public class PropertyBlockEditor : Editor
{
    SerializedProperty _renderers, _materialname, _blocks;

    private void OnEnable()
    {
        _renderers = serializedObject.FindProperty("_Renderer");
        _materialname = serializedObject.FindProperty("MaterialName");
        _blocks = serializedObject.FindProperty("Blocks");
    }

    public override void OnInspectorGUI()
    {
   

        PropertyBlock script = (PropertyBlock)target;
        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(_renderers, new GUIContent("Renderers"));



        EditorGUILayout.PropertyField(_materialname, new GUIContent("Material Name"));
        EditorGUILayout.PropertyField(_blocks, new GUIContent("Material Blocks"));

        serializedObject.ApplyModifiedProperties();
    }

}
