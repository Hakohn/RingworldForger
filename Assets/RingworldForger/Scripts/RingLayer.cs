using UnityEngine;
using UnityEngine.Serialization;

namespace ChironPE
{
    // Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
    [DisallowMultipleComponent]
    public class RingLayer : MonoBehaviour
    {
        public const int mapChunkSize = 241;
        public const int maxBiomeCount = 10;

        [Header("Noise / Height Map")]
        [Tooltip("The seed of the random number generator. If you like your current settings but want to see a new result, just change this.")]
        public int seed = 0;
        [Tooltip("The scale of the noise map. Changing this value feels like zooming in and out when looking on a digital map.")]
        public float noiseScale = 0.3f;
        [Tooltip("Change this to move the location on the noise map.")]
        public Vector2 offset = Vector2.zero;
        [Space]
        [Tooltip("The normalize mode represents the calculation of the highest and lowest height possible. If using more than 1 chunk, leave it on Global.")]
        public Noise.NormalizeMode normalizeMode = Noise.NormalizeMode.Global;
        [Tooltip("Should the face normals be inverted? The face normal represents the direction a face is facing.")]
        public bool invertFaces = false;
        [Tooltip("The number of octaves determines how blurry the height map is. Adding a higher number of octaves feels like putting on better and better glasses.")]
        [Space]
        public int octaves = 4;
        [Tooltip("The higher the value, the more sharp the terrain heights will be.")]
        [Range(0f, 1f)]
        public float persistance = 0.5f;
        [Tooltip("Lacunarity represents a layer of fog put on top of the height map. The higher the value, the more dense and scattered the fog is.")]
        public float lacunarity = 2f;
        [Space]
        [Tooltip("How high each height is and how low the low points are.")]
        public float meshHeightMultiplier = 1f;
        [Tooltip("Determines how the heights, multiplied by the height multiplier, work. You may want, for example, the mountains to happen less often, but when they do, they actually go tall; to achieve this, keep the curve relatively flat until a certain point and then spike the remaining curve up to (1, 1)")]
        public AnimationCurve meshHeightCurve = AnimationCurve.Linear(0f, 1f, 0f, 1f);


        [Space]
        [Tooltip("The material that will be applied to all the child chunks. If you made some changes to the base material, make sure to tap the refresh button; the children chunks are duplicating the base material on spawn and will not inherit from it unless refreshed. For terrain, it is strongly recommended to use the RingworldForger Terrain Shader.")]
        public Material baseMaterial = null;
        [Tooltip("All the textures assigned to biomes MUST be of this exact size.")]
        public int textureSize = 512;
        [Tooltip("When pressing the spawn vegetation button, this is the maximum amount of objects that can be spawned. Make sure to have at least one biome with a vegetation table attached to see any results.")]
        public int vegetationSpawnLimit = 2500;
        [Tooltip("On what Unity layers can the vegetation be spawned on?")]
        public LayerMask vegetationGeneratorLayers = 1;
        [Tooltip("Height levels, to be precise. The terrain changes its colours based on these biomes. Make sure to keep their heights sorted.")]
        [FormerlySerializedAs("regions")]
        public Biome[] biomes = new Biome[1];
        // Gathered on validate from biomes.
        private Texture2DArray biome_textures = default;
        private float[] biome_textureScales = default;
        private Color[] biome_tints = default;
        private float[] biome_tintStrengths = default;
        private float[] biome_startHeights = default;
        private float[] biome_blendStrengths = default;

        [Header("Chunks")]
        [Tooltip("The size of the ring, calculated in the number of pieces of terrain it will be composed of. X is the width, and Y is the circumference.")]
        [Space]
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        public Vector2Int chunkCount = new Vector2Int(1, 8);
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        [DisableField]
        public float radius = 100;
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        [DisableField]
        public float width = 20;
        [Tooltip("Should the generated chunks of terrain have colliders / be solid?")]
        public bool generateColliders = false;
        [SerializeField]
        private RingChunkMesh[] ringChunkMeshes = new RingChunkMesh[0];
        [Space]
        [Tooltip("Determines how much detail will the ring you're working on have. The lower the value, the more detail it has.")]
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        [Range(0f, 6f)]
        public int editorPreviewLOD = 0;
        [Tooltip("Should the ring recreate all its vertices every time you change one its variables, and allow live procedural generation? It is NOT recommended to be enabled if you're dealing with a large ring or have poor hardware.")]
        [ConditionalHide("isRingworldForgerControlled", hideInInspector: true, reversed: true)]
        public bool autoRefresh = false;

