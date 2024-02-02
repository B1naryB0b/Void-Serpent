using UnityEngine;

[System.Serializable]
public class POI : IPointOfInterest
{
    [SerializeField] public Transform poiTransform;
    public Transform PointTransform => poiTransform; // Implementation of IPointOfInterest

    [SerializeField]
    [Range(0f, 1f)]
    public float importance = 0.1f; // Implementation of IPointOfInterest
    public float Importance
    {
        get => importance;
        set
        {
            if (value >= 0f && value <= 1f)
                importance = value;
        }
    }
}

