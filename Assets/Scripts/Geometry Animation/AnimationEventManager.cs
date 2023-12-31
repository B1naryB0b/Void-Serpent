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
        StartCoroutine(AnimateTransform(element, obj));
    }

    IEnumerator AnimateTransform(TransformAnimationElement element, GameObject obj)
    {
        float elapsed = 0f;
        Transform originalObjTransform = obj.transform;

        // Save the original state
        Vector3 originalPosition = originalObjTransform.position;
        Quaternion originalRotation = originalObjTransform.rotation;
        Vector3 originalScale = originalObjTransform.localScale;

        while (elapsed < element.animationDuration)
        {
            elapsed += Time.deltaTime;
            float progress = element.curve.Evaluate(elapsed / element.animationDuration);

            // Position
            Vector3 startPosition = originalPosition + element.startTransform.position;
            Vector3 endPosition = originalPosition + element.endTransform.position;
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, progress);

            // Rotation
            Vector3 startRotation = originalRotation.eulerAngles + element.startTransform.rotation.eulerAngles;
            Vector3 endRotation = originalRotation.eulerAngles + element.endTransform.rotation.eulerAngles;
            obj.transform.rotation = Quaternion.Lerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), progress);

            // Scale
            Vector3 startScale = new Vector3(
                    originalScale.x * element.startTransform.localScale.x,
                    originalScale.y * element.startTransform.localScale.y,
                    originalScale.z * element.startTransform.localScale.z
                );

            Vector3 endScale = new Vector3(
                    originalScale.x * element.endTransform.localScale.x,
                    originalScale.y * element.endTransform.localScale.y,
                    originalScale.z * element.endTransform.localScale.z
                );

            obj.transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            yield return null;
        }

        // Reset to original state if not looped
        if (!element.isLooped)
        {
            obj.transform.position = originalPosition;
            //obj.transform.rotation = originalRotation;
            obj.transform.localScale = originalScale;
        }
        else
        {
            StartCoroutine(AnimateTransform(element, obj)); // Restart the animation if looped
        }
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

