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
            Texture2D generatedTexture = GenerateTexture(settings);
            SaveTexture(generatedTexture, settings);
        }
    }

    private Texture2D GenerateTexture(PerlinTextureSettings settings)
    {
        Texture2D texture = new Texture2D(settings.width, settings.height);

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float xCoord = (float)x / settings.width;
                float yCoord = (float)y / settings.height;
                float sample = MultiLayerPerlin(xCoord, yCoord, settings);

                switch (settings.textureMode)
                {
                    case PerlinTextureSettings.TextureMode.GrayscaleStep:
                        sample = GrayscaleStep(sample, settings.numLayers);
                        break;
                    case PerlinTextureSettings.TextureMode.LineStep:
                        sample = LineStep(sample, settings.numLayers, settings.lineWidth);
                        break;
                    case PerlinTextureSettings.TextureMode.GrayscaleLineStep:
                        sample = GrayscaleLineStep(sample, settings.numLayers, settings.lineWidth);
                        break;

                }

                Color color = new Color(sample, sample, sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }


    private void SaveTexture(Texture2D texture, PerlinTextureSettings settings)
    {
        string name = settings.name;
        byte[] bytes = texture.EncodeToPNG();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Scripts/Topology Mapper/" + name + ".png");
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

    float LineStep(float value, int numLayers, float lineWidth)
    {
        float stepSize = 1f / numLayers;
        float layerValue = Mathf.Floor(value / stepSize) * stepSize;

        float delta = Mathf.Abs(value - layerValue);

        if (delta < lineWidth / 2f)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    float GrayscaleLineStep(float value, int numLayers, float lineWidth)
    {
        float stepSize = 1f / numLayers;
        float layerValue = Mathf.Floor(value / stepSize) * stepSize;

        float delta = Mathf.Abs(value - layerValue);

        if (delta < lineWidth / 2f)
        {
            return 1f;
        }
        else
        {
            return layerValue;
        }
    }


}
