using UnityEngine;

namespace ChironPE
{
    // Main inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
    // Fixes inspired from (Lejynn, 2022): https://www.youtube.com/watch?v=XpG3YqUkCTY&t
    public static class Noise
    {
        public enum NormalizeMode
        {
            Local,
            Global
        };

        public static float[,] GenerateNoiseMap(Vector2Int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
        {
            float[,] noiseMap = new float[mapSize.x, mapSize.y];

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            // We do not want any division my zero or negative scale, so this should clamp it to the smallest allowed value.
            if(scale <= 0.101f)
            {
                scale = 0.101f;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            Vector2 halfMapSize = new Vector2(mapSize.x, mapSize.y) / 2f;

            for(int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    // Octaves
                    for(int i = 0; i < octaves; i++)
                    {
                        // Higher frequency makes points further apart.
                        float sampleX = (x - halfMapSize.x + octaveOffsets[i].x) / scale * frequency;
                        float sampleY = (y - halfMapSize.y + octaveOffsets[i].y) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    // Finding the minimum and maximum heights.
                    if(noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    else if(noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }

                    // Saving the generated height.
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    switch(normalizeMode)
                    {
                        case NormalizeMode.Local:
                            // Normalize the heights between 0 and 1.
                            noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                            break;
                        case NormalizeMode.Global:
                            float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                            noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                            break;
                    }    
                }
            }


            return noiseMap;
        }
    }
}
