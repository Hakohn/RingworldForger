using UnityEngine;

// Mainly inspired from (Sebastian Lague, 2016) https://www.youtube.com/watch?v=4RpVBYW1r5M
public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        AnimationCurve _heightCurve = new AnimationCurve(heightCurve.keys);
        Vector2Int mapSize = new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));
        float topLeftX = (mapSize.x - 1) / -2f;
        float topLeftZ = (mapSize.y - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (mapSize.x - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(mapSize);
        int vertexIndex = 0;

        for(int y = 0; y < mapSize.y; y += meshSimplificationIncrement)
        {
            for(int x = 0; x < mapSize.x; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, _heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)mapSize.x, y / (float)mapSize.y);

                // Generating the quads.
                if(x < mapSize.x - 1 && y < mapSize.y - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices = default;
    public int[] triangles = default;
    public Vector2[] uvs = default;

    private int triangleIndex = 0;

    public MeshData(Vector2Int size)
    {
        vertices = new Vector3[size.x * size.y];
        uvs = new Vector2[size.x * size.y];
        triangles = new int[(size.x - 1) * (size.y - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
