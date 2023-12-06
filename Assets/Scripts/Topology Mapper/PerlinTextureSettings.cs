using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PerlinTextureSettings", menuName = "TextureGeneration/PerlinTexture")]
public class PerlinTextureSettings : ScriptableObject
{
    public enum TextureMode
    {
        GrayscaleStep,
        LineStep,
        GrayscaleLineStep
    }

    public int width = 256;
    public int height = 256;
    public List<Layer> layers = new List<Layer>();

    public TextureMode textureMode = TextureMode.GrayscaleStep;
    public int numLayers = 20;

    [Range(0f, 0.01f)]
    public float lineWidth = 0.01f;

    [System.Serializable]
    public struct Layer
    {
        public float scale;
        public float layerWeight;

        public Layer(float scale, float layerWeight)
        {
            this.scale = scale;
            this.layerWeight = layerWeight;
        }
    }
}