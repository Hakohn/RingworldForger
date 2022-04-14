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

    
        [Header("Ringworld Generator-controlled")]
        [DisableField]
        public bool needsCollider = false;
        //public 

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

            if(needsCollider)
            {
                if((meshCollider = GetComponent<MeshCollider>()) == null)
                {
                    meshCollider = gameObject.AddComponent<MeshCollider>();
                }

                meshCollider.sharedMesh = Filter.sharedMesh;
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
        }

        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                Gizmos.DrawWireSphere(Centre, 5f);
            }
        }
    }
}
