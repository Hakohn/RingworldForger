using UnityEngine;
using UnityEditor;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(RingLayer))]
    [CanEditMultipleObjects]
    public class RingLayerEditor : UnityEditor.Editor
    {
        [MenuItem("Ringworld Forger/Create/New Ring Layer")]
        [MenuItem("GameObject/3D Object/Ring Layer")]
        private static void CreateNewRingLayerEditor()
        {
            CreateNewRingLayer();
        }

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

            bool displayDrawButton = !rl.autoRecreate;
            if (DrawDefaultInspector())
            {
                if (rl.autoRecreate)
                {
                    rl.RefreshLayerChunks();
                }
            }

            if (displayDrawButton/* && !rl.isRingworldForgerControlled*/)
            {
                if (GUILayout.Button("Refresh"))
                {
                    rl.RefreshLayerChunks();
                }
            }

            //EditorApplication.QueuePlayerLoopUpdate();
            //SceneView.RepaintAll();

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
