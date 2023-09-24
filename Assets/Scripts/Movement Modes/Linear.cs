using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : MonoBehaviour
{

    [SerializeField] private float maxLinearVelocity = 20.0f;
    [SerializeField] private float acceleration = 10.0f;

    public void UpdateMovement(Rigidbody2D rb)
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        // Set the velocity directly based on input and maxThrust
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(horizontalMove, verticalMove).normalized * maxLinearVelocity, Time.deltaTime * acceleration);
        }

        //currentThrust = maxThrust * Mathf.Abs(horizontalMove * verticalMove);
    }
}
