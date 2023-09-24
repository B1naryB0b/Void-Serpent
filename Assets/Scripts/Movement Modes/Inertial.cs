using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inertial : MonoBehaviour
{
    [SerializeField] private float maxThrust = 5.0f;          
    [SerializeField] private float thrustIncreaseRate = 10.0f;  
    [SerializeField] private float thrustDecreaseRate = 20.0f;
    [SerializeField] private float terminalVelocity = 12.0f;  

    private bool canChangeVerticalDirection = true;
    private bool canChangeHorizontalDirection = true;

    public void UpdateMovement(Rigidbody2D rb, Vector2 currentThrust)
    {
        currentThrust.y = AdjustThrust(currentThrust.y, KeyCode.W, KeyCode.S, canChangeVerticalDirection, VerticalDirectionChangeDelay);
        currentThrust.x = AdjustThrust(currentThrust.x, KeyCode.D, KeyCode.A, canChangeHorizontalDirection, HorizontalDirectionChangeDelay);

        // Apply force based on thrust values
        rb.AddForce(transform.up * currentThrust.y + transform.right * currentThrust.x);
        
        void ClampVelocity(Rigidbody2D rb)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalVelocity);
        }

        ClampVelocity(rb);
    }

    private float AdjustThrust(float current, KeyCode positiveKey, KeyCode negativeKey, bool canChangeDirection, Func<IEnumerator> delayMethod)
    {
        if (Input.GetKey(positiveKey) && canChangeDirection)
        {
            if (current < 0 && !Mathf.Approximately(current, 0f))
            {
                current = DecreaseToZero(current, delayMethod);
            }
            else
            {
                current = Mathf.Clamp(current + thrustIncreaseRate * Time.fixedDeltaTime, 0, maxThrust);
            }
        }
        else if (Input.GetKey(negativeKey) && canChangeDirection)
        {
            if (current > 0 && !Mathf.Approximately(current, 0f))
            {
                current = DecreaseToZero(current, delayMethod);
            }
            else
            {
                current = Mathf.Clamp(current - thrustIncreaseRate * Time.fixedDeltaTime, -maxThrust, 0);
            }
        }
        return current;
    }

    private float DecreaseToZero(float current, Func<IEnumerator> delayMethod)
    {
        current = Mathf.MoveTowards(current, 0, thrustDecreaseRate * Time.fixedDeltaTime);
        if (Mathf.Approximately(current, 0f))
        {
            StartCoroutine(delayMethod());
        }
        return current;
    }

    private IEnumerator VerticalDirectionChangeDelay()
    {
        canChangeVerticalDirection = false;
        yield return new WaitForSeconds(0.3f);
        canChangeVerticalDirection = true;
    }

    private IEnumerator HorizontalDirectionChangeDelay()
    {
        canChangeHorizontalDirection = false;
        yield return new WaitForSeconds(0.3f);
        canChangeHorizontalDirection = true;
    }

}
