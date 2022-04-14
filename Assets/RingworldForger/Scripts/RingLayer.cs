using UnityEngine;
using UnityEngine.Serialization;

namespace ChironPE
{
    // Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
    [DisallowMultipleComponent]
    public class RingLayer : MonoBehaviour
    {
        [Header("Generator")]
        public float noiseScale = 0.3f;
        public const int mapChunkSize = 241;
        public Noise.NormalizeMode normalizeMode = Noise.NormalizeMode.Global;
        public bool invertFaces = false;
        [Space]
        public int octaves = 4;
        [Range(0f, 1f)]
        public float persistance = 0.5f;
        public float lacunarity = 2f;
        [Space]
        public int seed = 0;
        public Vector2 offset = Vector2.zero;
        [Space]
        public float meshHeightMultiplier = 1f;
        public AnimationCurve meshHeightCurve = AnimationCurve.Linear(0f, 1f, 0f, 1f);
        [Space]
        public Material baseMaterial = null;
        [FormerlySerializedAs("regions")]
        public Biome[] biomes = default;

        [Header("Chunks")]
        [Space]
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        public Vector2Int chunkCount = new Vector2Int(1, 8);
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        [DisableField]
        public float radius = 100;
        public bool generateColliders = false;
        [SerializeField]
        private RingChunkMesh[] ringChunkMeshes = default;
        [Space]
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        [Range(0f, 6f)]
        public int editorPreviewLOD = 0;
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        public bool autoRecreate = false;

        [HideInInspector]
        public bool isRingworldForgerControlled = false;

        public void OnValidate()
        {
            if (noiseScale <= 0.101f)
            {
                noiseScale = 0.101f;
            }

            octaves = Mathf.Clamp(octaves, 1, octaves);
            persistance = Mathf.Clamp(persistance, 0, persistance);
            lacunarity = Mathf.Clamp(lacunarity, 1, lacunarity);

            if(!isRingworldForgerControlled)
            {
                chunkCount.x = Mathf.Clamp(chunkCount.x, 1, chunkCount.x);
                chunkCount.y = Mathf.Clamp(chunkCount.y, 1, chunkCount.y);

                radius = mapChunkSize / 6f * chunkCount.y;
            }

        }

        private void Awake()
        {
            // Shouldn't exist at runtime.
            Destroy(this);
            return;
        }

        public void RefreshLayerChunks()
        {
            if (ringChunkMeshes == null || ringChunkMeshes.Length != chunkCount.x * chunkCount.y)
            {
                // Remove old children.
                if (ringChunkMeshes != null)
                {
                    foreach(RingChunkMesh display in ringChunkMeshes)
                    {
                        if (display == null) continue;
                        DestroyImmediate(display.gameObject);
                    }
                }

                // Remove remaining children, if any leftovers.
                ringChunkMeshes = GetComponentsInChildren<RingChunkMesh>();
                if (ringChunkMeshes != null)
                {
                    foreach (RingChunkMesh display in ringChunkMeshes)
                    {
                        if (display == null) continue;
                        DestroyImmediate(display.gameObject);
                    }
                }

                // Create the new children.
                ringChunkMeshes = new RingChunkMesh[chunkCount.x * chunkCount.y];
                for (int y = 0; y < chunkCount.y; y++)
                {
                    for (int x = 0; x < chunkCount.x ; x++)
                    {
                        int chunkIndex = y * chunkCount.x + x;
                        ringChunkMeshes[chunkIndex] = new GameObject("Chunk #" + chunkIndex).AddComponent<RingChunkMesh>();
                        ringChunkMeshes[chunkIndex].transform.parent = transform;
                    }
                }
            }

            float degPerChunk = 360f / chunkCount.y;
            for (int x = 0; x < chunkCount.x; x++)
            {
                for (int y = 0; y < chunkCount.y; y++)
                {
                    MapData mapData = GenerateMapData(new Vector2(x, y) * (mapChunkSize - 1));

                    RingChunkMesh chunk = ringChunkMeshes[y * chunkCount.x + x];

                    Vector3 localPos = chunk.transform.localPosition;
                    localPos.x = (x - (chunkCount.x - 1) / 2f) * (mapChunkSize - 1);
                    chunk.transform.localPosition = localPos;

                    chunk.transform.rotation = Quaternion.Euler(degPerChunk * y, 0, 0) * transform.rotation;
                    MeshData meshData = MeshGenerator.GenerateRingTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD, radius, degPerChunk, invertFaces);
                    chunk.SetMesh(
                        meshData,
                        TextureGenerator.TextureFromColourMap(new Vector2Int(mapChunkSize, mapChunkSize), mapData.colourMap),
                        baseMaterial,
                        generateColliders
                    );

                    for(int vegX = 0; vegX < mapChunkSize; vegX++)
                    {
                        for (int vegY = 0; vegY < mapChunkSize; vegY++)
                        {
                            if(mapData.vegetationMap[vegX, vegY] != null)
                            {
                                GameObject obj = Instantiate(mapData.vegetationMap[vegX, vegY]);
                                obj.transform.parent = chunk.transform;
                                obj.transform.localPosition = meshData.vertices[vegY * mapChunkSize + vegX];
                                obj.transform.up = (transform.position - obj.transform.position).normalized;
                            }
                        }
                    }
                }
            }
        }

        public MapData GenerateMapData(Vector2 centre)
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(new Vector2Int(mapChunkSize, mapChunkSize), seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

            Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
            GameObject[,] vegetationMap = new GameObject[mapChunkSize, mapChunkSize];
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < biomes.Length; i++)
                    {
                        if (currentHeight >= biomes[i].height)
                        {
                            colourMap[y * mapChunkSize + x] = biomes[i].colour;

                            if(biomes[i].vegetationTable != null)
                            {
                                vegetationMap[x, y] = biomes[i].vegetationTable.GenerateLoot();
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colourMap, vegetationMap);
        }
    }

    [System.Serializable]
    public class Biome
    {
        public string name;
        [Range(0f, 1f)]
        public float height = 0f;
        public Color colour;
        public SelectionTableSO vegetationTable = null;
    }

    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colourMap;
        public readonly GameObject[,] vegetationMap;

        public MapData(float[,] heightMap, Color[] colourMap, GameObject[,] vegetationMap)
        {
            this.heightMap = heightMap;
            this.colourMap = colourMap;
            this.vegetationMap = vegetationMap;
        }
    }
}

