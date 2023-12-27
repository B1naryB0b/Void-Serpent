using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SimpleInertial))]

public class PlayerController : MonoBehaviour
{

    [SerializeField] private DataBank dataBank;
    private PlayerData playerData;
    
    private Vector2 currentThrust = Vector2.zero;
    [HideInInspector] public Vector2 CurrentThrust => currentThrust != null ? currentThrust : Vector2.zero;

    private Rigidbody2D playerRb;
    private SimpleInertial inertialMovement;

    private void Start()
    {
        playerData = dataBank.playerData;
        inertialMovement = GetComponent<SimpleInertial>();

        playerRb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void MovePlayer()
    {
        inertialMovement.UpdateMovement(playerRb, currentThrust);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Brake(playerRb);
        }
    }

    private void Brake(Rigidbody2D rb)
    {
        rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity * 0.9f, playerData.brakeStrength * Time.fixedDeltaTime);
        if (rb.velocity.sqrMagnitude < playerData.brakeStoppingVelocity)
        {
            rb.velocity = Vector2.zero;
        }
        currentThrust = Vector2.zero;
    }

    private void RotatePlayer()
    {
        Quaternion rotationTowardsMouse = GetRotationTowardsMouse();
        Quaternion tiltRotation = GetTiltBasedOnAngleToMouse();
        Quaternion totalTargetRotation = rotationTowardsMouse * tiltRotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, totalTargetRotation, Time.fixedDeltaTime);
    }

    private Quaternion GetRotationTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(-Vector3.forward, transform.position);

        playerPlane.Raycast(ray, out float distanceToPlane);
      
        Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
        Vector3 direction = pointOnPlane - transform.position;

        direction.z = 0;

        Debug.DrawLine(transform.position, direction);

        return Quaternion.LookRotation(Vector3.forward, direction);

    }

    private Quaternion GetTiltBasedOnAngleToMouse()
    {
        Vector3 mousePosition = GetMousePositionOnPlayerPlane();
        float signedAngle = CalculateSignedAngleToMouse(mousePosition);
        float yTilt = Mathf.Clamp(signedAngle / 2, -playerData.tiltClampAngle, playerData.tiltClampAngle);

        return Quaternion.AngleAxis(yTilt, Vector3.up);
    }

    private Vector3 GetMousePositionOnPlayerPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(-Vector3.forward, transform.position);
        playerPlane.Raycast(ray, out float distanceToPlane);
        return ray.GetPoint(distanceToPlane);
    }

    private float CalculateSignedAngleToMouse(Vector3 mousePosition)
    {
        Vector3 direction = (mousePosition - transform.position).normalized;
        Vector3 playerForward = transform.up;
        return Vector3.SignedAngle(playerForward, direction, Vector3.forward);
    }

}
