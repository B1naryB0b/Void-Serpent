using UnityEngine;
using System.Collections.Generic;
using System;
using Stopwatch = System.Diagnostics.Stopwatch;

[CreateAssetMenu]
public class ParallaxBackgroundGenerator : ScriptableObject
{
    public string backgroundName;
    public Vector2 mapSize = new Vector2(1, 1);
    public Vector2 chunkSize = new Vector2(100, 100);
    public int layerCount;
    public float noiseScale = 1.0f;
    public Vector2 noiseOffset;
    private List<GameObject> backgroundObjectsParents = new List<GameObject>();
    private List<Vector3> spawnedPositions = new List<Vector3>();
    public float objectSpawnThreshold;
    public Sprite[] sprites;
    public Color spriteColor = Color.white;
    public float objectSpacing;
    public float parallaxSpacing;
    public Vector2 rotationRange = new Vector2(0, 360);

    public float minScale = 0.1f;
    public float maxScale = 0.5f;

    private GameObject masterBackgroundParent;

    public void GenerateBackground()
    {

        if (masterBackgroundParent == null)
        {
            masterBackgroundParent = new GameObject($"{backgroundName}_MasterParent");
        }

        ClearChildren(masterBackgroundParent);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        InitializeBackgroundParents();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Initializing background parents took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();
        stopwatch.Start();
        spawnedPositions.Clear();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Clearing spawned positions took: {stopwatch.ElapsedMilliseconds} ms");
        stopwatch.Reset();
        stopwatch.Start();
        GenerateObjects();
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Generating objects took: {stopwatch.ElapsedMilliseconds} ms");

        UnityEngine.Debug.Log(spawnedPositions.Count);
    }

    private void InitializeBackgroundParents()
    {
        backgroundObjectsParents.Clear();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float currentZPosition = 0f;
                for (int i = 0; i < layerCount; i++)
                {
                    string name = $"{backgroundName}_{i}";
                    GameObject backgroundParent = new GameObject(name);
                    backgroundParent.transform.localPosition = new Vector3(x * chunkSize.x, y * chunkSize.y, currentZPosition);
                    currentZPosition += parallaxSpacing;
                    backgroundObjectsParents.Add(backgroundParent);
                    backgroundParent.transform.SetParent(masterBackgroundParent.transform, false);
                }
            }
        }
        
    }

    private void ClearChildren(GameObject parent)
    {
        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }
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

    private float CalculateScale(Vector3 position)
    {
        // You can modify this method to use minScale and maxScale
        float noiseValue = CalculateNoiseValue(position);
        return Mathf.Lerp(minScale, maxScale, noiseValue);
    }

    private void CreateSpriteObject(Vector3 localPosition, Sprite sprite, Vector2 spriteRotationRange, float scale, Transform parentTransform)
    {
        GameObject spriteObject = new GameObject(sprite.name);
        spriteObject.transform.localPosition = localPosition;
        float randomRotation = UnityEngine.Random.Range(spriteRotationRange.x, spriteRotationRange.y);
        spriteObject.transform.localEulerAngles = new Vector3(0, 0, randomRotation);
        spriteObject.transform.localScale = new Vector3(scale, scale, 1);
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = spriteColor;
        spriteObject.transform.SetParent(parentTransform, false);
        spawnedPositions.Add(parentTransform.TransformPoint(localPosition));
    }


    public void GenerateObjects()
    {
        int totalChunks = Mathf.FloorToInt(mapSize.x * mapSize.y);
        int totalObjects = Mathf.FloorToInt(chunkSize.x * chunkSize.y * totalChunks * objectSpawnThreshold);
        int objectsPerChunk = totalObjects / totalChunks;
        int objectsPerLayer = objectsPerChunk / layerCount;
        int remainder = totalObjects % totalChunks;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                int chunkIndex = x * (int)mapSize.y + y;
                int objectCountForThisChunk = objectsPerChunk + (remainder > 0 ? 1 : 0);
                if (remainder > 0) remainder--;

                for (int layer = 0; layer < layerCount; layer++)
                {
                    int objectsForThisLayer = objectsPerLayer;
                    // If this is the last layer in the chunk and there's a remainder, add it to this layer.
                    if (layer == layerCount - 1)
                    {
                        objectsForThisLayer += objectCountForThisChunk % layerCount;
                    }

                    for (int i = 0; i < objectsForThisLayer; i++)
                    {
                        int parentIndex = chunkIndex * layerCount + layer;
                        GameObject parentObject = backgroundObjectsParents[parentIndex];
                        Vector3 randomPosition = GetRandomPositionWithinChunk(x, y);
                        if (IsPositionValid(randomPosition, objectSpacing))
                        {
                            float noiseValue = CalculateNoiseValue(randomPosition);
                            if (noiseValue > objectSpawnThreshold)
                            {
                                ObjectSpacing(randomPosition, parentObject);
                            }
                        }
                    }
                }
            }
        }
    }


    private Vector3 GetRandomPositionWithinChunk(int chunkX, int chunkY)
    {
        float startX = chunkX * chunkSize.x;
        float startY = chunkY * chunkSize.y;
        return new Vector3(
            UnityEngine.Random.Range(startX, startX + chunkSize.x),
            UnityEngine.Random.Range(startY, startY + chunkSize.y),
            0
        );
    }

    private void ObjectSpacing(Vector3 clusterCenter, GameObject parentObject)
    {
        int objectsInCluster = UnityEngine.Random.Range(3, 10);
        int spritesLength = sprites.Length;

        for (int i = 0; i < objectsInCluster; i++)
        {
            float randomXOffset = UnityEngine.Random.Range(-objectSpacing, objectSpacing);
            float randomYOffset = UnityEngine.Random.Range(-objectSpacing, objectSpacing);
            Vector3 objectPosition = new Vector3(clusterCenter.x + randomXOffset, clusterCenter.y + randomYOffset, 0);

            if (IsPositionValid(objectPosition, objectSpacing))
            {
                Sprite randomSprite = sprites[UnityEngine.Random.Range(0, spritesLength)];
                float objectScale = CalculateScale(objectPosition);
                CreateSpriteObject(objectPosition, randomSprite, rotationRange, objectScale, parentObject.transform);
            }
        }
    }

}
