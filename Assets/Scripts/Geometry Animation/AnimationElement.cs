using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationElement
{
    public enum ElementType
    {
        None,
        Color,
        Transform,
        Displace,
        Trace,
        Draw
    }

    public AnimationCurve curve;

}

[System.Serializable]
public class ColorAnimationElement : AnimationElement
{
    public ElementType type { get; private set; } = ElementType.Color;
    public Color startColor;
    public Color endColor;
}

[System.Serializable]
public class TransformAnimationElement : AnimationElement
{
    public ElementType type { get; private set; } = ElementType.Transform;
    public Transform startTransform;
    public Transform endTransform;
}

[System.Serializable]
public class DisplaceAnimationElement : AnimationElement
{
    public ElementType type { get; private set; } = ElementType.Displace;
    public Vector3 startPosition;
    public Vector3 endPosition;
}

[System.Serializable]
public class TraceAnimationElement : AnimationElement
{
    public ElementType type { get; private set; } = ElementType.Trace;
    public LineRenderer line;
    //public Vector3[] lineVerticies = line.GetPositions;
}

[System.Serializable]
public class DrawAnimationElement : AnimationElement
{
    public ElementType type { get; private set; } = ElementType.Draw;

}

