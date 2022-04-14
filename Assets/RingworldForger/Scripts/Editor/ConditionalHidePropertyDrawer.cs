using UnityEngine;
using UnityEditor;

namespace ChironPE
{
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        #region Unity Methods
        /// <summary>
        /// When Unity wants to draw the property in the inspector we need to:
        /// <list type="bullet">
        /// <item> <i> Check the parameters that we used in our custom attribute </i> </item>
        /// <item> <i> Hide and/or disable the property that is being drawn based on the attribute parameters </i> </item>
        /// </list>
        /// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHideAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHideAtt, property);

            EditorGUI.BeginDisabledGroup(!enabled);
            if (!condHideAtt.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Calculate the height of our property so that (when the property needs to be hidden) the following properties that are being drawn don’t overlap.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHideAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHideAtt, property);

            if (!condHideAtt.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// In order to check if the property should be enabled or not we call GetConditionalHideAttributeResult.
        /// </summary>
        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHideAtt, SerializedProperty property)
        {
            bool enabled = true;
            string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
            string conditionPath = propertyPath.Replace(property.name, condHideAtt.ConditionalSourceField); //changes the path to the conditional source property path
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

            if (sourcePropertyValue != null)
            {
                if(!condHideAtt.Reversed)
                    enabled = sourcePropertyValue.boolValue;
                else
                    enabled = !sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + condHideAtt.ConditionalSourceField);
            }

            return enabled;
        }
		#endregion
	}
}