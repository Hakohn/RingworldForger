using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = target as MapGenerator;

        bool displayDrawButton = !mapGen.autoRedraw;
        if(DrawDefaultInspector())
        {
            if(mapGen.autoRedraw)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if(displayDrawButton)
        {
            if(GUILayout.Button("Draw"))
            {
                mapGen.DrawMapInEditor();
            }
        }
    }
}
