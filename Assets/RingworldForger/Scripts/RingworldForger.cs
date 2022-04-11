using UnityEngine;

namespace ChironPE
{
    [ExecuteAlways]
    public class RingworldForger : MonoBehaviour
    {
        [Header("Ring")]
        public float radius = 500;
        public float width = 32;
        public float spinningSpeed = 10;
        public RingMaterialPaletteSO materialPalette = null;

        [Header("Outer Ring")]
        public RingLayer outer = new RingLayer();

        [Header("Ocean")]
        public RingLayer ocean = new RingLayer();
        public float oceanHeight = 5;

        [Header("Continental Crust")]
        public RingLayer continent = new RingLayer();
        [SerializeField]
        private Vector2 continentPerlinOffset = Vector2.one * 0.3f;
        [SerializeField]
        private float continentMinHeight = 1.0f;
        [SerializeField]
        private float continentMaxHeight = 20.0f;

        [Header("On Save")]
        [SerializeField]
        private bool generateCollider = false;

        public Vector3 Centre => transform.TransformPoint(width / 2, radius, 0);

        private void Start()
        {
            // Outer Ring
            if(outer.mesh == null)
            {
                outer.mesh = new Mesh();
                outer.mesh.name = gameObject.name + " Outer Ring Mesh";
                outer.filter = new GameObject("Outer Ring", typeof(MeshFilter)).GetComponent<MeshFilter>();
                outer.renderer = outer.filter.gameObject.AddComponent<MeshRenderer>();
                outer.renderer.materials = materialPalette.outer;
                outer.filter.mesh = outer.mesh;
                outer.renderer.transform.parent = transform;
                outer.renderer.transform.localPosition = new Vector3(0, -radius, 0);
            }

            // Ocean
            if(ocean.mesh == null)
            {
                ocean.mesh = new Mesh();
                ocean.mesh.name = gameObject.name + " Ocean Mesh";
                ocean.filter = new GameObject("Ocean", typeof(MeshFilter)).GetComponent<MeshFilter>();
                ocean.renderer = ocean.filter.gameObject.AddComponent<MeshRenderer>();
                ocean.renderer.materials = materialPalette.ocean;
                ocean.filter.mesh = ocean.mesh;
                ocean.renderer.transform.parent = transform;
                ocean.renderer.transform.localPosition = new Vector3(0, -radius, 0);
            }

            // Continental Crust
            if(continent.mesh == null)
            {
                continent.mesh = new Mesh();
                continent.mesh.name = gameObject.name + " Continent Mesh";
                continent.filter = new GameObject("Continent", typeof(MeshFilter)).GetComponent<MeshFilter>();
                continent.renderer = continent.filter.gameObject.AddComponent<MeshRenderer>();
                continent.renderer.materials = materialPalette.continent;
                continent.filter.mesh = continent.mesh;
                continent.renderer.transform.parent = transform;
                continent.renderer.transform.localPosition = new Vector3(0, -radius, 0);
            }

            CreateShape();
            UpdateMesh();
        }

        private void OnValidate()
        {
            if (outer.mesh != null)
            {
                outer.mesh.name = gameObject.name + " Outer Ring Mesh";
                outer.renderer.materials = materialPalette.outer;
            }
            if (ocean.mesh != null)
            {
                ocean.mesh.name = gameObject.name + " Ocean Mesh";
                ocean.renderer.materials = materialPalette.ocean;
            }
            if (continent.mesh != null)
            {
                continent.mesh.name = gameObject.name + " Continent Mesh";
                continent.renderer.materials = materialPalette.continent;
            }

            width = Mathf.Clamp(width, 1, width);
            radius = Mathf.Clamp(radius, 1, radius);

            outer.widthSubdivisions = Mathf.Clamp(outer.widthSubdivisions, 2, outer.widthSubdivisions);
            outer.lengthSubdivisions = Mathf.Clamp(outer.lengthSubdivisions, 5, outer.lengthSubdivisions);
            outer.renderer.transform.localPosition = new Vector3(0, -radius, 0);

            ocean.widthSubdivisions = Mathf.Clamp(ocean.widthSubdivisions, 2, ocean.widthSubdivisions);
            ocean.lengthSubdivisions = Mathf.Clamp(ocean.lengthSubdivisions, 5, ocean.lengthSubdivisions);
            ocean.renderer.transform.localPosition = new Vector3(0, -radius, 0);

            continent.widthSubdivisions = Mathf.Clamp(continent.widthSubdivisions, 2, continent.widthSubdivisions);
            continent.lengthSubdivisions = Mathf.Clamp(continent.lengthSubdivisions, 5, continent.lengthSubdivisions);
            continent.renderer.transform.localPosition = new Vector3(0, -radius, 0);

            CreateShape();
            UpdateMesh();
        }

