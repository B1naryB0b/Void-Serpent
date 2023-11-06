using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveGenerator)), CanEditMultipleObjects]
public class WaveEditor : Editor
{
    SerializedProperty autoUpdateProperty; // For the auto-update functionality
    SerializedProperty frequencyProperty;
    SerializedProperty amplitudeProperty;
    SerializedProperty wavelengthProperty;
    SerializedProperty phaseDifferenceProperty;
    SerializedProperty totalLengthProperty;
    SerializedProperty resolutionProperty;
    SerializedProperty lineWidthProperty; // Ensure this is defined in the WaveGenerator class

    private void OnEnable()
    {
        autoUpdateProperty = serializedObject.FindProperty("autoUpdate");
        frequencyProperty = serializedObject.FindProperty("frequency");
        amplitudeProperty = serializedObject.FindProperty("amplitude");
        wavelengthProperty = serializedObject.FindProperty("wavelength");
        phaseDifferenceProperty = serializedObject.FindProperty("phaseDifference");
        totalLengthProperty = serializedObject.FindProperty("totalLength");
        resolutionProperty = serializedObject.FindProperty("resolution");
        lineWidthProperty = serializedObject.FindProperty("lineWidth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(autoUpdateProperty, new GUIContent("Auto Update"));
        EditorGUILayout.Slider(frequencyProperty, 0, 10, new GUIContent("Frequency"));
        EditorGUILayout.PropertyField(amplitudeProperty);
        EditorGUILayout.PropertyField(wavelengthProperty);
        EditorGUILayout.Slider(phaseDifferenceProperty, 0, 2 * Mathf.PI, new GUIContent("Phase Difference"));
        EditorGUILayout.Slider(totalLengthProperty, 1, 1000, new GUIContent("Total Length"));
        EditorGUILayout.IntSlider(resolutionProperty, 1, 100, new GUIContent("Resolution"));
        EditorGUILayout.PropertyField(lineWidthProperty);

        // Apply the modified properties
        if (serializedObject.ApplyModifiedProperties() && autoUpdateProperty.boolValue)
        {
            // Call GenerateWave on each selected WaveGenerator object
            foreach (Object targetObject in serializedObject.targetObjects)
            {
                WaveGenerator waveGenerator = (WaveGenerator)targetObject;
                if (waveGenerator != null)
                {
                    waveGenerator.GenerateWave();
                }
            }
        }

        if (!autoUpdateProperty.boolValue && GUILayout.Button("Generate Wave"))
        {
            // Loop through all selected WaveGenerator objects and call GenerateWave on each
            foreach (Object targetObject in serializedObject.targetObjects)
            {
                WaveGenerator waveGenerator = (WaveGenerator)targetObject;
                if (waveGenerator != null)
                {
                    waveGenerator.GenerateWave();
                }
            }
        }
    }

}
