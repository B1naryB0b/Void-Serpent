using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{

    [SerializeField] enum GizmoType { Never, SelectedOnly, Always }
    [SerializeField] GizmoType showTriggerRange;
    [SerializeField] Color triggerColor;

    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject triggerObject;

    [Range(0f, 100f)]
    [SerializeField] float triggerRange;

    [Range(0f, 1f)]
    [SerializeField] float triggerCheckIntervalTime;

    [SerializeField] AnimationEventManager animationEventManager;
    [SerializeField] AnimationEvent animationEvent;

    private float timeSinceLastCheck;

    private bool isTriggered;

    private void Start()
    {
        timeSinceLastCheck = 0f;
    }

    private void Update()
    {
        if (isTriggered) return;
        Timer();
    }

    private void Timer()
    {
        timeSinceLastCheck += Time.deltaTime;

        if (timeSinceLastCheck > triggerCheckIntervalTime)
        {
            CheckTrigger();

            timeSinceLastCheck = 0f;
        }
    }

    private void CheckTrigger()
    {
        //Ability to add custom trigger condition check to be added
        if (Vector2.Distance(playerObject.transform.position, gameObject.transform.position) < triggerRange)
        {
            animationEvent.TriggerEvent(triggerObject, animationEventManager);
            isTriggered = true;
        }
    }

    void OnDrawGizmos()
    {
        if (showTriggerRange == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showTriggerRange == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {
        Gizmos.color = new Color(triggerColor.r, triggerColor.g, triggerColor.b, 0.3f);
        Gizmos.DrawWireSphere(transform.position, triggerRange);
    }
}