        private void CreateShape()
        {
            int vertIndex, triIndex;

            #region Outer Ring
            // Generating the vertices.
            outer.verts = new Vector3[outer.widthSubdivisions * outer.lengthSubdivisions];

            for(int z = 0; z < outer.lengthSubdivisions; z++)
            {
                for(int x = 0; x < outer.widthSubdivisions; x++)
                {
                    outer.verts[z * outer.widthSubdivisions + x] = new Vector3(
                        x * (width / (outer.widthSubdivisions - 1)),
                        Centre.y + radius * Mathf.Cos(360f / (outer.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        Centre.z + radius * Mathf.Sin(360f / (outer.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
                    );
                }
            }

            // Generating the triangles.
            outer.tris = new int[(outer.widthSubdivisions - 1) * (outer.lengthSubdivisions - 1) * 6];
            vertIndex = 0;
            triIndex = 0;

            for (int z = 0; z < outer.lengthSubdivisions - 1; z++)
            {
                for(int x = 0; x < outer.widthSubdivisions - 1; x++)
                {
                    outer.tris[triIndex + 0] = vertIndex + 0;
                    outer.tris[triIndex + 1] = vertIndex + outer.widthSubdivisions;
                    outer.tris[triIndex + 2] = vertIndex + 1;
                    outer.tris[triIndex + 3] = vertIndex + 1;
                    outer.tris[triIndex + 4] = vertIndex + outer.widthSubdivisions;
                    outer.tris[triIndex + 5] = vertIndex + outer.widthSubdivisions + 1;

                    vertIndex++;
                    triIndex += 6;
                }
                vertIndex++;
            }
            #endregion

            #region Ocean
            // Generating the vertices.
            ocean.verts = new Vector3[ocean.widthSubdivisions * ocean.lengthSubdivisions];

            for (int z = 0; z < ocean.lengthSubdivisions; z++)
            {
                for (int x = 0; x < ocean.widthSubdivisions; x++)
                {
                    float y = (radius - oceanHeight);
                    ocean.verts[z * ocean.widthSubdivisions + x] = new Vector3(
                        x * (width / (ocean.widthSubdivisions - 1)),
                        Centre.y + y * Mathf.Cos(360f / (ocean.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        Centre.z + (radius - oceanHeight) * Mathf.Sin(360f / (ocean.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
                    );
                }
            }

            // Generating the triangles.
            ocean.tris = new int[(ocean.widthSubdivisions - 1) * (ocean.lengthSubdivisions - 1) * 6];
            vertIndex = 0;
            triIndex = 0;

            for (int z = 0; z < ocean.lengthSubdivisions - 1; z++)
            {
                for (int x = 0; x < ocean.widthSubdivisions - 1; x++)
                {
                    ocean.tris[triIndex + 2] = vertIndex + 0;
                    ocean.tris[triIndex + 1] = vertIndex + ocean.widthSubdivisions;
                    ocean.tris[triIndex + 0] = vertIndex + 1;

                    ocean.tris[triIndex + 5] = vertIndex + 1;
                    ocean.tris[triIndex + 4] = vertIndex + ocean.widthSubdivisions;
                    ocean.tris[triIndex + 3] = vertIndex + ocean.widthSubdivisions + 1;

                    vertIndex++;
                    triIndex += 6;
                }
                vertIndex++;
            }
            #endregion

            #region Continental Crust
            // Generating the vertices.
            continent.verts = new Vector3[continent.widthSubdivisions * continent.lengthSubdivisions];

            for (int z = 0; z < continent.lengthSubdivisions; z++)
            {
                for (int x = 0; x < continent.widthSubdivisions; x++)
                {
                    float y = continentMinHeight + Mathf.PerlinNoise(x * continentPerlinOffset.x, z * continentPerlinOffset.y) * continentMaxHeight;
                    continent.verts[z * continent.widthSubdivisions + x] = new Vector3(
                        x * (width / (continent.widthSubdivisions - 1)),
                        Centre.y + (radius - y) * Mathf.Cos(360f / (continent.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        Centre.z + (radius - y) * Mathf.Sin(360f / (continent.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
                    );
                }
            }

            // Generating the triangles.
            continent.tris = new int[(continent.widthSubdivisions - 1) * (continent.lengthSubdivisions - 1) * 6];
            vertIndex = 0;
            triIndex = 0;

            for (int z = 0; z < continent.lengthSubdivisions - 1; z++)
            {
                for (int x = 0; x < continent.widthSubdivisions - 1; x++)
                {
                    continent.tris[triIndex + 2] = vertIndex + 0;
                    continent.tris[triIndex + 1] = vertIndex + continent.widthSubdivisions;
                    continent.tris[triIndex + 0] = vertIndex + 1;

                    continent.tris[triIndex + 5] = vertIndex + 1;
                    continent.tris[triIndex + 4] = vertIndex + continent.widthSubdivisions;
                    continent.tris[triIndex + 3] = vertIndex + continent.widthSubdivisions + 1;

                    vertIndex++;
                    triIndex += 6;
                }
                vertIndex++;
            }
            #endregion
        }

        private void UpdateMesh()
        {
            if (outer.mesh != null)
            {
                outer.mesh.Clear();

                outer.mesh.vertices = outer.verts;
                outer.mesh.triangles = outer.tris;
                outer.RecalculateUVs();
                outer.mesh.uv = outer.uv;

                outer.mesh.RecalculateNormals();
            }

            if (ocean.mesh != null)
            {
                ocean.mesh.Clear();

                ocean.mesh.vertices = ocean.verts;
                ocean.mesh.triangles = ocean.tris;
                ocean.RecalculateUVs();
                ocean.mesh.uv = ocean.uv;

                ocean.mesh.RecalculateNormals();
            }

            if (continent.mesh != null)
            {
                continent.mesh.Clear();

                continent.mesh.vertices = continent.verts;
                continent.mesh.triangles = continent.tris;
                continent.RecalculateUVs();
                continent.mesh.uv = continent.uv;

                continent.mesh.RecalculateNormals();
            }
        }
        
        private void SaveAndExit()
        {
            outer.mesh.RecalculateBounds();
            ocean.mesh.RecalculateBounds();
            continent.mesh.RecalculateBounds();

            if (generateCollider)
            {
                if (!outer.renderer.gameObject.TryGetComponent(out MeshCollider meshCollider))
                {
                    meshCollider = outer.renderer.gameObject.AddComponent<MeshCollider>();
                }
                meshCollider.sharedMesh = outer.mesh;

                if (!continent.renderer.gameObject.TryGetComponent(out meshCollider))
                {
                    meshCollider = continent.renderer.gameObject.AddComponent<MeshCollider>();
                }
                meshCollider.sharedMesh = continent.mesh;
            }

            if (!gameObject.TryGetComponent(out Ringworld ring))
            {
                ring = gameObject.AddComponent<Ringworld>();
            }
            ring.spinningSpeed = spinningSpeed;
            ring.radius = radius;
            ring.width = width;
            ring.UpdateBounds();

            enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (outer.verts != null)
            {
                for(int i = 0; i < outer.verts.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(outer.verts[i], 0.25f);
                }
            }

            if(ocean.verts != null)
            {
                for (int i = 0; i < ocean.verts.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(ocean.verts[i], 0.25f);
                }
            }

            if(continent.verts != null)
            {
                for (int i = 0; i < continent.verts.Length; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(continent.verts[i], 0.25f);
                }
            }
        }
    }
}
