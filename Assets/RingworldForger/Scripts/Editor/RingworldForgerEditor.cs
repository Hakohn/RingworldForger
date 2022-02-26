using UnityEditor;
using UnityEngine;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(RingworldForger))]
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
            if(GameObject.Find(objName) != null)
            {
                int index = 1;
                do
                {
                    objName = $"Ringworld ({index++})";
                } 
                while (GameObject.Find(objName) != null);
            }
            rfObj.name = objName;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RingworldForger rf = serializedObject.targetObject as RingworldForger;

            if (GUILayout.Button("Create"))
            {
                //rf.Invoke("CreateShape", 0);
                //rf.Invoke("UpdateMesh", 0);
                rf.Invoke("SaveAndExit", 0);

                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();

                //Debug.Log("RingworldForger: Ringworld forged.");
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
