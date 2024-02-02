/*using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

[CustomEditor(typeof(AnimationEvent))]
public class AnimationEventEditor : Editor
{
    private ElementType selectedType = ElementType.None;
    private List<bool> foldoutStates = new List<bool>();

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        GUIStyle coloredBackgroundStyle = new GUIStyle(GUI.skin.box);
        coloredBackgroundStyle.normal.background = MakeTex(2, 2, new Color(0.8f, 0.8f, 0.8f)); // Light gray color

        AnimationEvent animationEvent = (AnimationEvent)target;
        selectedType = (ElementType)EditorGUILayout.EnumPopup("Add Animation Element Type", selectedType);

        if (GUILayout.Button("Add Animation Element"))
        {
            AddAnimationElement(animationEvent, selectedType);
            foldoutStates.Add(true); // Assume new elements are unfolded by default
        }

        if (animationEvent.animationElements != null)
        {
            while (foldoutStates.Count < animationEvent.animationElements.Count)
                foldoutStates.Add(false); // Ensure foldoutStates list matches the number of elements

            for (int i = 0; i < animationEvent.animationElements.Count; i++)
            {
                AnimationElement element = animationEvent.animationElements[i];
                if (element != null)
                {
                    foldoutStates[i] = EditorGUILayout.Foldout(foldoutStates[i], "Element " + (i + 1).ToString(), true);
                    if (foldoutStates[i])
                    {
                        // Apply the colored background style
                        GUILayout.BeginVertical(coloredBackgroundStyle);
                        DisplayElementProperties(element);
                        GUILayout.EndVertical();
                    }
                }
            }

        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(animationEvent);
        }
    }

    private void DisplayElementProperties(AnimationElement element)
    {
        element.isLooped = EditorGUILayout.Toggle("Is Looped", element.isLooped);
        element.animationDuration = EditorGUILayout.FloatField("Duration", element.animationDuration);

        if (element.curve == null)
            element.curve = new AnimationCurve();
        element.curve = EditorGUILayout.CurveField("Animation Curve", element.curve);

        if (element is ColorAnimationElement colorElement)
        {
            colorElement.startColor = EditorGUILayout.ColorField("Start Color", colorElement.startColor);
            colorElement.endColor = EditorGUILayout.ColorField("End Color", colorElement.endColor);
        }
        else if (element is TransformAnimationElement transformElement)
        {
            transformElement.startTransform = (Transform)EditorGUILayout.ObjectField("Start Transform", transformElement.startTransform, typeof(Transform), true);
            transformElement.endTransform = (Transform)EditorGUILayout.ObjectField("End Transform", transformElement.endTransform, typeof(Transform), true);
        }
        else if (element is DisplaceAnimationElement displaceElement)
        {
            displaceElement.line = (LineRenderer)EditorGUILayout.ObjectField("Line Renderer", displaceElement.line, typeof(LineRenderer), true);
        }
        else if (element is TraceAnimationElement traceElement)
        {
            traceElement.line = (LineRenderer)EditorGUILayout.ObjectField("Line Renderer", traceElement.line, typeof(LineRenderer), true);
        }
        else if (element is DrawAnimationElement drawElement)
        {
            drawElement.line = (LineRenderer)EditorGUILayout.ObjectField("Line Renderer", drawElement.line, typeof(LineRenderer), true);
            drawElement.isEraseLine = EditorGUILayout.Toggle("Is Erase Line", drawElement.isEraseLine);
            drawElement.drawEraseSeparation = EditorGUILayout.FloatField("Draw Erase Separation", drawElement.drawEraseSeparation);
            drawElement.eraseCurve = EditorGUILayout.CurveField("Erase Curve", drawElement.eraseCurve);
        }
    }



    private void AddAnimationElement(AnimationEvent animationEvent, ElementType type)
    {
        if (animationEvent == null)
        {
            Debug.LogError("AnimationEvent reference is null.");
            return;
        }

        if (animationEvent.animationElements == null)
        {
            animationEvent.animationElements = new List<AnimationElement>();
        }

        switch (type)
        {
            case ElementType.Color:
                animationEvent.animationElements.Add(new ColorAnimationElement());
                break;
            case ElementType.Transform:
                animationEvent.animationElements.Add(new TransformAnimationElement());
                break;
            case ElementType.Displace:
                animationEvent.animationElements.Add(new DisplaceAnimationElement());
                break;
            case ElementType.Trace:
                animationEvent.animationElements.Add(new TraceAnimationElement());
                break;
            case ElementType.Draw:
                animationEvent.animationElements.Add(new DrawAnimationElement());
                break;
            case ElementType.None:
                // Optionally handle the 'None' case if needed
                break;
            default:
                Debug.LogWarning("Unhandled ElementType: " + type);
                break;
        }

        EditorUtility.SetDirty(animationEvent);
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }


}
*/