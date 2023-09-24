using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thrusters : MonoBehaviour
{

    [SerializeField] private GameObject thrustTrail;

    [SerializeField] private Slider verticalThrustSlider;
    [SerializeField] private Slider horizontalThrustSlider;

    [SerializeField] private float thrustTrailMaxScale = 1.0f;  // Maximum scale for the thrust trail

    [SerializeField] private AudioSource thrustAudioSource;

    private Vector2 previousVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        
    }

    private Vector2 CalculateAcceleration(Rigidbody2D rb)
    {
        // Calculate acceleration: a = (v - u) / t
        Vector2 acceleration = (rb.velocity - previousVelocity) / Time.fixedDeltaTime;

        // Update the previous velocity for the next calculation
        previousVelocity = rb.velocity;

        return acceleration;
    }

    private Vector2 previousThrustDirection = Vector2.zero;

    // Modify the UpdateThrustTrail function
    public void UpdateThrustTrail(Rigidbody2D rb)
    {
        if (thrustTrail != null)
        {
            Vector2 thrustDirection = CalculateAcceleration(rb);
            Vector2 smoothedThrustDirection = Vector2.Lerp(previousThrustDirection, thrustDirection, Time.fixedDeltaTime * 0.5f);

            previousThrustDirection = smoothedThrustDirection;

            Debug.DrawRay(transform.position, smoothedThrustDirection);

            float targetScale = Mathf.Lerp(0, thrustTrailMaxScale, Mathf.Abs(thrustDirection.magnitude));
            targetScale = Mathf.Clamp(targetScale, 0, thrustTrailMaxScale);

            float rotationAngle = Mathf.Atan2(smoothedThrustDirection.y, smoothedThrustDirection.x) * Mathf.Rad2Deg;
            thrustTrail.transform.rotation = Quaternion.Euler(0, 0, rotationAngle - 90f);

            thrustAudioSource.volume = Mathf.Abs(thrustDirection.magnitude) * 0.1f;

            thrustTrail.transform.localScale = new Vector3(thrustTrail.transform.localScale.x, targetScale, thrustTrail.transform.localScale.z);

            // Calculate the displacement
            float displacementMagnitude = thrustTrail.transform.localScale.y / 2;
            Vector2 normalizedOppositeDirection = -smoothedThrustDirection.normalized;
            Vector2 displacement = displacementMagnitude * normalizedOppositeDirection;

            // Rotate the displacement by the negative of the parent's rotation
            float parentRotationAngle = -transform.eulerAngles.z * Mathf.Deg2Rad;
            float rotatedX = displacement.x * Mathf.Cos(parentRotationAngle) - displacement.y * Mathf.Sin(parentRotationAngle);
            float rotatedY = displacement.x * Mathf.Sin(parentRotationAngle) + displacement.y * Mathf.Cos(parentRotationAngle);
            displacement = new Vector2(rotatedX, rotatedY);

            thrustTrail.transform.localPosition = new Vector3(displacement.x, displacement.y, thrustTrail.transform.localPosition.z);

        }
        else
        {
            thrustAudioSource.volume = 0f;
        }
    }






    /*private void DepricatedUpdateThrustTrail()
    {
        UpdateThrustTrailDirection(currentThrust.y, verticalThrustTrail, Vector3.up, thrustSounds[0]);
        UpdateThrustTrailDirection(currentThrust.x, horizontalThrustTrail, Vector3.right, thrustSounds[1]);
    }

    private void UpdateThrustTrailDirection(float thrustDirectionValue, GameObject thrustTrailObj, Vector3 movementDirection, AudioSource thrustAudioSource)
    {
        if (thrustTrailObj)
        {
            float currentYScale = Mathf.Abs(thrustTrailObj.transform.localScale.y);
            float targetYScale = Mathf.Lerp(0, thrustTrailMaxScale, Mathf.Abs(thrustDirectionValue) / maxThrust);

            thrustAudioSource.volume = (Mathf.Abs(thrustDirectionValue) / maxThrust) * 0.1f;

            // Calculate the change in scale
            float scaleChangeY = targetYScale - currentYScale;

            // Adjust the position of the thrust trail based on the change in scale and direction
            Vector3 adjustedPosition = thrustTrailObj.transform.localPosition;

            if (movementDirection == Vector3.up || movementDirection == Vector3.down)
            {
                adjustedPosition.y -= scaleChangeY / 2.0f * Mathf.Sign(thrustDirectionValue);
            }
            else // if movement direction is horizontal (right or left)
            {
                adjustedPosition.x -= scaleChangeY / 2.0f * Mathf.Sign(thrustDirectionValue);
            }

            thrustTrailObj.transform.localPosition = adjustedPosition;

            // Update the y-scale based on the thrust direction
            thrustTrailObj.transform.localScale = new Vector3(thrustTrailObj.transform.localScale.x, targetYScale * Mathf.Sign(thrustDirectionValue), thrustTrailObj.transform.localScale.z);

            if (movementMode == MovementMode.Linear)
            {
                // Determine the rotation angle based on movement direction
                float rotationAngle;
                if (movementDirection == Vector3.up)
                {
                    rotationAngle = Mathf.Atan2(movementDirection.x, movementDirection.y) * Mathf.Rad2Deg - 90f;
                }
                else // if movement direction is horizontal
                {
                    rotationAngle = Mathf.Atan2(-movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
                }
                Quaternion desiredRotation = Quaternion.Euler(0, 0, rotationAngle);
                thrustTrailObj.transform.rotation = Quaternion.RotateTowards(thrustTrailObj.transform.rotation, desiredRotation, rotationSpeed);
            }
        }
        else
        {
            thrustAudioSource.volume = 0f;
        }
    }*/
}
