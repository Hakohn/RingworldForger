using UnityEngine;

namespace ChironPE
{
    // Mainly inspired from (Sebastian Lague, 2016) https://www.youtube.com/watch?v=4RpVBYW1r5M
    public static class MeshGenerator
    {
        public static MeshData GenerateRingTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail, float radius, float degrees, bool invertTriangles)
        {

            AnimationCurve _heightCurve = new AnimationCurve(heightCurve.keys);
            Vector2Int mapSize = new Vector2Int(heightMap.GetLength(0), heightMap.GetLength(1));
            float topLeftX = (mapSize.x - 1) / -2f;
            float topLeftZ = (mapSize.y - 1) / 2f;

            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (mapSize.x - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(mapSize);
            int vertexIndex = 0;

            for (int x = 0; x < mapSize.x; x += meshSimplificationIncrement)
            {
                for (int y = 0; y < mapSize.y; y += meshSimplificationIncrement)
                {
                    Vector3 vertex = new Vector3(topLeftX + x, _heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - y);

                    // Wrapping it around a cylinder.
                    vertex = new Vector3
                    {
                        x = vertex.x,
                        y = (radius - vertex.y) * Mathf.Cos(degrees / (RingLayer.mapChunkSize - 1) * vertex.z * Mathf.Deg2Rad),
                        z = (radius - vertex.y) * Mathf.Sin(degrees / (RingLayer.mapChunkSize - 1) * vertex.z * Mathf.Deg2Rad)
                    };
                    
                    meshData.vertices[vertexIndex] = vertex;
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)mapSize.x, y / (float)mapSize.y);

                    // Generating the quads.
                    if (x < mapSize.x - 1 && y < mapSize.y - 1)
                    {
                        Vector3Int indicesA = new Vector3Int(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        Vector3Int indicesB = new Vector3Int(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);

                        if(invertTriangles)
                        {
                            meshData.AddInvertedTriangle(indicesA.x, indicesA.y, indicesA.z);
                            meshData.AddInvertedTriangle(indicesB.x, indicesB.y, indicesB.z);
                        }
                        else
                        {
                            meshData.AddTriangle(indicesA.x, indicesA.y, indicesA.z);
                            meshData.AddTriangle(indicesB.x, indicesB.y, indicesB.z);
                        }
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

        public void AddInvertedTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = c;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = a;

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
}
