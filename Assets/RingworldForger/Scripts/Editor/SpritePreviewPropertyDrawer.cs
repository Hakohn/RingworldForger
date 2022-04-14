using UnityEditor;
using UnityEngine;

namespace ChironPE
{
	[CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewPropertyDrawer : PropertyDrawer
    {
        private float textSize = 100;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent labelN)
        {
            if (property.objectReferenceValue != null)
            {
                return textSize;
            }
            else
            {
                return base.GetPropertyHeight(property, labelN);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            SpritePreviewAttribute spritePrevAtt = (SpritePreviewAttribute)attribute;
            textSize = spritePrevAtt.PreviewSize;


            EditorGUI.BeginProperty(position, label, prop);
            Rect labelRect, spriteRect;
            if(prop.objectReferenceValue != null)
			{
                if(spritePrevAtt.IncludeLabel)
				{
                    labelRect = new Rect(position.x, position.y - textSize / 2 + EditorGUIUtility.singleLineHeight / 2, position.width, position.height);
                    spriteRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, position.y, textSize, textSize);
                    GUI.Label(labelRect, prop.displayName);
				}
                else
				{
                    spriteRect = new Rect(position.x, position.y, textSize, textSize);
				}
            }
            else
			{
                if(spritePrevAtt.IncludeLabel)
				{
                    labelRect = position;
                    spriteRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, position.y, labelRect.width - EditorGUIUtility.labelWidth, labelRect.height);
                    GUI.Label(labelRect, prop.displayName);
                }
                else
				{
                    if(spritePrevAtt.ForceSpriteSize)
                        spriteRect = new Rect(position.x, position.y, textSize, textSize);
                    else
                        spriteRect = position;
				}
            }

            Object spriteValue = EditorGUI.ObjectField(spriteRect, prop.objectReferenceValue, typeof(Sprite), false);
            if(!prop.serializedObject.isEditingMultipleObjects)
            {
                prop.objectReferenceValue = spriteValue;
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(TexturePreviewAttribute))]
    public class TexturePreviewPropertyDrawer : PropertyDrawer
    {
        private float textSize = 100;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent labelN)
        {
            if (property.objectReferenceValue != null)
            {
                return textSize;
            }
            else
            {
                return base.GetPropertyHeight(property, labelN);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            TexturePreviewAttribute texPrevAtt = (TexturePreviewAttribute)attribute;
            textSize = texPrevAtt.PreviewSize;


            EditorGUI.BeginProperty(position, label, prop);
            Rect labelRect, texRect;
            if (prop.objectReferenceValue != null)
            {
                if (texPrevAtt.IncludeLabel)
                {
                    labelRect = new Rect(position.x, position.y - textSize / 2 + EditorGUIUtility.singleLineHeight / 2, position.width, position.height);
                    texRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, position.y, textSize, textSize);
                    GUI.Label(labelRect, prop.displayName);
                }
                else
                {
                    texRect = new Rect(position.x, position.y, textSize, textSize);
                }
            }
            else
            {
                if (texPrevAtt.IncludeLabel)
                {
                    labelRect = position;
                    texRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth, position.y, labelRect.width - EditorGUIUtility.labelWidth, labelRect.height);
                    GUI.Label(labelRect, prop.displayName);
                }
                else
                {
                    if (texPrevAtt.ForceTextureSize)
                        texRect = new Rect(position.x, position.y, textSize, textSize);
                    else
                        texRect = position;
                }
            }

            Object texValue = EditorGUI.ObjectField(texRect, prop.objectReferenceValue, typeof(Texture), false);
            if (!prop.serializedObject.isEditingMultipleObjects)
            {
                prop.objectReferenceValue = texValue;
            }

            EditorGUI.EndProperty();
        }
    }
}