        [HideInInspector]
        public bool isRingworldForgerControlled = false;

        public float MinHeight => meshHeightMultiplier * meshHeightCurve.Evaluate(0);
        public float MaxHeight => meshHeightMultiplier * meshHeightCurve.Evaluate(1);

        private void Awake()
        {
            RefreshBiomeData();
        }

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

            textureSize = Mathf.Clamp(textureSize, 1, textureSize);
            vegetationSpawnLimit = Mathf.Clamp(vegetationSpawnLimit, 1, vegetationSpawnLimit);

            RefreshBiomeData();
        }

        private void RefreshBiomeData()
        {
            // Limit the maximum number of biomes inputted by the user.
            if(biomes.Length > maxBiomeCount)
            {
                Biome[] newBiomes = new Biome[maxBiomeCount];
                for(int i = 0; i < maxBiomeCount; i++)
                {
                    newBiomes[i] = biomes[i];
                }

                biomes = newBiomes;
            }

            // Forcing it to be zero, to avoid shader blackness.
            if(biomes.Length > 0 && biomes[0] != null)
            {
                biomes[0].startHeight = 0f;
            }

            // Gathering biome data for better performance with the shader.
            biome_textures = new Texture2DArray(textureSize, textureSize, biomes.Length, TextureFormat.RGB565, true);
            biome_textureScales = new float[biomes.Length];
            biome_tints = new Color[biomes.Length];
            biome_tintStrengths = new float[biomes.Length];
            biome_startHeights = new float[biomes.Length];
            biome_blendStrengths = new float[biomes.Length];

            for (int i = 0; i < biomes.Length; i++)
            {
                if(biomes[i] == null)
                {
                    biomes[i] = new Biome();
                }

                if (biomes[i].texture != null)
                {
                    biome_textures.SetPixels(biomes[i].texture.GetPixels(), i);
                }
                biome_textureScales[i] = biomes[i].textureScale;

                biome_tints[i] = biomes[i].tint;
                biome_tintStrengths[i] = biomes[i].tintStrength;

                biome_startHeights[i] = biomes[i].startHeight;
                biome_blendStrengths[i] = biomes[i].blendStrength;
            }
            biome_textures.Apply();
        }

        public void UpdateShaders()
        {
            RingChunkMesh.ShaderData shaderData = new RingChunkMesh.ShaderData
            {
                // Height
                height_max = MaxHeight,
                height_min = MinHeight,
                // Ring
                ring_position = transform.position,
                ring_radius = radius,
                ring_right = transform.right,
                ring_width = width,
                // Biome
                biomeCount = biomes.Length,
                biome_tints = biome_tints,
                biome_startHeights = biome_startHeights,
                biome_blendStrengths = biome_blendStrengths,
                biome_textures = biome_textures,
                biome_textureScales = biome_textureScales,
                biome_tintStrengths = biome_tintStrengths,
                localToWorldMatrix = transform.localToWorldMatrix
            };
            foreach (RingChunkMesh chunk in ringChunkMeshes)
            {
                chunk.UpdateMeshShader(shaderData);
            }
        }

        public void RefreshLayerChunks()
        {
            // Refresh the number of children chunks.
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

                    MeshData meshData = MeshGenerator.GenerateRingTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLOD, radius, degPerChunk, invertFaces);
                    chunk.SetMesh(
                        meshData,
                        TextureGenerator.TextureFromColourMap(new Vector2Int(mapChunkSize, mapChunkSize), mapData.colourMap),
                        baseMaterial,
                        generateColliders
                    );

                    Vector3 localPos = chunk.transform.localPosition;
                    localPos.x = (x - (chunkCount.x - 1) / 2f) * (mapChunkSize - 1);
                    chunk.transform.localPosition = localPos;
                    chunk.transform.localRotation = Quaternion.Euler(degPerChunk * y, 0, 0) * transform.localRotation;

