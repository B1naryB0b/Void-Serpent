using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventManager : MonoBehaviour
{
    public void QueAnimationEvent(GameObject triggeredObject, List<AnimationElement> animationElements)
    {
        foreach (var animationElement in animationElements)
        {
            switch (animationElement)
            {
                case ColorAnimationElement colorElement:
                    HandleColorAnimationElement(colorElement, triggeredObject);
                    break;

                case TransformAnimationElement transformElement:
                    HandleTransformAnimationElement(transformElement, triggeredObject);
                    break;

                case DisplaceAnimationElement displaceElement:
                    HandleDisplaceAnimationElement(displaceElement, triggeredObject);
                    break;

                case TraceAnimationElement traceElement:
                    HandleTraceAnimationElement(traceElement, triggeredObject);
                    break;

                case DrawAnimationElement drawElement:
                    HandleDrawAnimationElement(drawElement, triggeredObject);
                    break;

                default:
                    Debug.LogWarning("No animation element type selected.");
                    break;
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
        
        IEnumerator AnimateColor(SpriteRenderer spriteRenderer, ColorAnimationElement element)
        {
            float elapsed = 0f;

            while (elapsed < element.animationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = element.curve.Evaluate(elapsed / element.animationDuration);
                Debug.Log(progress);
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

