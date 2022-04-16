using UnityEngine;
using UnityEditor;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(RingLayer))]
    [CanEditMultipleObjects]
    public class RingLayerEditor : UnityEditor.Editor
    {
        [MenuItem("Ringworld Forger/Create/New Ring Layer")]
        [MenuItem("GameObject/3D Object/Ringworld/Ring Layer")]
        public static RingLayer CreateNewRingLayer()
        {
            // Creating the new game object with the necessary components onto it.
            GameObject rfObj = new GameObject();
            RingLayer rl = rfObj.AddComponent<RingLayer>();

            // Focus the editor onto the object.
            Selection.activeGameObject = rfObj;

            // Make sure that the Ringworld name is unique.
            rfObj.name = "Ring Layer";

            return rl;
        }

        public override void OnInspectorGUI()
        {
            RingLayer rl = target as RingLayer;

            bool displayDrawButton = !rl.autoRefresh;
            if (DrawDefaultInspector())
            {
                if (rl.autoRefresh)
                {
                    rl.RefreshLayerChunks();
                }
            }

            if (displayDrawButton/* && !rl.isRingworldForgerControlled*/)
            {
                if (GUILayout.Button("Refresh Layer Chunks"))
                {
                    rl.RefreshLayerChunks();
                }
            }

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("(Re)Spawn Vegetation"))
            {
                rl.RefreshVegetation();
            }
            if(GUILayout.Button("Clear Vegetation"))
            {
                rl.ClearVegetation();
            }
            GUILayout.EndHorizontal();

            //EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
