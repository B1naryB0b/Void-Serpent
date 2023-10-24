using UnityEngine;

public interface IPointOfInterest
{
    Transform PointTransform { get; }
    float Importance { get; set; }
}

