using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=f0m73RsBik4
public class RingTerrain : MonoBehaviour
{
    [SerializeField]
    private float scale = 1f;
    private static float _scale = 1f;

    private float maxViewDist = default;
    public LODInfo[] detailLevels = default;

    public Transform viewer = null;
    [SerializeField]
    private float viewerMoveThresholdForChunkUpdate = 25f;
    private float sqrViewerMoveThresholdForChunkUpdate = 25f * 25f;

    public MapGenerator mapGenerator = null;
    public Material mapMaterial = null;
    public static Vector2 viewerPosition = default;
    private Vector2 viewerPositionOld = default;
    private int chunkSize = default;
    private int chunksVisibleInViewDist = default;

    private readonly Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private static readonly List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    private void Start()
    {
        _scale = scale;

        maxViewDist = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDist = Mathf.RoundToInt(maxViewDist / chunkSize);
        sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / _scale;

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    private void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        Vector2Int currentChunkCoords = new Vector2Int(
            Mathf.RoundToInt(viewerPosition.x / chunkSize),
            Mathf.RoundToInt(viewerPosition.y / chunkSize)
        );

        for (int yOffset = -chunksVisibleInViewDist; yOffset <= chunksVisibleInViewDist; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDist; xOffset <= chunksVisibleInViewDist; xOffset++)
            {
                Vector2 viewedChunkCoords = currentChunkCoords + new Vector2Int(xOffset, yOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoords))
                {
                    terrainChunkDictionary[viewedChunkCoords].UpdateTerrainChunk(maxViewDist);
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoords, new TerrainChunk(viewedChunkCoords, chunkSize, detailLevels, mapMaterial, transform, mapGenerator, maxViewDist));
                }
            }
        }
    }

    public class TerrainChunk
    {
        private readonly GameObject meshObject = null;
        private Bounds bounds = default;
        private Vector2 position = default;

        private MapData mapData = default;
        private bool mapDataReceived = false;
        private int previousLODIndex = -1;

        private readonly MeshRenderer meshRenderer = null;
        private readonly MapGenerator mapGenerator = null;
        private readonly MeshFilter meshFilter = null;
        private readonly LODInfo[] detailLevels = default;
        private readonly LODMesh[] lodMeshes = default;

        private readonly float maxViewDist = default;

        public TerrainChunk(Vector2 coords, int size, LODInfo[] detailLevels, Material mapMaterial, Transform parent, MapGenerator mapGenerator, float maxViewDist)
        {
            this.detailLevels = detailLevels;
            this.mapGenerator = mapGenerator;
            this.maxViewDist = maxViewDist;

            position = coords * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshRenderer.material = mapMaterial;

            meshObject.transform.position = positionV3 * _scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * _scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, this.mapGenerator, () => UpdateTerrainChunk(this.maxViewDist));
            }

            this.mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(Vector2Int.one * MapGenerator.mapChunkSize, mapData.colourMap);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk(maxViewDist);
        }

        public void UpdateTerrainChunk(float maxViewDist)
        {
            if (!mapDataReceived) return;

            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDist;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData);
                    }
                }

                terrainChunksVisibleLastUpdate.Add(this);
            }

            SetVisible(visible);
        }

        public void SetVisible(bool value)
        {
            meshObject.SetActive(value);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
