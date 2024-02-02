using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParallaxBackgroundGenerator))]
public class ParallaxBackgroundGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ParallaxBackgroundGenerator generator = (ParallaxBackgroundGenerator)target;

        if (GUILayout.Button("Apply"))
        {
            generator.GenerateBackground();
        }
    }
}
