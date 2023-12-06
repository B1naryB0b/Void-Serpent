using UnityEditor;
using UnityEngine;
using static TreeEditor.TextureAtlas;

[CustomEditor(typeof(PerlinTextureSettings))]
public class PerlinTextureGenerator : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PerlinTextureSettings settings = (PerlinTextureSettings)target;

        if (GUILayout.Button("Generate Texture"))
        {
            GenerateTexture(settings);
        }
    }

    private void GenerateTexture(PerlinTextureSettings settings)
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
                }

                Color color = new Color(sample, sample, sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        SaveTexture(texture);

    }

    private void SaveTexture(Texture2D texture)
    {

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
            return layerValue;
        }
    }


}
