using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{

    [SerializeField] private float terminalVelocity = 12.0f;

    public void UpdateMovement(Rigidbody2D rb, Vector2 currentThrust)
    {
        /*if (Input.GetKey(KeyCode.W))
        {
            currentThrust = Mathf.Clamp(currentThrust + thrustIncreaseRate * Time.fixedDeltaTime, 0, maxThrust);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentThrust = Mathf.Clamp(currentThrust - thrustDecreaseRate * Time.fixedDeltaTime, 0, maxThrust);
        }
*/
        // Apply force in the forward direction based on current thrust
        rb.AddForce(transform.up * currentThrust);

        void ClampVelocity(Rigidbody2D rb)
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalVelocity);
        }

        ClampVelocity(rb);
    }

}
