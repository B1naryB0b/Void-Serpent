using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[CreateAssetMenu]
public class AnimationEvent : ScriptableObject
{
    public List<ColorAnimationElement> colorAnimationElements;
    public List<TransformAnimationElement> transformAnimationElements;
    public List<DisplaceAnimationElement> displaceAnimationElements;
    public List<TraceAnimationElement> traceAnimationElements;
    public List<DrawAnimationElement> drawAnimationElements;

    private List<AnimationElement> animationElements;

    public void TriggerEvent(GameObject triggeredObject, AnimationEventManager animationEventManager)
    {
        LoadAnimationElements();

        Debug.Log($"triggeredObject: {(triggeredObject != null ? "Valid" : "Null")}");
        Debug.Log($"animationEventManager: {(animationEventManager != null ? "Valid" : "Null")}");
        Debug.Log($"animationElements Count: {animationElements?.Count}");

        animationEventManager.QueAnimationEvent(triggeredObject, animationElements);
    }

    public void LoadAnimationElements()
    {
        if (animationElements == null)
            animationElements = new List<AnimationElement>();

        animationElements.Clear();

        animationElements.AddRange(colorAnimationElements);
        animationElements.AddRange(transformAnimationElements);
        animationElements.AddRange(displaceAnimationElements);
        animationElements.AddRange(traceAnimationElements);
        animationElements.AddRange(drawAnimationElements);

        animationElements.Sort((element1, element2) => element1.animationOrder.CompareTo(element2.animationOrder));
    }

}


