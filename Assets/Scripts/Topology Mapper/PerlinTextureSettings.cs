using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PerlinTextureSettings", menuName = "TextureGeneration/PerlinTexture")]
public class PerlinTextureSettings : ScriptableObject
{
    public int width = 256;
    public int height = 256;
    public List<Layer> layers = new List<Layer>();


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
