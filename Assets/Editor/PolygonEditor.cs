using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolygonGenerator))]
public class PolygonEditor : Editor
{
    SerializedProperty sidesProperty;
    SerializedProperty radiusProperty;
    SerializedProperty widthProperty;

    private void OnEnable()
    {
        sidesProperty = serializedObject.FindProperty("sides");
        radiusProperty = serializedObject.FindProperty("radius");
        widthProperty = serializedObject.FindProperty("lineWidth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.IntSlider(sidesProperty, 3, 360, new GUIContent("Number of Sides"));
        EditorGUILayout.Slider(radiusProperty, 0.5f, 10f, new GUIContent("Radius"));
        EditorGUILayout.Slider(widthProperty, 0.1f, 1f, new GUIContent("Line Width"));

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Generate Polygon"))
        {
            ((PolygonGenerator)target).GeneratePolygon();
        }
    }
}