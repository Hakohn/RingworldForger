using UnityEngine;
using UnityEditor;

namespace ChironPE.Editor
{
    [CustomEditor(typeof(CentrifugalBody))]
    [CanEditMultipleObjects]
    public class CentrifugalBodyEditor : UnityEditor.Editor
    {
        [MenuItem("Ringworld Forger/Create/New Centrifugal Body")]
        [MenuItem("GameObject/3D Object/Ringworld/Centrifugal Body")]
        public static CentrifugalBody CreateNewCentrifugalBody()
        {
            // Creating the new game object with the necessary components onto it.
            GameObject rfObj = new GameObject();
            CentrifugalBody rl = rfObj.AddComponent<CentrifugalBody>();
            rl.GetComponent<Rigidbody>().useGravity = false;

            // Focus the editor onto the object.
            Selection.activeGameObject = rfObj;

            // Make sure that the Ringworld name is unique.
            rfObj.name = "Centrifugal Body";

            return rl;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CentrifugalBody rl = target as CentrifugalBody;

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
