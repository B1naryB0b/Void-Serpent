using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidHelper2D
{

    const int numViewDirections = 30;
    public static readonly Vector2[] directions; // Directions in 2D space.

    static BoidHelper2D()
    {
        directions = new Vector2[BoidHelper2D.numViewDirections];

        // We will use a simple approach to create directions in 2D space, 
        // covering 360 degrees.
        float angleIncrement = Mathf.PI * 2 / numViewDirections; // Full circle divided by the number of directions.

        for (int i = 0; i < numViewDirections; i++)
        {
            float angle = (angleIncrement * i) + (Mathf.PI/2);
        
            float x = Mathf.Cos(angle);
            float y = Mathf.Sin(angle);
            
            directions[i] = new Vector2(x, y); // Storing the 2D direction.

        }
    }
}