                    chunk.OnGeneration();
                }
            }

            // This will update the shader data.
            UpdateShaders();
        }

        public void RefreshVegetation()
        {
            // Clearing existing vegetation, if applicable.
            ClearVegetation();

            for(int i = 0; i < vegetationSpawnLimit; i++)
            {
                Ray ray = new Ray
                {
                    // Choosing a random start point along the width of the ring.
                    origin = transform.position + transform.right * Random.Range(-width / 2f, width / 2f),
                    // Choosing a random world direction for the ray.
                    direction = transform.TransformDirection(new Vector3(0, Random.Range(-1f, 1f), Random.Range(-1f, 1f))),
                };
                bool successful = Physics.Raycast(ray, out RaycastHit hitInfo, radius, vegetationGeneratorLayers, QueryTriggerInteraction.Ignore);

                // Was anything hit?
                if (!successful) continue;
                // Was a chunk's terrain hit?
                if (!hitInfo.collider.gameObject.TryGetComponent(out RingChunkMesh chunk)) continue;

                // A chunk was hit, so now it is time to check what vegetation needs to be spawned.
                // First, the biome needs to be found, and it will be done through hit info point height detection.
                float height = radius - hitInfo.distance;
                float heightPercent = height.Remap(MinHeight, MaxHeight, 0f, 1f);
                Biome biome = null;
                for(int j = biomes.Length - 1; j >= 0; j--)
                {
                    if(heightPercent >= biomes[j].startHeight)
                    {
                        biome = biomes[j];
                        break;
                    }
                }

                // No biome was found (This means something must be wrong...)
                if (biome == null) 
                {
                    Debug.LogWarning($"No biome was found with a start height lower than {heightPercent}%, which makes the vegetation spawning invalid. Check the biomes settings and try again.");
                    continue; 
                }

                // The biome was found but has no vegetation table.
                if (biome.vegetationTable == null) continue;

                GameObject vegetationPrefab = biome.vegetationTable.GenerateLoot();
                // The loot table was quite unlucky.
                if (vegetationPrefab == null) continue;

                // Finding the parent for the new vegetation object.
                if(chunk.vegetationParent == null)
                {
                    GameObject vegParent = new GameObject("Vegetation");
                    vegParent.transform.parent = chunk.transform;
                    vegParent.transform.localPosition = Vector3.zero;
                    vegParent.transform.localRotation = Quaternion.identity;
                    vegParent.transform.localScale = Vector3.one;
                    chunk.vegetationParent = vegParent.transform;
                }

                // The desired vegetation was found, and will now be spawned.
                GameObject spawnedVegetation = Instantiate(vegetationPrefab,
                    hitInfo.point,
                    Quaternion.identity,
                    chunk.vegetationParent
                );
                // Rotating it accordingly.
                spawnedVegetation.transform.up = hitInfo.normal;
                spawnedVegetation.transform.Rotate(Vector3.up, Random.Range(0, 360), Space.Self);
                spawnedVegetation.name = spawnedVegetation.name.Replace("(Clone)", "");
                chunk.vegetationObjects.Add(spawnedVegetation);
            }
        }

        public void ClearVegetation()
        {
            for (int y = 0; y < chunkCount.y; y++)
            {
                for (int x = 0; x < chunkCount.x; x++)
                {
                    int chunkIndex = y * chunkCount.x + x;
                    ringChunkMeshes[chunkIndex].ClearVegetation();
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
                    for (int i = 0; i < biomes.Length; i++)
                    {
                        if (currentHeight >= biomes[i].startHeight)
                        {
                            colourMap[y * mapChunkSize + x] = biomes[i].tint;
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

    [System.Serializable]
    public class Biome
    {
        [Tooltip("The name of the biome. Used only for display purposes.")]
        public string name;
        [Space]
        [Tooltip("The texture applied to the terrain. Can be left empty.")]
        public Texture2D texture = null;
        [Tooltip("The scale of the texture (how big or how small it is displayed on 3D models.")]
        public float textureScale = 1f;
        [Space]
        [Tooltip("The colour / tint applied to the terrain.")]
        [FormerlySerializedAs("colour")]
        public Color tint;
        [Tooltip("The strength of the applied tint. If set to one, the texture (if any) will be invisible, and only the colour will be displayed. If set to zero, only the texture is displayed. If somewhere in between, the texture will have its colour tinted by chosen tint colour.")]
        [Range(0f, 1f)]
        public float tintStrength = 0.5f;
        [Space]
        [Tooltip("The height from which this biome will began being used.")]
        [Range(0f, 1f), FormerlySerializedAs("height")]
        public float startHeight = 0f;
        [Tooltip("How strong will the blending go between this and the other biomes? Fades in/out the textures / tint.")]
        [Range(0f, 1f), FormerlySerializedAs("blend")]
        public float blendStrength = 0.5f;
        [Space]
        [Tooltip("The table containing all the vegetation that can spawn here. It can be left empty, if no vegetation is desired.")]
        public SelectionTableSO vegetationTable = null;
    }

    public struct MapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colourMap;

        public MapData(float[,] heightMap, Color[] colourMap)
        {
            this.heightMap = heightMap;
            this.colourMap = colourMap;
        }
    }
}

