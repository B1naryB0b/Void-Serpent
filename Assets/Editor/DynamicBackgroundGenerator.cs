using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Stopwatch = System.Diagnostics.Stopwatch;

[CreateAssetMenu]
public class DynamicBackgroundGenerator : ScriptableObject
{

    public Vector2 mapSize = new Vector2(100, 100);
    public float noiseScale = 1.0f;
    public Vector2 noiseOffset;

    private GameObject backgroundAsteroidsParent;
    private GameObject backgroundStarsParent;
    private List<Vector3> spawnedPositions = new List<Vector3>();

    public float LARGE_ASTEROID_THRESHOLD = 0.8f;
    public float SMALL_ASTEROID_MIN_THRESHOLD = 0.6f;
    public float SMALL_ASTEROID_MAX_THRESHOLD = 0.8f;

    public Sprite[] largeAsteroidSprites; // Array of large asteroid sprites
    public Sprite[] smallAsteroidSprites; // Array of small asteroid sprites

    public Color spriteColor = Color.white; // Default to white (no color change)

    public float largeAsteroidSpacing;
    public float smallAsteroidSpacing;

    public Vector2 rotationRange = new Vector2(0, 360);

    public float STAR_CLUSTER_THRESHOLD = 0.7f;
    public Sprite[] starSprites; // Array of star sprites of varying sizes
    public float starSpacing = 0.5f; // Define an appropriate spacing for stars within clusters


    public void GenerateBackground()
    {
        Stopwatch stopwatch = new Stopwatch();

        // Measure time for initializing background parents
        stopwatch.Start();
        backgroundAsteroidsParent = InitializeBackgroundParent(backgroundAsteroidsParent, "Asteroids Background");
        backgroundStarsParent = InitializeBackgroundParent(backgroundStarsParent, "Stars Background");
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Initializing background parents took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();

        // Measure time for clearing spawned positions
        stopwatch.Start();
        spawnedPositions.Clear();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Clearing spawned positions took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();

        // Measure time for placing large asteroids
        stopwatch.Start();
        PlaceLargeAsteroids();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Placing large asteroids took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();

        // Measure time for placing small asteroids near large ones
        stopwatch.Start();
        PlaceSmallAsteroidsNearLargeOnes();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Placing small asteroids near large ones took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();

        // Measure time for generating star clusters
        stopwatch.Start();
        GenerateStarClusters();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Generating star clusters took: {stopwatch.ElapsedMilliseconds} ms");
    }

