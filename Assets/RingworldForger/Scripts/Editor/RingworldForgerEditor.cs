using UnityEngine;
using UnityEditor;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(RingworldForger))]
    [CanEditMultipleObjects]
    public class RingworldForgerEditor : UnityEditor.Editor
    {
        [MenuItem("Ringworld Forger/Create/New Ringworld")]
        [MenuItem("GameObject/3D Object/Ringworld")]
        private static void CreateNewRingworldForger()
        {
            // Creating the new game object with the necessary components onto it.
            GameObject rfObj = new GameObject();
            RingworldForger rf = rfObj.AddComponent<RingworldForger>();

            // Focus the editor onto the object.
            Selection.activeGameObject = rfObj;

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
                    height = 0,
                    colour = Color.gray
                }
            };

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
                    height = 0,
                    colour = new Color(0.3176471f, 0.6964f, 0)
                }
            };

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
                    height = 0.25f,
                    colour = new Color(0.8117648f, 0.8196079f, 0.4941177f)
                },
                new Biome
                {
                    name = "Grass",
                    height = 0.57f,
                    colour = new Color(0.2470588f, 0.4156863f, 0.07450981f)
                },
                new Biome
                {
                    name = "Rock",
                    height = 0.87f,
                    colour = new Color(0.2627451f, 0.2078432f, 0.1882353f)
                }
            };
            #endregion

            rf.OnValidate();
            rf.RefreshRingChunks();
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

                rf.info.layers = rf.GetComponentsInChildren<RingLayer>();
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
