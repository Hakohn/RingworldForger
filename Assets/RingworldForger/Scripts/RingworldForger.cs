using UnityEngine;

namespace ChironPE
{
    [ExecuteInEditMode]
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
        private GenerationAlgorithmSO[] generationAlgorithms = new GenerationAlgorithmSO[0];

        [Header("On Save")]
        [SerializeField]
        private bool generateCollider = false;

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
                outer.renderer.transform.localPosition = Vector3.zero;
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
                ocean.renderer.transform.localPosition = Vector3.zero;
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
                continent.renderer.transform.localPosition = Vector3.zero;
            }
        }

        private void OnValidate()
        {
            if (outer == null) outer = new RingLayer();
            if (outer.mesh != null)
            {
                outer.mesh.name = gameObject.name + " Outer Ring Mesh";
                outer.renderer.materials = materialPalette.outer;
            }
            if (ocean == null) ocean = new RingLayer();
            if (ocean.mesh != null)
            {
                ocean.mesh.name = gameObject.name + " Ocean Mesh";
                ocean.renderer.materials = materialPalette.ocean;
            }
            if (continent == null) continent = new RingLayer();
            if (continent.mesh != null)
            {
                continent.mesh.name = gameObject.name + " Continent Mesh";
                continent.renderer.materials = materialPalette.continent;
            }

            width = Mathf.Clamp(width, 1, width);
            radius = Mathf.Clamp(radius, 1, radius);

            outer.widthSubdivisions = Mathf.Clamp(outer.widthSubdivisions, 2, outer.widthSubdivisions);
            outer.lengthSubdivisions = Mathf.Clamp(outer.lengthSubdivisions, 5, outer.lengthSubdivisions);

            ocean.widthSubdivisions = Mathf.Clamp(ocean.widthSubdivisions, 2, ocean.widthSubdivisions);
            ocean.lengthSubdivisions = Mathf.Clamp(ocean.lengthSubdivisions, 5, ocean.lengthSubdivisions);

            continent.widthSubdivisions = Mathf.Clamp(continent.widthSubdivisions, 2, continent.widthSubdivisions);
            continent.lengthSubdivisions = Mathf.Clamp(continent.lengthSubdivisions, 5, continent.lengthSubdivisions);
        }

        private void CreateShape()
        {
            int vertIndex, triIndex;

            Vector3 localCentre = new Vector3(- width / 2, 0, 0);

            #region Outer Ring
            // Generating the vertices.
            outer.verts = new Vector3[outer.widthSubdivisions * outer.lengthSubdivisions];

            for(int z = 0; z < outer.lengthSubdivisions; z++)
            {
                for(int x = 0; x < outer.widthSubdivisions; x++)
                {
                    outer.verts[z * outer.widthSubdivisions + x] = (new Vector3(
                        localCentre.x + x * (width / (outer.widthSubdivisions - 1)),
                        localCentre.y + radius * Mathf.Cos(360f / (outer.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        localCentre.z + radius * Mathf.Sin(360f / (outer.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
                    ));
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
                    ocean.verts[z * ocean.widthSubdivisions + x] = (new Vector3(
                        localCentre.x + x * (width / (ocean.widthSubdivisions - 1)),
                        localCentre.y + y * Mathf.Cos(360f / (ocean.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        localCentre.z + (radius - oceanHeight) * Mathf.Sin(360f / (ocean.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
                    ));
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


            // Setting up the heightmap.
            Vector2Int heightMapSize = new Vector2Int(continent.widthSubdivisions, continent.lengthSubdivisions);
            float[,] heightMap = new float[heightMapSize.x, heightMapSize.y];
            for (int x = 0; x < heightMapSize.x; x++)
            {
                for (int z = 0; z < heightMapSize.y; z++)
                {
                    heightMap[x, z] = 0;
                }
            }

            // Layering the algorithms on top of each other.
            foreach (GenerationAlgorithmSO ga in generationAlgorithms)
            {
                if (ga == null) continue;
                float[,] otherHeightMap = ga.GenerateHeightmap(heightMapSize);

                for(int x = 0; x < heightMapSize.x; x++)
                {
                    for(int z = 0; z < heightMapSize.y; z++)
                    {
                        heightMap[x, z] += otherHeightMap[x, z];
                    }
                }
            }

            // Applying the height map.
            for (int x = 0; x < heightMapSize.x; x++)
            {
                for (int z = 0; z < heightMapSize.y; z++)
                {
                    float h = heightMap[x, z];
                    continent.verts[z * continent.widthSubdivisions + x] = new Vector3(
                        localCentre.x + x * (width / (continent.widthSubdivisions - 1)),
                        localCentre.y + (radius - h) * Mathf.Cos(360f / (continent.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)/* - y*/,
                        localCentre.z + (radius - h) * Mathf.Sin(360f / (continent.lengthSubdivisions - 1) * z * Mathf.Deg2Rad)
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
                    Gizmos.DrawWireSphere(transform.TransformPoint(outer.verts[i]), 0.25f);
                }
            }

            if(ocean.verts != null)
            {
                for (int i = 0; i < ocean.verts.Length; i++)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(transform.TransformPoint(ocean.verts[i]), 0.25f);
                }
            }

            if(continent.verts != null)
            {
                for (int i = 0; i < continent.verts.Length; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(transform.TransformPoint(continent.verts[i]), 0.25f);
                }
            }
        }
    }
}