    private GameObject InitializeBackgroundParent(GameObject backgroundParent, string name)
    {
        backgroundParent = GameObject.Find(name);
        if (backgroundParent == null)
        {
            GameObject backgroundObj = new GameObject(name);
            return backgroundObj;
        }
        else
        {
            foreach (Transform child in backgroundParent.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            return backgroundParent;
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(UnityEngine.Random.Range(0, mapSize.x), UnityEngine.Random.Range(0, mapSize.y), 0);
    }

    private float CalculateNoiseValue(Vector3 position)
    {
        return Mathf.PerlinNoise(position.x * noiseScale + noiseOffset.x, position.y * noiseScale + noiseOffset.y);
    }


    private bool IsPositionValid(Vector3 position, float spacing)
{
    float squaredSpacing = spacing * spacing; // Compute squared spacing once

    foreach (Vector3 spawnedPosition in spawnedPositions)
    {
        if ((position - spawnedPosition).sqrMagnitude < squaredSpacing)
        {
            return false; // Early exit
        }
    }
    return true;
}


    private void PlaceLargeAsteroids()
    {
        int totalLargeAsteroids = Mathf.FloorToInt(mapSize.x * mapSize.y * LARGE_ASTEROID_THRESHOLD);

        for (int i = 0; i < totalLargeAsteroids; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            float noiseValue = CalculateNoiseValue(randomPosition);

            if (noiseValue > LARGE_ASTEROID_THRESHOLD && IsPositionValid(randomPosition, largeAsteroidSpacing))
            {
                Sprite randomLargeAsteroid = largeAsteroidSprites[UnityEngine.Random.Range(0, largeAsteroidSprites.Length)];

                bool isLargeAsteroid = Array.Exists(largeAsteroidSprites, s => s == randomLargeAsteroid);
                float asteroidScale = CalculateAsteroidScale(randomPosition, isLargeAsteroid);
                CreateSpriteObject(randomPosition, randomLargeAsteroid, rotationRange, asteroidScale, backgroundAsteroidsParent.transform);
            }
        }
    }

    private void PlaceSmallAsteroidsNearLargeOnes()
    {
        int totalSmallAsteroids = Mathf.FloorToInt(mapSize.x * mapSize.y * (SMALL_ASTEROID_MAX_THRESHOLD - SMALL_ASTEROID_MIN_THRESHOLD));

        for (int i = 0; i < totalSmallAsteroids; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            float noiseValue = CalculateNoiseValue(randomPosition);

            if (noiseValue > SMALL_ASTEROID_MIN_THRESHOLD && noiseValue <= SMALL_ASTEROID_MAX_THRESHOLD && IsPositionValid(randomPosition, smallAsteroidSpacing))
            {
                Sprite randomSmallAsteroid = smallAsteroidSprites[UnityEngine.Random.Range(0, smallAsteroidSprites.Length)];

                bool isSmallAsteroid = Array.Exists(largeAsteroidSprites, s => s == randomSmallAsteroid);
                float asteroidScale = CalculateAsteroidScale(randomPosition, isSmallAsteroid);
                CreateSpriteObject(randomPosition, randomSmallAsteroid, rotationRange, asteroidScale, backgroundAsteroidsParent.transform);
            }
        }
    }

    private float CalculateAsteroidScale(Vector3 position, bool isLargeAsteroid)
    {
        float minDistance = float.MaxValue;

        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            float distance = Vector3.Distance(position, spawnedPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        if (isLargeAsteroid)
        {
            // For large asteroids, scale varies from 1 to 4 based on proximity
            return Mathf.Lerp(4, 1, minDistance / 10.0f); // Assuming 10 units as the max distance for full scale
        }
        else
        {
            // For small asteroids, scale varies from 1 to 0.5 based on proximity
            return Mathf.Lerp(1, 0.5f, minDistance / 5.0f); // Assuming 5 units as the max distance for full scale
        }
    }

    private float CalculateStarScale(Vector3 position)
    {
        float minDistance = float.MaxValue;

        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            float distance = Vector3.Distance(position, spawnedPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }

        return Mathf.Lerp(0.1f, 0.5f, minDistance / 1.0f);

    }

    private void CreateSpriteObject(Vector3 position, Sprite sprite, Vector2 spriteRotationRange, float scale, Transform parentTransform, List<Vector3> positionsList = null)
    {
        GameObject spriteObject = new GameObject(sprite.name);
        spriteObject.transform.position = position;

        // Calculate a random rotation value within the specified range and apply it
        float randomRotation = UnityEngine.Random.Range(spriteRotationRange.x, spriteRotationRange.y);
        spriteObject.transform.eulerAngles = new Vector3(0, 0, randomRotation);

        // Set the scale
        spriteObject.transform.localScale = new Vector3(scale, scale, 1);

        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = spriteColor;
        spriteObject.transform.SetParent(parentTransform);

        if (positionsList != null)
        {
            positionsList.Add(position);
        }
        else
        {
            spawnedPositions.Add(position);
        }
    }


    public void GenerateStarClusters()
    {

        int totalStarClusters = Mathf.FloorToInt(mapSize.x * mapSize.y * STAR_CLUSTER_THRESHOLD);

        for (int i = 0; i < totalStarClusters; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            float noiseValue = CalculateNoiseValue(randomPosition);

            if (noiseValue > STAR_CLUSTER_THRESHOLD && IsPositionValid(randomPosition, starSpacing))
            {
                
                PlaceStarsInCluster(randomPosition);

            }
        }


    }

    private void PlaceStarsInCluster(Vector3 clusterCenter)
    {
        int starsInCluster = UnityEngine.Random.Range(3, 10); // Define a range for the number of stars in a cluster
        int starSpritesLength = starSprites.Length; // Cache the length of the starSprites array

        for (int i = 0; i < starsInCluster; i++)
        {
            float randomXOffset = UnityEngine.Random.Range(-starSpacing, starSpacing);
            float randomYOffset = UnityEngine.Random.Range(-starSpacing, starSpacing);
            Vector3 starPosition = new Vector3(clusterCenter.x + randomXOffset, clusterCenter.y + randomYOffset, 0);

            if (IsPositionValid(starPosition, starSpacing))
            {
                Sprite randomStar = starSprites[UnityEngine.Random.Range(0, starSpritesLength)];
                float starScale = CalculateStarScale(starPosition);
                CreateSpriteObject(starPosition, randomStar, Vector2.zero, starScale, backgroundStarsParent.transform);
            }
        }
    }


}
