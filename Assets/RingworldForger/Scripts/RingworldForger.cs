using System.Collections.Generic;
using UnityEngine;

namespace ChironPE
{
    [RequireComponent(typeof(Ringworld)), DisallowMultipleComponent]
    public class RingworldForger : MonoBehaviour
    {
        [Header("Ring Control")]
        public Vector2Int chunkCount = new Vector2Int(1, 8);
        public RingworldForgerInfo info = default;
        [Space]
        [Range(0f, 6f)]
        public int editorPreviewLOD = 0;
        public bool autoRefresh = false;

        private Ringworld ringworld = null;

        public void OnValidate()
        {
            chunkCount.x = Mathf.Clamp(chunkCount.x, 1, chunkCount.x);
            chunkCount.y = Mathf.Clamp(chunkCount.y, 1, chunkCount.y);

            info.radius = RingLayer.mapChunkSize / 6f * chunkCount.y;
            info.width = RingLayer.mapChunkSize * chunkCount.x;
            info.circumference = 2 * Mathf.PI * info.radius;

            foreach (RingLayer layer in info.layers)
            {
                layer.radius = info.radius;
                layer.chunkCount = chunkCount;

                layer.editorPreviewLOD = editorPreviewLOD;

                layer.isRingworldForgerControlled = true;
                layer.OnValidate();
                layer.autoRecreate = autoRefresh;
            }

            if(ringworld == null)
            {
                ringworld = GetComponent<Ringworld>();
            }

            ringworld.radius = info.radius;
            ringworld.width = info.width;
        }

        private void Awake()
        {
            // Shouldn't exist at runtime.
            Destroy(this);
            return;
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
            public RingLayer[] layers = default;
        }
    }
}
