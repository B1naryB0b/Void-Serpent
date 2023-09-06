using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DynamicBackgroundGenerator))]
public class DynamicBackgroundGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DynamicBackgroundGenerator generator = (DynamicBackgroundGenerator)target;

        if (GUILayout.Button("Apply"))
        {
            generator.GenerateBackground();
        }
    }
}
