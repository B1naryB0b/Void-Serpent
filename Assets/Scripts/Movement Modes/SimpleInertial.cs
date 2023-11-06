using UnityEngine;

public class SimpleInertial : MonoBehaviour
{
    [SerializeField] private float maxThrust = 5.0f;
    [SerializeField] private float thrustIncreaseRate = 10.0f;
    [SerializeField] private float thrustDecreaseRate = 20.0f;
    [SerializeField] private float terminalVelocity = 12.0f;

    public void UpdateMovement(Rigidbody2D rb, Vector2 currentThrust)
    {
        // Assuming the vertical input is being used for thrust.
        if (Input.GetKey(KeyCode.W))
        {
            currentThrust.y += thrustIncreaseRate * Time.fixedDeltaTime;
        }
        else
        {
            currentThrust.y -= thrustDecreaseRate * Time.fixedDeltaTime;
        }

        currentThrust.y = Mathf.Clamp(currentThrust.y, 0, maxThrust);

        rb.AddForce(transform.up * currentThrust.y);
        ClampVelocity(rb);
    }

    private void ClampVelocity(Rigidbody2D rb)
    {
        if (rb.velocity.magnitude > terminalVelocity)
        {
            rb.velocity = rb.velocity.normalized * terminalVelocity;
        }
    }
}
