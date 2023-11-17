using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinTextureSettings))]
public class PerlinTextureGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PerlinTextureSettings settings = (PerlinTextureSettings)target;

        if (GUILayout.Button("Generate Texture"))
        {
            GenerateAndSaveTexture(settings);
        }
    }

    private void GenerateAndSaveTexture(PerlinTextureSettings settings)
    {
        Texture2D texture = new Texture2D(settings.width, settings.height);
        int numLayers = 20; // Example value, adjust as needed

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float xCoord = (float)x / settings.width;
                float yCoord = (float)y / settings.height;
                float sample = MultiLayerPerlin(xCoord, yCoord, settings);
                sample = GrayscaleStep(sample, numLayers);
                Color color = new Color(sample, sample, sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/Topology Mapper/GeneratedPerlinTexture.png");
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();
    }

    private float MultiLayerPerlin(float x, float y, PerlinTextureSettings settings)
    {
        float perlinValue = 0f;
        float weightSum = 0f;

        foreach (var layer in settings.layers)
        {
            float layerValue = Mathf.PerlinNoise(x * layer.scale, y * layer.scale) * layer.layerWeight;
            perlinValue += layerValue;
            weightSum += layer.layerWeight;
        }

        return weightSum > 0 ? perlinValue / weightSum : 0f;
    }

    float GrayscaleStep(float x, int numLayers)
    {
        float stepSize = 1f / numLayers;
        return Mathf.Floor(x / stepSize) * stepSize;
    }




}
