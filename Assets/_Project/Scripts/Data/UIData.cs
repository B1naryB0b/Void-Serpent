using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "Data Storage/UIData")]
public class UIData : ScriptableObject
{
    [Header("Player Lives")]
    public GameObject lifeIconPrefab;
    public float livesUISpacingOffset;
    public float livesUIHorizontalAdjustmentOffset;
}
