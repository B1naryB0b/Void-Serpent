using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class POI
{
    public Transform poiTransform;

    [Range(0f, 1f)] public float importance = 0.2f; //value between 0f and 1f that specifies the importance of the point of interest weighting
}
