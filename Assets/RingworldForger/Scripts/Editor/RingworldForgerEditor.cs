using UnityEngine;
using UnityEditor;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(RingworldForger))]
    [CanEditMultipleObjects]
    public class RingworldForgerEditor : UnityEditor.Editor
    {
        [MenuItem("Ringworld Forger/Create/New Ringworld")]
        [MenuItem("GameObject/3D Object/Ringworld/Ringworld")]
        private static void CreateNewRingworldForger()
        {
            // Creating the new game object with the necessary components onto it.
            GameObject rfObj = new GameObject();
            RingworldForger rf = rfObj.AddComponent<RingworldForger>();

            // Make sure that the Ringworld name is unique.
            string objName = "Ringworld";
            if (GameObject.Find(objName) != null)
            {
                int index = 1;
                do
                {
                    objName = $"Ringworld ({index++})";
                }
                while (GameObject.Find(objName) != null);
            }
            rfObj.name = objName;

            #region Create the basic 3 Layers
            RingLayer outerLayer = RingLayerEditor.CreateNewRingLayer();
            outerLayer.transform.parent = rf.transform;
            outerLayer.gameObject.name = "Outer";
            outerLayer.noiseScale = 1f;
            outerLayer.invertFaces = true;
            outerLayer.meshHeightMultiplier = 0;
            outerLayer.meshHeightCurve = AnimationCurve.Constant(0, 1, 1);
            outerLayer.biomes = new Biome[]
            {
                new Biome
                {
                    name = "Metal",
                    startHeight = 0,
                    tint = Color.gray
                }
            };
            outerLayer.seed = Random.Range(int.MinValue, int.MaxValue);
            outerLayer.generateColliders = false;

            RingLayer oceanLayer = RingLayerEditor.CreateNewRingLayer();
            oceanLayer.transform.parent = rf.transform;
            oceanLayer.gameObject.name = "Ocean";
            oceanLayer.noiseScale = 1f;
            oceanLayer.invertFaces = false;
            oceanLayer.meshHeightMultiplier = 4.5f;
            oceanLayer.meshHeightCurve = AnimationCurve.Constant(0, 1, 1);
            oceanLayer.biomes = new Biome[]
            {
                new Biome
                {
                    name = "Ocean",
                    tintStrength = 1f,
                    startHeight = 0,
                    tint = new Color(0, 0.5f, 1f, 0f)
                }
            };
            oceanLayer.seed = Random.Range(int.MinValue, int.MaxValue);
            oceanLayer.generateColliders = false;

            RingLayer terrainLayer = RingLayerEditor.CreateNewRingLayer();
            terrainLayer.transform.parent = rf.transform;
            terrainLayer.gameObject.name = "Terrain";
            terrainLayer.noiseScale = 200f;
            terrainLayer.invertFaces = false;
            terrainLayer.meshHeightMultiplier = 100f;
            terrainLayer.meshHeightCurve = new AnimationCurve(new Keyframe[] {
                new Keyframe
                {
                    time = 0f,
                    value = 0f,
                },
                new Keyframe
                {
                    time = 0.457f,
                    value = 0.077f,
                },
                new Keyframe
                {
                    time = 0.668f,
                    value = 0.266f,
                },
                new Keyframe
                {
                    time = 1.0f,
                    value = 1.0f,
                },
            });
            terrainLayer.biomes = new Biome[]
            {
                new Biome
                {
                    name = "Sand",
                    tintStrength = 1f,
                    startHeight = 0.0f,
                    blendStrength = 0f,
                    tint = new Color(0.8117648f, 0.8196079f, 0.4941177f, 0f)
                },
                new Biome
                {
                    name = "Grass",
                    tintStrength = 1f,
                    startHeight = 0.067f,
                    blendStrength = 0.021f,
                    tint = new Color(0.2470588f, 0.4156863f, 0.07450981f, 0f)
                },
                new Biome
                {
                    name = "Rock",
                    tintStrength = 1f,
                    startHeight = 0.52f,
                    blendStrength = 0.0238f,
                    tint = new Color(0.2627451f, 0.2078432f, 0.1882353f, 0f)
                }
            };
            terrainLayer.seed = Random.Range(int.MinValue, int.MaxValue);
            terrainLayer.generateColliders = true;
            #endregion

            rf.OnValidate();
            rf.RefreshRingChunks();

            // Focus the editor onto the object.
            Selection.activeGameObject = rfObj;
        }

        public override void OnInspectorGUI()
        {
            RingworldForger rf = target as RingworldForger;

            bool displayDrawButton = !rf.autoRefresh;

            // If something has changed...
            if (DrawDefaultInspector())
            {
                if (rf.autoRefresh)
                {
                    rf.RefreshRingChunks();
                }
            }
            //EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();

            if (displayDrawButton)
            {
                if (GUILayout.Button("Refresh"))
                {
                    rf.RefreshRingChunks();
                }
            }

            if (GUILayout.Button("Add New Ring Layer"))
            {
                RingLayer layer = RingLayerEditor.CreateNewRingLayer();
                layer.transform.parent = rf.transform;
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
