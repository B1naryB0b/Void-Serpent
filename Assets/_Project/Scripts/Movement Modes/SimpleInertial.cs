using UnityEngine;

public class SimpleInertial : MonoBehaviour
{
    [SerializeField] private float maxThrust = 5.0f;
    [SerializeField] private float thrustIncreaseRate = 10.0f;
    [SerializeField] private float thrustDecreaseRate = 20.0f;

    public float terminalVelocity = 12.0f;
    [SerializeField] private float tempTerminalVelocity;
    [SerializeField] private float terminalVelocityDecay;

    private float currentTerminalVelocity;

    private float thrustInput = 0.0f;

    private void Start()
    {
        currentTerminalVelocity = terminalVelocity;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            thrustInput = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            thrustInput = -1.0f;
        }
        else
        {
            thrustInput = 0.0f;
        }
    }

    public void UpdateMovement(Rigidbody2D rb, Vector2 currentThrust)
    {
        if (thrustInput != 0)
        {
            float thrustDirection = Mathf.Sign(thrustInput);
            currentThrust.y += thrustDirection * (thrustInput > 0 ? thrustIncreaseRate : thrustDecreaseRate) * Time.fixedDeltaTime;
        }
        else
        {
            currentThrust = DecreaseThrust(currentThrust);
        }

        Vector2 DecreaseThrust(Vector2 currentThrust)
        {
            if (currentThrust.y > 0)
            {
                currentThrust.y -= thrustDecreaseRate * Time.fixedDeltaTime;
            }
            else if (currentThrust.y < 0)
            {
                currentThrust.y += thrustDecreaseRate * Time.fixedDeltaTime;
            }

            return currentThrust;
        }

        currentThrust.y = Mathf.Clamp(currentThrust.y, -maxThrust, maxThrust);

        rb.AddForce(transform.up * currentThrust.y);
        ClampVelocity(rb);
    }

    private void ClampVelocity(Rigidbody2D rb)
    {
        if (rb.velocity.magnitude > currentTerminalVelocity)
        {
            rb.velocity = rb.velocity.normalized * currentTerminalVelocity;
        }

        currentTerminalVelocity -= Time.fixedDeltaTime * terminalVelocityDecay;
        currentTerminalVelocity = Mathf.Clamp(currentTerminalVelocity, terminalVelocity, tempTerminalVelocity);
    }


    public void SetBoostTerminalVelocity(float boostVelocity = 0f)
    {
        currentTerminalVelocity += boostVelocity;
    }

 
}