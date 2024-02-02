using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    None,
    Color,
    Transform,
    Displace,
    Trace,
    Draw
}

[System.Serializable]
public struct TransformComponent
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public TransformComponent(Vector3 position = default, Quaternion rotation = default, Vector3 localScale = default)
    {
        this.position = position == default ? Vector3.zero : position;
        this.rotation = rotation == default ? Quaternion.identity : rotation;
        this.localScale = localScale == default ? Vector3.one : localScale;
    }
}

[System.Serializable]
public abstract class AnimationElement
{
    public float animationOrder;
    public bool isLooped;
    public float animationDuration;
    public AnimationCurve curve;
}

[System.Serializable]
public class ColorAnimationElement : AnimationElement
{
    public ElementType type => ElementType.Color;
    public Color startColor;
    public Color endColor;
}

[System.Serializable]
public class TransformAnimationElement : AnimationElement
{
    public ElementType type => ElementType.Transform;
    public TransformComponent startTransform;
    public TransformComponent endTransform;
}

[System.Serializable]
public class DisplaceAnimationElement : AnimationElement
{
    public ElementType type => ElementType.Displace;
    public LineRenderer line;
    [HideInInspector] public Vector3[] lineVertices { get; private set; }
    public Vector3[] vertexDisplacement;

    public void InitializeLineVertices()
    {
        if (line != null)
        {
            lineVertices = new Vector3[line.positionCount];
            vertexDisplacement = new Vector3[line.positionCount];

            line.GetPositions(lineVertices);
        }
    }
}

[System.Serializable]
public class TraceAnimationElement : AnimationElement
{
    public ElementType type => ElementType.Trace;
    public LineRenderer line;
    [HideInInspector] public Vector3[] lineVertices { get; private set; }

    public void InitializeLineVertices()
    {
        if (line != null)
        {
            lineVertices = new Vector3[line.positionCount];
            line.GetPositions(lineVertices);
        }
    }
}

[System.Serializable]
public class DrawAnimationElement : AnimationElement
{
    public ElementType type => ElementType.Draw;
    public LineRenderer line;
    public bool isEraseLine;
    public float drawEraseSeparation;
    public AnimationCurve eraseCurve;

}

