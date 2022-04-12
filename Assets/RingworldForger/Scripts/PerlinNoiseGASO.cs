using UnityEngine;

namespace ChironPE
{
    [CreateAssetMenu(menuName = "Ringworld Forger/Generation Algorithms/Perlin Noise")]
	public class PerlinNoiseGASO : GenerationAlgorithmSO
	{
		public float heightMin = 0;
		public float heightMax = 1;
		public Vector2 perlinOffset = Vector2.zero;

		public override float[,] GenerateHeightmap(Vector2Int size)
		{
			// Initialize terrain - set values in the height map to 0
			float[,] heights = new float[size.x, size.y];
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					heights[x, y] = 0;
				}
			}

			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					float height = heightMin + Mathf.PerlinNoise(x * perlinOffset.x, y * perlinOffset.y) * heightMax;
					heights[x, y] = height;
				}
			}

			return heights;
		}
	}
}
