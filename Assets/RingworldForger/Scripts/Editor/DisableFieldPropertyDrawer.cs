using UnityEditor;
using UnityEngine;

namespace ChironPE
{
    [CustomPropertyDrawer(typeof(DisableFieldAttribute))]
    public class DisableFieldPropertyDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}