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
    [SerializeField] private GameObject playerMeshObject;
    private PlayerData playerData;
    
    private Vector2 currentThrust = Vector2.zero;
    [HideInInspector] public Vector2 CurrentThrust => currentThrust != null ? currentThrust : Vector2.zero;

    private Rigidbody2D playerRb;
    private SimpleInertial inertialMovement;

    private Camera mainCamera;

    private void Start()
    {
        playerData = dataBank.playerData;
        inertialMovement = GetComponent<SimpleInertial>();

        playerRb = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        RotatePlayer();
    }


    private void MovePlayer()
    {
        inertialMovement.UpdateMovement(playerRb, currentThrust);

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Brake(playerRb);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Boost(playerRb);
        }
    }

    private void Boost(Rigidbody2D rb)
    {
        Vector2 boostVector = transform.up * inertialMovement.terminalVelocity;
        inertialMovement.SetBoostTerminalVelocity(boostVector.magnitude);
        rb.velocity += boostVector;
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
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationTowardsMouse, Time.deltaTime * playerData.pointToMouseRotationSpeed);

        Vector3 tiltRotation = GetTiltBasedOnAngleToMouse();
        //Debug.Log(tiltRotation);
        Vector3 currentMeshRotationEuler = playerMeshObject.transform.localEulerAngles;
        float lerpedYTilt = CustomLerpAngle(currentMeshRotationEuler.y, tiltRotation.y, Time.deltaTime * playerData.tiltRotationSpeed);

        playerMeshObject.transform.localEulerAngles = new Vector3(0, lerpedYTilt, 0);

    }

    private Quaternion GetRotationTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane playerPlane = new Plane(-Vector3.forward, transform.position);

        playerPlane.Raycast(ray, out float distanceToPlane);
      
        Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
        Vector3 direction = pointOnPlane - transform.position;

        direction.z = 0;

        Debug.DrawLine(transform.position, direction);

        return Quaternion.LookRotation(Vector3.forward, direction);

    }

    private Vector3 GetTiltBasedOnAngleToMouse()
    {
        Vector3 mousePosition = GetMousePositionOnPlayerPlane();
        float signedAngle = CalculateSignedAngleToMouse(mousePosition);
        float yTilt = Mathf.Clamp(signedAngle, -playerData.tiltClampAngle, playerData.tiltClampAngle);
        //Debug.Log(signedAngle);

        return new Vector3(0, yTilt, 0);
    }

    private Vector3 GetMousePositionOnPlayerPlane()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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

    float CustomLerpAngle(float startAngle, float endAngle, float t)
    {
        // Normalize angles to be in the range -180 to 180
        startAngle = NormalizeAngle(startAngle);
        endAngle = NormalizeAngle(endAngle);

        // Calculate the difference
        float difference = Mathf.DeltaAngle(startAngle, endAngle);

        // Lerp
        return NormalizeAngle(startAngle + difference * t);
    }

    float NormalizeAngle(float angle)
    {
        // Normalize angle to be in the range of -180 to 180
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;

        return angle;
    }


}
