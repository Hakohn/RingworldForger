using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RingGenerator))]
[CanEditMultipleObjects]
public class RingGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RingGenerator ringGen = target as RingGenerator;

        bool displayDrawButton = !ringGen.autoRedraw;
        if (DrawDefaultInspector())
        {
            if (ringGen.autoRedraw)
            {
                ringGen.CreateRingChunks();
            }
        }

        if (displayDrawButton)
        {
            if (GUILayout.Button("Draw"))
            {
                ringGen.CreateRingChunks();
            }
        }
    }
}
