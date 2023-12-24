using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data Storage/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]


    [Header("Rotation")]
    [Range(0f, 90f)]
    public float tiltClampAngle;

    [Header("Braking")]
    public float brakeStrength;
    public float brakeStoppingVelocity;

    [Header("HP/Shielding")]

    public int maxLives;
    public int maxShield;

    public float invulnerabilityDuration;
    public float shieldedInvulnerabilityDuration;

    public float shieldRegenDelay;
    public float flashDuration;
    public float fadeDuration;

    public GameObject explosionPrefab;

}
