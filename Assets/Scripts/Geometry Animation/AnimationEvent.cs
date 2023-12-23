using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[CreateAssetMenu]
public class AnimationEvent : ScriptableObject
{
    public List<AnimationElement> animationElements;

    public void TriggerEvent(GameObject triggeredObject, AnimationEventManager animationEventManager)
    {

        Debug.Log($"triggeredObject: {(triggeredObject != null ? "Valid" : "Null")}");
        Debug.Log($"animationEventManager: {(animationEventManager != null ? "Valid" : "Null")}");
        Debug.Log($"animationElements Count: {animationElements?.Count}");

        animationEventManager.QueAnimationEvent(triggeredObject, animationElements);
    }
}


