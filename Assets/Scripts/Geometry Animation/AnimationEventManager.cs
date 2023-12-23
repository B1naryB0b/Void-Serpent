using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public void QueAnimationEvent(GameObject triggeredObject, List<AnimationElement> animationElements)
    {
        foreach (var animationElement in animationElements)
        {
            if (animationElement is ColorAnimationElement colorElement)
            {
                HandleColorAnimationElement(colorElement, triggeredObject);
            }
            else if (animationElement is TransformAnimationElement transformElement)
            {
                HandleTransformAnimationElement(transformElement, triggeredObject);
            }
            else if (animationElement is DisplaceAnimationElement displaceElement)
            {
                HandleDisplaceAnimationElement(displaceElement, triggeredObject);
            }
            else if (animationElement is TraceAnimationElement traceElement)
            {
                HandleTraceAnimationElement(traceElement, triggeredObject);
            }
            else if (animationElement is DrawAnimationElement drawElement)
            {
                HandleDrawAnimationElement(drawElement, triggeredObject);
            }
            else
            {
                Debug.LogWarning("No animation element type selected.");
            }
        }
    }

    private void HandleColorAnimationElement(ColorAnimationElement element, GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            StartCoroutine(AnimateColor(spriteRenderer, element));
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on the object.");
        }
    }

    private IEnumerator AnimateColor(SpriteRenderer spriteRenderer, ColorAnimationElement element)
    {
        float elapsed = 0f;

        while (elapsed < element.animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = element.curve.Evaluate(elapsed / element.animationDuration);
            spriteRenderer.color = Color.Lerp(element.startColor, element.endColor, progress);
            yield return null;
        }

        // Ensure the color is set to the end color at the end of the animation
        spriteRenderer.color = element.endColor;

        if (element.isLooped)
        {
            StartCoroutine(AnimateColor(spriteRenderer, element)); // Restart the animation if looped
        }
    }


    private void HandleTransformAnimationElement(TransformAnimationElement element, GameObject obj)
    {
        // Implementation for Transform Animation Element
    }

    private void HandleDisplaceAnimationElement(DisplaceAnimationElement element, GameObject obj)
    {
        // Implementation for Displace Animation Element
    }

    private void HandleTraceAnimationElement(TraceAnimationElement element, GameObject obj)
    {
        // Implementation for Trace Animation Element
    }

    private void HandleDrawAnimationElement(DrawAnimationElement element, GameObject obj)
    {
        // Implementation for Draw Animation Element
    }
}

