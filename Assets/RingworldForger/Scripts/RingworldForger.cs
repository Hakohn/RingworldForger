using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [ExecuteAlways]
    [RequireComponent(typeof(Ringworld)), DisallowMultipleComponent]
    public class RingworldForger : MonoBehaviour
    {
        [Header("Ring Control")]
        [Tooltip("The size of the ring, calculated in the number of pieces of terrain it will be composed of. X is the width, and Y is the circumference. The resulting sizes (radius, width, etc.) in units / metres is visible under 'Info'.")]
        public Vector2Int chunkCount = new Vector2Int(1, 8);
        [Tooltip("Information that you may find useful about the ring you're working in.")]
        public RingworldForgerInfo info = new RingworldForgerInfo();
        [Tooltip("Determines how much detail will the ring you're working on have. The lower the value, the more detail it has.")]
        [Space]
        [Range(0f, 6f)]
        public int editorPreviewLOD = 0;
        [Tooltip("Should the ring recreate all its vertices every time you change one its variables, and allow live procedural generation? It is NOT recommended to be enabled if you're dealing with a large ring or have poor hardware.")]
        public bool autoRefresh = false;

        /// <summary>
        /// The ringworld that is created by the forger.
        /// </summary>
        private Ringworld ringworld = null;

        public void OnValidate()
        {
            chunkCount.x = Mathf.Clamp(chunkCount.x, 1, chunkCount.x);
            chunkCount.y = Mathf.Clamp(chunkCount.y, 1, chunkCount.y);

            info.radius = RingLayer.mapChunkSize / 6f * chunkCount.y;
            info.width = RingLayer.mapChunkSize * chunkCount.x;
            info.circumference = 2 * Mathf.PI * info.radius;

            info.layers = GetComponentsInChildren<RingLayer>();
            foreach (RingLayer layer in info.layers)
            {
                layer.radius = info.radius;
                layer.chunkCount = chunkCount;
                layer.width = info.width;

                layer.editorPreviewLOD = editorPreviewLOD;

                layer.isRingworldForgerControlled = true;
                layer.OnValidate();
                layer.autoRefresh = autoRefresh;
            }

            if(ringworld == null)
            {
                ringworld = GetComponent<Ringworld>();
            }

            ringworld.radius = info.radius;
            ringworld.width = info.width;
            ringworld.OnValidate();
            ringworld.UpdateBounds();
        }

        //private void Awake()
        //{
        //    // Shouldn't exist at runtime.
        //    Destroy(this);
        //    return;
        //}

        private void Update()
        {
            foreach (RingLayer layer in info.layers)
            {
                layer.UpdateShaders();
            }
        }

        public void RefreshRingChunks()
        {
            foreach (RingLayer layer in info.layers)
            {
                layer.RefreshLayerChunks();
            }
        }

        [System.Serializable]
        public class RingworldForgerInfo
        {
            [DisableField]
            public float radius = 0;
            [DisableField]
            public float width = 0;
            [DisableField]
            public float circumference = 0;
            [DisableField]
            public RingLayer[] layers = new RingLayer[0];
        }
    }
}
