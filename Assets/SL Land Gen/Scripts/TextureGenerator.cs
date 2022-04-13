using UnityEngine;

// Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
public static class TextureGenerator
{
    public static Texture2D TextureFromColourMap(Vector2Int mapSize, Color[] colourMap)
    {
        Texture2D texture = new Texture2D(mapSize.x, mapSize.y);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;


        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        Vector2Int mapSize = new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));

        Color[] colourMap = new Color[mapSize.x * mapSize.y];
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                colourMap[y * mapSize.x + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(mapSize, colourMap);
    }
}
