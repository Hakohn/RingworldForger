using UnityEngine;

// Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
public class RingGenerator : MonoBehaviour
{
    [Header("Generator")]
    public float noiseScale = 0.3f;
    public const int mapChunkSize = 241;
    public Noise.NormalizeMode normalizeMode = Noise.NormalizeMode.Global;
    public bool invertFaces = false;

    public int octaves = 4;
    [Range(0f, 1f)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;

    public int seed = 0;
    public Vector2 offset = Vector2.zero;

    public float meshHeightMultiplier = 1f;
    public AnimationCurve meshHeightCurve = AnimationCurve.Linear(0f, 1f, 0f, 1f);

    public TerrainType[] regions = default;

    [Header("Ring")]
    public int circumferenceChunkCount = 8;
    public int widthChunkCount = 1;
    public float radius = 100;
    public Material baseMaterial = null;
    public bool generateColliders = false;
    [SerializeField]
    private RingChunkMesh[] ringChunkMeshes = default;

    [Header("Live Display")]
    [Range(0f, 6f)]
    public int editorPreviewLOD = 0;
    public bool autoRedraw = false;


    private void OnValidate()
    {
        if (noiseScale <= 0.101f)
        {
            noiseScale = 0.101f;
        }

        octaves = Mathf.Clamp(octaves, 1, octaves);
        persistance = Mathf.Clamp(persistance, 0, persistance);
        lacunarity = Mathf.Clamp(lacunarity, 1, lacunarity);

        widthChunkCount = Mathf.Clamp(widthChunkCount, 1, widthChunkCount);
        circumferenceChunkCount = Mathf.Clamp(circumferenceChunkCount, 1, circumferenceChunkCount);

        radius = mapChunkSize / 6f * circumferenceChunkCount;
    }

    public void CreateRingChunks()
    {
        if (ringChunkMeshes == null || ringChunkMeshes.Length != widthChunkCount * circumferenceChunkCount)
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
            ringChunkMeshes = new RingChunkMesh[widthChunkCount * circumferenceChunkCount];
            for (int y = 0; y < circumferenceChunkCount; y++)
            {
                for (int x = 0; x < widthChunkCount ; x++)
                {
                    int chunkIndex = y * widthChunkCount + x;
                    ringChunkMeshes[chunkIndex] = new GameObject("Chunk #" + chunkIndex).AddComponent<RingChunkMesh>();
                    ringChunkMeshes[chunkIndex].transform.parent = transform;
                }
            }
        }

        float degPerChunk = 360f / circumferenceChunkCount;
        for (int x = 0; x < widthChunkCount; x++)
        {
            for (int y = 0; y < circumferenceChunkCount; y++)
            {
                MapData mapData = GenerateMapData(new Vector2(x, y) * (mapChunkSize - 1));

                RingChunkMesh currentDisplay = ringChunkMeshes[y * widthChunkCount + x];

                currentDisplay.transform.position += x * (mapChunkSize - 1) * transform.right;
                currentDisplay.transform.rotation = Quaternion.Euler(degPerChunk * y, 0, 0) * transform.rotation;
                currentDisplay.SetMesh(
                    MeshGenerator.GenerateRingTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD, radius, degPerChunk, invertFaces),
                    TextureGenerator.TextureFromColourMap(new Vector2Int(mapChunkSize, mapChunkSize), mapData.colourMap),
                    baseMaterial,
                    generateColliders
                );
            }
        }
    }

    public MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(new Vector2Int(mapChunkSize, mapChunkSize), seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colourMap);
    }
}

