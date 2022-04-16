using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    // Mainly inspired from (Sebastian Lague, 2016): https://www.youtube.com/watch?v=MRNFcywkUSA
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RingChunkMesh : MonoBehaviour
    {
        private MeshFilter meshFilter = null;
        private MeshRenderer meshRenderer = null;
        private Vector3 localCentre = Vector3.zero;

        [HideInInspector]
        public List<GameObject> vegetationObjects = new List<GameObject>();
        [HideInInspector]
        public Transform vegetationParent = null;
    
        [Header("Ring Layer-controlled")]
        [DisableField]
        public bool needsCollider = false;

        [SerializeField, HideInInspector]
        private ShaderData savedShaderData = new ShaderData();

        public MeshCollider meshCollider { get; private set; } = null;

        public Vector3 Centre => transform.TransformPoint(localCentre);

        public MeshFilter Filter
        {
            get
            {
                if (meshFilter == null)
                    meshFilter = GetComponent<MeshFilter>();

                return meshFilter;
            }
        }
        public MeshRenderer Renderer
        {
            get
            {
                if (meshRenderer == null)
                    meshRenderer = GetComponent<MeshRenderer>();

                return meshRenderer;
            }
        }

        private void Awake()
        {
            localCentre = (Filter.mesh.vertices[0] + Filter.mesh.vertices[Filter.mesh.vertices.Length - 1]) / 2f;
        }

        private void OnValidate()
        {
            UpdateMeshShader(savedShaderData);
        }

        public void OnGeneration()
        {
            if (needsCollider)
            {
                if ((meshCollider = GetComponent<MeshCollider>()) == null)
                {
                    meshCollider = gameObject.AddComponent<MeshCollider>();
                }

                meshCollider.sharedMesh = Filter.sharedMesh;
            }
        }

        public void UpdateMeshShader(ShaderData shaderData)
        {
            savedShaderData = shaderData;

            if (Renderer == null) return;
            if (Renderer.sharedMaterial == null) return;

            // Heights
            Renderer.sharedMaterial.SetFloat("minHeight", shaderData.height_min);
            Renderer.sharedMaterial.SetFloat("maxHeight", shaderData.height_max);

            // Ring info
            Renderer.sharedMaterial.SetVector("ring_centre", shaderData.ring_position);
            Renderer.sharedMaterial.SetVector("ring_right", shaderData.ring_right);
            Renderer.sharedMaterial.SetFloat("ring_radius", shaderData.ring_radius);
            Renderer.sharedMaterial.SetFloat("ring_width", shaderData.ring_width);
            Renderer.sharedMaterial.SetMatrix("localToWorldMatrix", shaderData.localToWorldMatrix);

            // Biomes
            Renderer.sharedMaterial.SetInt("biomeCount", shaderData.biomeCount);
            if(shaderData.biome_tints != null)
            {
                Renderer.sharedMaterial.SetTexture("biome_textures", shaderData.biome_textures);
                Renderer.sharedMaterial.SetFloatArray("biome_textureScales", shaderData.biome_textureScales);
                Renderer.sharedMaterial.SetColorArray("biome_tints", shaderData.biome_tints);
                Renderer.sharedMaterial.SetFloatArray("biome_tintStrengths", shaderData.biome_tintStrengths);
                Renderer.sharedMaterial.SetFloatArray("biome_startHeights", shaderData.biome_startHeights);
                Renderer.sharedMaterial.SetFloatArray("biome_blendStrengths", shaderData.biome_blendStrengths);
            }
        }

        public void SetMesh(MeshData meshData, Texture2D texture, Material baseMat, bool needsCollider = false)
        {
            Filter.sharedMesh = meshData.CreateMesh();
            Renderer.sharedMaterial = new Material(baseMat)
            {
                mainTexture = texture
            };

            this.needsCollider = needsCollider;

            UpdateMeshShader(savedShaderData);
        }

        public void ClearVegetation()
        {
            for(int i = 0; i < vegetationObjects.Count; i++)
            {
                if(vegetationObjects[i] == null) continue;

                if (Application.isPlaying)
                {
                    Destroy(vegetationObjects[i]);
                }
                else
                {
                    DestroyImmediate(vegetationObjects[i]);
                }
            }
            vegetationObjects.Clear();
        }
        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                Gizmos.DrawWireSphere(Centre, 5f);
            }
        }

        public struct ShaderData
        {
            // Heights
            public float height_min;
            public float height_max;
            // Ring
            public Vector3 ring_position;
            public Vector3 ring_right;
            public float ring_radius;
            public float ring_width;
            public Matrix4x4 localToWorldMatrix;
            // Biomes
            public int biomeCount;
            public Texture2DArray biome_textures;
            public float[] biome_textureScales;
            public Color[] biome_tints;
            public float[] biome_tintStrengths;
            public float[] biome_startHeights;
            public float[] biome_blendStrengths;
        }
    }
}
